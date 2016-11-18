/*
    sb0t ares chat server
    Copyright (C) 2016  AresChat

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using captcha;
using iconnect;
using core.LinkHub;
using System.Net;
using commands;

namespace core
{
    class TCPProcessor
    {
        public static void Eval(AresClient client, TCPPacket packet, ulong time)
        {
            if (!client.LoggedIn && (packet.Msg > TCPMsg.MSG_CHAT_CLIENT_LOGIN && packet.Msg != TCPMsg.MSG_LINK_PROTO))
                throw new Exception("unordered login routine");

            if (client.LoggedIn)
                if (FloodControl.IsFlooding(client, packet.Msg, packet.Packet.ToArray(), time))
                    if (Events.Flooding(client, (byte)packet.Msg))
                    {
                        Events.Flooded(client);
                        client.Disconnect();
                        return;
                    }

            switch (packet.Msg)
            {
                case TCPMsg.MSG_CHAT_CLIENT_LOGIN:
                case TCPMsg.MSG_CHAT_CLIENT_RELOGIN:
                    Login(client, packet.Packet, time, packet.Msg == TCPMsg.MSG_CHAT_CLIENT_RELOGIN);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_UPDATE_STATUS:
                    client.Time = time;
                    client.SendPacket(TCPOutbound.UpdateUserStatus(client, client));
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_AVATAR:
                    Avatar(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_PERSONAL_MESSAGE:
                    PersonalMessage(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_FASTPING:
                    client.FastPing = true;
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_CUSTOM_DATA:
                    CustomData(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_CUSTOM_DATA_ALL:
                    CustomDataAll(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL:
                    TCPAdvancedProcessor.Eval(client, packet, time);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_PUBLIC:
                    Public(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_EMOTE:
                    Emote(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_COMMAND:
                    Command(client, packet.Packet.ReadString(client));
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_PVT:
                    Private(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_IGNORELIST:
                    IgnoreList(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_ADDSHARE:
                    FileBrowseProcessor.AddShare(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_REMSHARE:
                    FileBrowseProcessor.RemShare(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_BROWSE:
                    FileBrowseProcessor.Browse(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_SEARCH:
                    FileBrowseProcessor.Search(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_AUTHLOGIN:
                    Command(client, "login " + packet.Packet.ReadString(client));
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_AUTHREGISTER:
                    Command(client, "register " + packet.Packet.ReadString(client));
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_AUTOLOGIN:
                    SecureLogin(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_DUMMY:
                    Dummy(client);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_DIRCHATPUSH:
                    DirChatPush(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_SEND_SUPERNODES:
                    client.SendPacket(TCPOutbound.SuperNodes());
                    break;

                case TCPMsg.MSG_LINK_PROTO:
                    LinkProto(client, packet.Packet, time);
                    break;

                default:
                    Events.UnhandledProtocol(client, false, packet.Msg, packet.Packet, time);
                    break;
            }
        }

        private static void LinkProto(AresClient client, TCPPacketReader packet, ulong time)
        {
            Leaf leaf = new Leaf(client.Sock, time, packet.ToArray());
            client.IsLeaf = true;
            client.Sock = null;
            LeafPool.Leaves.Add(leaf);
        }

        private static void Dummy(AresClient client)
        {
            if (Events.ProxyDetected(client))
                client.Disconnect();
        }

        private static void SecureLogin(AresClient client, TCPPacketReader packet)
        {
            if (!client.HasSecureLoginAttempted)
            {
                client.HasSecureLoginAttempted = true;
                AccountManager.SecureLogin(client, packet);
            }
        }

        private static void DirChatPush(AresClient client, TCPPacketReader packet)
        {
            if (client.Quarantined)
                return;

            String name = packet.ReadString(client);

            if (Encoding.UTF8.GetByteCount(name) < 2)
                client.SendPacket(new byte[] { 1, 0, (byte)TCPMsg.MSG_CHAT_CLIENT_DIRCHATPUSH, 4 });
            else if (packet.Remaining != 16)
                client.SendPacket(new byte[] { 1, 0, (byte)TCPMsg.MSG_CHAT_CLIENT_DIRCHATPUSH, 3 });
            else
            {
                byte[] cookie = packet;
                AresClient target = UserPool.AUsers.Find(x => x.Name == name);

                if (target == null)
                    client.SendPacket(new byte[] { 1, 0, (byte)TCPMsg.MSG_CHAT_CLIENT_DIRCHATPUSH, 1 });
                else if (target.IgnoreList.Contains(client.Name))
                    client.SendPacket(new byte[] { 1, 0, (byte)TCPMsg.MSG_CHAT_CLIENT_DIRCHATPUSH, 2 });
                else
                {
                    client.SendPacket(new byte[] { 1, 0, (byte)TCPMsg.MSG_CHAT_CLIENT_DIRCHATPUSH, 0 });
                    target.SendPacket(TCPOutbound.DirectChatPush(target, client, cookie));
                }
            }
        }

        private static void IgnoreList(AresClient client, TCPPacketReader packet)
        {
            if (client.Quarantined)
                return;

            bool ignore = (byte)packet != 0;
            String name = packet.ReadString(client);
            IClient target = UserPool.AUsers.Find(x => x.Name == name);

            ignore = !client.IgnoreList.Contains(name);

            if (target == null)
                target = UserPool.WUsers.Find(x => x.Name == name);

            if (target == null && ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                target = ServerCore.Linker.FindUser(x => x.Name == name);

            if (target != null)
                if (!ignore)
                {
                    client.IgnoreList.RemoveAll(x => x == name);
                    Events.IgnoredStateChanged(client, target, ignore);
                    IgnoreManager.RemoveIgnore(client, name);
                }
                else if (Events.Ignoring(client, target))
                    if (client.SocketConnected)
                    {
                        if (!client.IgnoreList.Contains(name))
                        {
                            client.IgnoreList.Add(name);
                            IgnoreManager.AddIgnore(client, name);
                        }

                        Events.IgnoredStateChanged(client, target, ignore);
                    }
        }

        private static void Command(AresClient client, String text)
        {
            if (text.Length > 300)
                text = text.Substring(0, 300);

            Command cmd = new Command { Text = text, Args = String.Empty };
            Helpers.PopulateCommand(cmd);
            Events.Command(client, text, cmd.Target, cmd.Args);
        }

        private static void CustomData(AresClient client, TCPPacketReader packet)
        {
            if (client.Quarantined || !client.Captcha)
                return;

            String ident = packet.ReadString(client);
            String name = packet.ReadString(client);
            byte[] data = packet;
            AresClient target = UserPool.AUsers.Find(x => x.Name == name && x.LoggedIn && x.CustomClient);

            if (ident == "cb0t_pm_msg")
            {
                if (target != null)
                {
                    if (target.IgnoreList.Contains(client.Name) || client.Muzzled)
                        client.SendPacket(TCPOutbound.IsIgnoringYou(client, name));
                    else
                    {
                        if (target.Cloaked)
                        {
                            client.SendPacket(TCPOutbound.OfflineUser(client, name));
                            return;
                        }

                        PMEventArgs args = new PMEventArgs { Cancel = false, Text = "          " };
                        Events.PrivateSending(client, target, args);

                        if (!args.Cancel && client.SocketConnected)
                        {
                            target.SendPacket(TCPOutbound.CustomData(target, client.Name, ident, data));
                            Events.PrivateSent(client, target, args.Text);
                        }
                    }
                }
                else client.SendPacket(TCPOutbound.OfflineUser(client, name));
            }
            else
            {
                if (target != null)
                {
                    if(ident == "cb0t_scribble_once" && !Events.CanPrivateMessage(client, target))
                    {
                        return;
                    }
                    if (!target.IgnoreList.Contains(client.Name))
                        target.SendPacket(TCPOutbound.CustomData(target, client.Name, ident, data));
                }
                else if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                {
                    IClient linked = ServerCore.Linker.FindUser(x => x.Name == name && x.CustomClient);

                    if (linked != null)
                        ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafCustomDataTo(ServerCore.Linker,
                            linked.IUser.Link.Ident, client.Name, linked.Name, ident, data));
                }
            }
        }

        private static void Private(AresClient client, TCPPacketReader packet)
        {
            String name = packet.ReadString(client);
            String text = packet.ReadString(client);

            if (text.Length > 300)
                text = text.Substring(0, 300);

            PMEventArgs args = new PMEventArgs { Cancel = false, Text = text };

            if (name == Settings.Get<String>("bot"))
            {
                if (text.StartsWith("#login") || text.StartsWith("#register"))
                {
                    Command(client, text.Substring(1));
                    return;
                }
                else
                {
                    if (text.StartsWith("#") || text.StartsWith("/"))
                        Command(client, text.Substring(1));

                    if (!client.Quarantined)
                        Events.BotPrivateSent(client, args.Text);
                }
            }
            else
            {
                if (!client.Captcha)
                    return;

                IClient target = UserPool.AUsers.Find(x => x.Name == name && x.LoggedIn);

                if (target == null)
                    target = UserPool.WUsers.Find(x => x.Name == name && x.LoggedIn);

                if (target == null && ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                {
                    target = ServerCore.Linker.FindUser(x => x.Name == name);

                    if (target != null)
                    {
                        ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafPrivateText(ServerCore.Linker, client.Name, target, text));
                        return;
                    }
                }

                if (target == null)
                    client.SendPacket(TCPOutbound.OfflineUser(client, name));
                else if (target.IgnoreList.Contains(client.Name) || client.Muzzled)
                    client.SendPacket(TCPOutbound.IsIgnoringYou(client, name));
                else
                {
                    if (target.Cloaked)
                    {
                        client.SendPacket(TCPOutbound.OfflineUser(client, name));
                        return;
                    }

                    Events.PrivateSending(client, target, args);

                    if (!args.Cancel && !String.IsNullOrEmpty(args.Text) && client.SocketConnected)
                    {
                        target.IUser.PM(client.Name, args.Text);
                        Events.PrivateSent(client, target, args.Text);
                    }
                }
            }
        }

        private static void Public(AresClient client, TCPPacketReader packet)
        {
            String text = packet.ReadString(client);

            if (text.Length > 300)
                text = text.Substring(0, 300);

            if (text.StartsWith("#login") || text.StartsWith("#register"))
            {
                Command(client, text.Substring(1));
                return;
            }

            if (text.StartsWith("#") && client.SocketConnected)
                Command(client, text.Substring(1));

            if (client.SocketConnected)
            {
                if (!client.Captcha)
                {
                    if (String.IsNullOrEmpty(client.CaptchaWord) || (client.CaptchaWord.Length > 0 && client.CaptchaWord.ToUpper() != Helpers.StripColors(text).Trim().ToUpper()))
                    {
                        if (client.CaptchaWord.Length > 0 && client.CaptchaWord.ToUpper() != Helpers.StripColors(text).Trim().ToUpper())
                        {
                            Events.CaptchaReply(client, text);

                            if (!client.SocketConnected)
                                return;
                        }

                        CaptchaItem cap = Captcha.Create();
                        client.CaptchaWord = cap.Word;
                        Events.CaptchaSending(client);
                        client.SendPacket(TCPOutbound.NoSuch(client, String.Empty));

                        foreach (String str in cap.Lines)
                            client.SendPacket(TCPOutbound.NoSuch(client, str));

                        client.SendPacket(TCPOutbound.NoSuch(client, String.Empty));
                        return;
                    }
                    else
                    {
                        client.Captcha = true;
                        Events.CaptchaReply(client, text);
                        CaptchaManager.AddCaptcha(client);

                        if (client.Quarantined)
                            client.Unquarantine();

                        return;
                    }
                }
                else Events.TextReceived(client, text);
            }
            else return;

            if (client.SocketConnected)
            {
                text = Events.TextSending(client, text);

                if (!String.IsNullOrEmpty(text) && client.SocketConnected && !client.Muzzled)
                {
                    if (client.Idled)
                    {
                        uint seconds_away = (uint)((Time.Now - client.IdleStart) / 1000);
                        IdleManager.Remove(client);
                        Events.Unidled(client, seconds_away);
                    }

                    if (client.SocketConnected)
                    {
                        byte[] js_style = null;
                        AresFont font = (AresFont)client.Font;

                        if (font.Enabled)
                        {
                            font.IsEmote = !String.IsNullOrEmpty(client.CustomName);
                            js_style = TCPOutbound.Font(font);
                        }

                        UserPool.AUsers.ForEachWhere(x =>
                        {
                            if (x.SupportsHTML && x.Ares)
                            {
                                if (String.IsNullOrEmpty(client.CustomName) || x.BlockCustomNames)
                                {
                                    if (x.SupportsHTML)
                                        if (js_style != null)
                                            x.SendPacket(js_style);

                                    x.SendPacket(TCPOutbound.Public(x, client.Name, text));
                                }
                                else
                                {
                                    if (x.SupportsHTML)
                                        if (js_style != null)
                                            x.SendPacket(js_style);

                                    x.SendPacket(TCPOutbound.NoSuch(x, client.CustomName + text));
                                }
                            }
                            else
                            {
                                if (String.IsNullOrEmpty(client.CustomName) || x.BlockCustomNames)
                                    x.SendPacket(TCPOutbound.Public(x, client.Name, text));
                                else
                                    x.SendPacket(TCPOutbound.NoSuch(x, client.CustomName + text));
                            }
                        },
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.IgnoreList.Contains(client.Name) && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(String.IsNullOrEmpty(client.CustomName) ?
                            ib0t.WebOutbound.PublicTo(x, client.Name, text) : ib0t.WebOutbound.NoSuchTo(x, client.CustomName + text)),
                            x => x.LoggedIn && x.Vroom == client.Vroom && !x.IgnoreList.Contains(client.Name) && !x.Quarantined);

                        if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                            ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafPublicText(ServerCore.Linker, client.Name, text));

                        Events.TextSent(client, text);
                    }
                }
            }
        }

        private static void Emote(AresClient client, TCPPacketReader packet)
        {
            if (!client.Captcha)
                return;

            String text = packet.ReadString(client);

            if (text.Length > 300)
                text = text.Substring(0, 300);

            Events.EmoteReceived(client, text);

            if (client.SocketConnected)
            {
                text = Events.EmoteSending(client, text);

                if (!String.IsNullOrEmpty(text) && client.SocketConnected && !client.Muzzled)
                {
                    if (client.Idled)
                    {
                        uint seconds_away = (uint)((Time.Now - client.IdleStart) / 1000);
                        IdleManager.Remove(client);
                        Events.Unidled(client, seconds_away);
                    }

                    if (client.SocketConnected)
                    {
                        if (text.StartsWith("idles"))
                        {
                            if (!IdleManager.CheckIfCanIdle(client))
                            {
                                return;
                            }
                            IdleManager.Add(client);
                            Events.Idled(client);
                        }

                        if (client.SocketConnected)
                        {
                            byte[] js_style = null;
                            AresFont font = (AresFont)client.Font;

                            if (font.Enabled)
                            {
                                font.IsEmote = true;
                                js_style = TCPOutbound.Font(font);
                            }

                            UserPool.AUsers.ForEachWhere(x =>
                            {
                                if (x.SupportsHTML && x.Ares)
                                    if (js_style != null)
                                        x.SendPacket(js_style);

                                x.SendPacket(TCPOutbound.Emote(x, client.Name, text));
                            },
                            x => x.LoggedIn && x.Vroom == client.Vroom && !x.IgnoreList.Contains(client.Name) && !x.Quarantined);

                            UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.EmoteTo(x, client.Name, text)),
                                x => x.LoggedIn && x.Vroom == client.Vroom && !x.IgnoreList.Contains(client.Name) && !x.Quarantined);

                            if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                                ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafEmoteText(ServerCore.Linker, client.Name, text));

                            Events.EmoteSent(client, text);
                        }
                    }
                }
            }
        }
        
        private static void CustomDataAll(AresClient client, TCPPacketReader packet)
        {
            if (client.Quarantined || !client.Captcha)
                return;

            String ident = packet.ReadString(client);
            byte[] data = packet;

            if (ident.Contains("scribble"))
            {
                return;
            }

            if(ident.Contains("cb0t_writing") && client.Muzzled)
            {
                return;
            }

            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.CustomData(x, client.Name, ident, data)),
                x => x.LoggedIn && x.Vroom == client.Vroom && x.CustomClient && !x.Quarantined && x.ID != client.ID && !x.IgnoreList.Contains(client.Name));

            if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafCustomDataAll(ServerCore.Linker, client.Vroom, client.Name, ident, data));
        }

        private static void PersonalMessage(AresClient client, TCPPacketReader packet)
        {
            String text = packet.ReadString(client);

            if (text.Length > 50)
                text = text.Substring(0, 50);

            if (client.PersonalMessage != text)
                if (Events.PersonalMessageReceived(client, text))
                    if (client.SocketConnected)
                        client.PersonalMessage = text;
        }

        private static void Avatar(AresClient client, TCPPacketReader packet)
        {
            byte[] avatar = packet;

            if (!client.Avatar.SequenceEqual(avatar))
                if (Events.AvatarReceived(client))
                    if (avatar.Length < 4064)
                        if (client.SocketConnected)
                        {
                            client.OrgAvatar = avatar;
                            client.Avatar = avatar;
                            client.AvatarReceived = true;
                            client.DefaultAvatar = false;
                        }
        }

        private static void Login(AresClient client, TCPPacketReader packet, ulong time, bool relogin)
        {
            if (client.LoggedIn)
                return;

            client.FastPing = relogin;
            client.Guid = packet;
            client.FileCount = packet;
            ushort org_files = client.FileCount;
            byte crypto = packet;
            client.DataPort = packet;
            client.NodeIP = packet;
            client.NodePort = packet;
            packet.SkipBytes(4);
            client.OrgName = packet.ReadString(client);
            Helpers.FormatUsername(client);
            client.Name = client.OrgName;
            client.Version = packet.ReadString(client);
            client.Ares = client.Version.StartsWith("Ares 2.") || client.Version.StartsWith("Ares_2.");
            client.IsCbot = client.Version.StartsWith("cb0t ");
            client.CustomClient = true; // everyone can be custom client
            client.LocalIP = packet;
            packet.SkipBytes(4);
            client.Browsable = ((byte)packet) > 2 && Settings.Get<bool>("files");
            client.CurrentUploads = packet;
            client.MaxUploads = packet;
            client.CurrentQueued = packet;
            client.Age = packet;
            client.Sex = packet;
            client.Country = packet;
            client.Region = packet.ReadString(client);


            if(client.Region.Length > 30)
            {
                client.Region = client.Region.Substring(0, 30);
            }

            client.FileCount = client.Browsable && client.Version.StartsWith("Ares") ? client.FileCount : (ushort)0;

            if (client.FileCount == 0)
                client.Browsable = false;

            // new proto data
            if (packet.Remaining > 0)
            {
                byte vc = packet;
                client.VoiceChatPublic = ((vc & 1) == 1);
                client.VoiceChatPrivate = ((vc & 2) == 2);
                client.VoiceOpusChatPublic = ((vc & 4) == 4);
                client.VoiceOpusChatPrivate = ((vc & 8) == 8);
                client.SupportsHTML = ((vc & 16) == 16);

                if (client.VoiceOpusChatPublic)
                    client.VoiceChatPublic = true;

                if (client.VoiceOpusChatPrivate)
                    client.VoiceChatPrivate = true;
            }

            // maybe add encryption in next cbot?
            client.Encryption.Mode = crypto == 250 ? EncryptionMode.Encrypted : EncryptionMode.Unencrypted;

            IPAddress p_check = new IPAddress(client.ExternalIP.GetAddressBytes());

            if (client.Version.StartsWith("cb0t"))
                ObSalt.GetSalt(client); // client doesn't support file sharing, so protect their external ip from idiots!

            client.Captcha = !Settings.Get<bool>("captcha");

            if (!client.Captcha)
                client.Captcha = CaptchaManager.HasCaptcha(client);

            if (client.Encryption.Mode == EncryptionMode.Encrypted)
            {
                client.Encryption.Key = Crypto.CreateKey;
                client.Encryption.IV = Crypto.CreateIV;
                client.SendPacket(TCPOutbound.CryptoKey(client));
            }

            if ((UserPool.AUsers.FindAll(x => x.ExternalIP.Equals(client.ExternalIP)).Count +
                 UserPool.WUsers.FindAll(x => x.ExternalIP.Equals(client.ExternalIP)).Count) > 5)
            {
                Events.Rejected(client, RejectedMsg.TooManyClients);
                throw new Exception("too many clients from this ip");
            }

            if (UserHistory.IsJoinFlooding(client, time))
            {
                Events.Rejected(client, RejectedMsg.TooSoon);
                throw new Exception("joined too quickly");
            }

            IClient hijack = UserPool.AUsers.Find(x => (x.Name == client.Name ||
                x.OrgName == client.OrgName) && x.ID != client.ID && x.LoggedIn);

            if (hijack == null)
                hijack = UserPool.WUsers.Find(x => (x.Name == client.Name ||
                    x.OrgName == client.OrgName) && x.ID != client.ID && x.LoggedIn);

            if (hijack != null)
                if (hijack.ExternalIP.Equals(client.ExternalIP))
                {
                    if (!hijack.WebClient)
                        ((AresClient)hijack).Disconnect(true);
                    else
                        ((ib0t.ib0tClient)hijack).Disconnect();

                    client.Name = client.OrgName;
                    client.FastPing = true;
                }
                else
                {
                    Events.Rejected(client, RejectedMsg.NameInUse);
                    throw new Exception("name in use");
                }

            UserHistory.AddUser(client, time);

            if (BanSystem.IsBanned(client))
                if (!Helpers.IsLocalHost(client))
                {
                    if (hijack != null && hijack is AresClient)
                        ((AresClient)hijack).SendDepart();

                    Events.Rejected(client, RejectedMsg.Banned);
                    throw new Exception("banned user");
                }

            if (client.LocalIP.ToString() == "6.6.6.6" ||
                client.LocalIP.ToString() == "7.8.7.8" ||
                org_files == 6969)
            {
                if (hijack != null && hijack is AresClient)
                    ((AresClient)hijack).SendDepart();

                Events.Rejected(client, RejectedMsg.Banned);
                throw new Exception("spam bot");
            }

            if (Settings.Get<bool>("age_restrict"))
                if (client.Age > 0)
                    if ((byte)Settings.Get<int>("age_restrict_value") > client.Age)
                    {
                        if (hijack != null && hijack is AresClient)
                            ((AresClient)hijack).SendDepart();

                        Events.Rejected(client, RejectedMsg.UnderAge);
                        throw new Exception("under aged user");
                    }

            if (Helpers.IsUnacceptableGender(client))
            {
                if (hijack != null && hijack is AresClient)
                    ((AresClient)hijack).SendDepart();

                Events.Rejected(client, RejectedMsg.UnacceptableGender);
                throw new Exception("unacceptable gender");
            }

            if (Proxies.Check(p_check, client.DNS))
                if (!Helpers.IsLocalHost(client))
                    if (Events.ProxyDetected(client))
                    {
                        if (hijack != null && hijack is AresClient)
                            ((AresClient)hijack).SendDepart();

                        Events.Rejected(client, RejectedMsg.Proxy);
                        throw new Exception("proxy detected");
                    }

            client.Quarantined = !client.Captcha && Settings.Get<int>("captcha_mode") == 1;

            if (!Events.Joining(client))
                if (!Helpers.IsLocalHost(client))
                {
                    if (hijack != null && hijack is AresClient)
                        ((AresClient)hijack).SendDepart();

                    Events.Rejected(client, RejectedMsg.UserDefined);
                    throw new Exception("user defined rejection");
                }

            IgnoreManager.LoadIgnores(client);

            if (Helpers.IsLocalHost(client))
            {
                client.Captcha = true;
                client.Quarantined = false;
                client.Registered = true;
                client.Owner = true;
            }

            if (hijack != null)
                if (hijack.Cloaked)
                    hijack = null;

            if (!client.Quarantined)
            {
                if (hijack == null || !(hijack is AresClient))
                {
                    LinkLeaf.LinkUser other = null;

                    if (ServerCore.Linker.Busy)
                        foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                        {
                            other = leaf.Users.Find(x => x.Vroom == client.Vroom && x.Name == client.Name && x.Link.Visible);

                            if (other != null)
                            {
                                other.LinkCredentials.Visible = false;
                                break;
                            }
                        }

                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(other == null ? TCPOutbound.Join(x, client) : TCPOutbound.UpdateUserStatus(x, client)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                    UserPool.WUsers.ForEachWhere(x => x.QueuePacket(other == null ? ib0t.WebOutbound.JoinTo(x, client.Name, client.Level) : ib0t.WebOutbound.UpdateTo(x, client.Name, client.Level)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);
                }

                client.LoggedIn = true;
                client.SendPacket(TCPOutbound.Ack(client));
                client.SendPacket(TCPOutbound.MyFeatures(client));
                client.SendPacket(TCPOutbound.FavIcon());
                client.SendPacket(TCPOutbound.TopicFirst(client, Settings.Topic));
                client.SendPacket(TCPOutbound.UserlistBot(client));

                UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.Userlist(client, x)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined && !x.Cloaked);

                UserPool.WUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.Userlist(client, x)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined && !x.Cloaked);

                if (ServerCore.Linker.Busy)
                    foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                        leaf.Users.ForEachWhere(x => client.SendPacket(TCPOutbound.Userlist(client, x)),
                            x => x.Vroom == client.Vroom && x.Link.Visible && !x.Cloaked);

                client.SendPacket(TCPOutbound.UserListEnd());
                client.SendPacket(TCPOutbound.OpChange(client));
                client.SendPacket(TCPOutbound.SupportsVoiceClips());
                client.SendPacket(TCPOutbound.Url(client, Settings.Get<String>("link", "url"), Settings.Get<String>("text", "url")));
                client.SendPacket(TCPOutbound.PersonalMessageBot(client));
                client.SendPacket(Avatars.Server(client));

                if (client.CustomClient)
                    UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.VoiceChatUserSupport(client, x)),
                        x => (x.VoiceChatPrivate || x.VoiceChatPublic) && !x.Quarantined && !x.Cloaked);

                UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.Avatar(client, x)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && x.Avatar.Length > 0 && !x.Quarantined && !x.Cloaked);

                UserPool.WUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.Avatar(client, x)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined && !x.Cloaked);

                if (ServerCore.Linker.Busy)
                    foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                        leaf.Users.ForEachWhere(x => client.SendPacket(TCPOutbound.Avatar(client, x)),
                            x => x.Vroom == client.Vroom && x.Link.Visible && x.Avatar.Length > 0 && !x.Cloaked);

                UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.PersonalMessage(client, x)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && x.PersonalMessage.Length > 0 && !x.Quarantined && !x.Cloaked);

                UserPool.WUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.PersonalMessage(client, x)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined && !x.Cloaked);

                if (ServerCore.Linker.Busy)
                    foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                        leaf.Users.ForEachWhere(x => client.SendPacket(TCPOutbound.PersonalMessage(client, x)),
                            x => x.Vroom == client.Vroom && x.Link.Visible && x.PersonalMessage.Length > 0 && !x.Cloaked);

                FloodControl.Remove(client);

                if (client.SocketConnected)
                    IdleManager.Set(client);

                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafJoin(ServerCore.Linker, client));

                Events.Joined(client);

                if (client.Owner)
                    client.Level = ILevel.Host;

                if (client.IsCbot)
                    UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.CustomFont(client, x)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && x.Font.Enabled && !x.Cloaked);
            }
            else
            {
                if (hijack != null && hijack is AresClient)
                    ((AresClient)hijack).SendDepart();

                client.LoggedIn = true;
                client.SendPacket(TCPOutbound.Ack(client));
                client.SendPacket(TCPOutbound.MyFeatures(client));
                client.SendPacket(TCPOutbound.FavIcon());
                client.SendPacket(TCPOutbound.TopicFirst(client, Settings.Topic));
                client.SendPacket(TCPOutbound.UserlistBot(client));
                client.SendPacket(TCPOutbound.UserListEnd());
                client.SendPacket(TCPOutbound.PersonalMessageBot(client));
                client.SendPacket(Avatars.Server(client));

                CaptchaItem cap = Captcha.Create();
                client.CaptchaWord = cap.Word;
                Events.CaptchaSending(client);
                client.SendPacket(TCPOutbound.NoSuch(client, String.Empty));

                foreach (String str in cap.Lines)
                    client.SendPacket(TCPOutbound.NoSuch(client, str));

                client.SendPacket(TCPOutbound.NoSuch(client, String.Empty));
                FloodControl.Remove(client);
            }
        }

    }
}
