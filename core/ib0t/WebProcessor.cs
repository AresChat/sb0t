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
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using captcha;
using iconnect;

namespace core.ib0t
{
    class WebProcessor
    {
        public static void Evaluate(ib0tClient client, String ident, String args, ulong time)
        {
            if (!client.LoggedIn && !ident.StartsWith("LOGIN"))
                throw new Exception("unordered login routine");

            if (client.LoggedIn && ident.StartsWith("LOGIN"))
                return;

            if (client.LoggedIn)
                if (FloodControl.IsFlooding(client, ident, Encoding.UTF8.GetBytes(args), time))
                    if (Events.Flooding(client, (byte)FloodControl.WebMsgToTCPMsg(ident)))
                    {
                        Events.Flooded(client);
                        client.Disconnect();
                        return;
                    }

            switch (ident)
            {
                case "LOGIN":
                    Login(client, args, time);
                    break;

                case "PUBLIC":
                    Text(client, args, time);
                    break;

                case "EMOTE":
                    Emote(client, args, time);
                    break;

                case "COMMAND":
                    Command(client, args);
                    break;

                case "PING":
                    client.Time = time;
                    break;

                case "PM":
                    PM(client, args, time);
                    break;

                case "IGNORE":
                    Ignore(client, args);
                    break;

                case "LAG":
                    Lag(client, args);
                    break;

                default:
                    throw new Exception();
            }
        }

        private static void Lag(ib0tClient client, String args)
        {
            client.QueuePacket(WebOutbound.LagTo(client, args));
        }

        private static void Ignore(ib0tClient client, String args)
        {
            String tmp = args;
            int finder = tmp.IndexOf(":");
            String[] lens = tmp.Substring(0, finder).Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            tmp = tmp.Substring(finder + 1);
            int n_len = int.Parse(lens[0]);

            String name = tmp.Substring(0, n_len);
            bool ignore = int.Parse(tmp.Substring(tmp.Length - 1)) == 1;

            if (client.Quarantined)
                return;

            IClient target = UserPool.AUsers.Find(x => x.Name == name);

            if (target == null)
                target = UserPool.WUsers.Find(x => x.Name == name);

            if (target == null && ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                target = ServerCore.Linker.FindUser(x => x.Name == name);

            if (target != null)
                if (!ignore)
                {
                    client.IgnoreList.RemoveAll(x => x == name);
                    Events.IgnoredStateChanged(client, target, ignore);
                }
                else if (Events.Ignoring(client, target))
                    if (client.SocketConnected)
                    {
                        if (!client.IgnoreList.Contains(name))
                            client.IgnoreList.Add(name);

                        Events.IgnoredStateChanged(client, target, ignore);
                    }
        }

        private static void PM(ib0tClient client, String ags, ulong time)
        {
            String tmp = ags;
            int finder = tmp.IndexOf(":");
            String[] lens = tmp.Substring(0, finder).Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            tmp = tmp.Substring(finder + 1);
            int n_len = int.Parse(lens[0]);
            int t_len = int.Parse(lens[1]);

            String name = tmp.Substring(0, n_len);
            String text = tmp.Substring(n_len, t_len);

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
                    client.QueuePacket(WebOutbound.OfflineTo(client, name));
                else if (target.IgnoreList.Contains(client.Name) || client.Muzzled)
                    client.QueuePacket(WebOutbound.IgnoringTo(client, name));
                else
                {
                    if (target.Cloaked)
                    {
                        client.QueuePacket(WebOutbound.OfflineTo(client, name));
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

        private static String[] GetArgItems(String args)
        {
            List<String> items = new List<String>();
            int finder = args.IndexOf(":");

            if (finder > -1)
            {
                String[] lens = args.Substring(0, finder).Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                String data = args.Substring(finder + 1);

                for (int i = 0; i < lens.Length; i++)
                {
                    int l = int.Parse(lens[i]);
                    String s = data.Substring(0, l);
                    items.Add(s);
                    data = data.Substring(l);
                }
            }

            return items.ToArray();
        }

        private static void Login(ib0tClient client, String args, ulong time)
        {
            String[] arg_items = GetArgItems(args);

            client.Extended = int.Parse(arg_items[0]) >= 2000;

            byte[] g = new byte[16];

            for (int i = 0; i < g.Length; i++)
                g[i] = byte.Parse(arg_items[1].Substring((i * 2), 2), NumberStyles.HexNumber);

            using (MD5 md5 = MD5.Create())
                client.Guid = new Guid(md5.ComputeHash(g));

            client.OrgName = arg_items[2].Trim();
            Helpers.FormatUsername(client);
            client.Name = client.OrgName;
            client.FastPing = false;
            client.FileCount = 0;
            client.DataPort = 0;
            client.NodeIP = IPAddress.Parse("0.0.0.0");
            client.NodePort = 0;
            client.Version = arg_items[4] + " [" + arg_items[3] + "]";
            client._pmsg = arg_items[4];
            client.CustomClient = true;
            client.LocalIP = client.ExternalIP;
            client.Browsable = false;
            client.Age = 0;
            client.Sex = 0;
            client.Country = 0;
            client.Region = String.Empty;
            IPAddress p_check = new IPAddress(client.ExternalIP.GetAddressBytes());
            ObSalt.GetSalt(client);
            client.Captcha = !Settings.Get<bool>("captcha");

            if (!client.Captcha)
                client.Captcha = CaptchaManager.HasCaptcha(client);

            if ((UserPool.AUsers.FindAll(x => x.ExternalIP.Equals(client.ExternalIP)).Count +
                 UserPool.WUsers.FindAll(x => x.ExternalIP.Equals(client.ExternalIP)).Count) > 3)
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

            if (Helpers.IsLocalHost(client))
            {
                client.Captcha = true;
                client.Quarantined = false;
                client.Registered = true;
                client.Owner = true;
            }

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
                client.QueuePacket(WebOutbound.AckTo(client, client.Name));
                client.QueuePacket(WebOutbound.TopicFirstTo(client, Settings.Get<String>("topic")));
                client.QueuePacket(WebOutbound.UserlistItemTo(client, Settings.Get<String>("bot"), ILevel.Host));

                UserPool.AUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.UserlistItemTo(client, x.Name, x.Level)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                UserPool.WUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.UserlistItemTo(client, x.Name, x.Level)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                if (ServerCore.Linker.Busy)
                    foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                        leaf.Users.ForEachWhere(x => client.QueuePacket(WebOutbound.UserlistItemTo(client, x.Name, x.Level)),
                            x => x.Vroom == client.Vroom && x.Link.Visible);

                client.QueuePacket(WebOutbound.UserlistEndTo(client));
                client.QueuePacket(WebOutbound.UrlTo(client, Settings.Get<String>("link", "url"), Settings.Get<String>("text", "url")));

                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Avatar(x, client)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                UserPool.WUsers.ForEachWhere(x => x.QueuePacket(WebOutbound.AvatarTo(x, client.Name, client.Avatar)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined && x.Extended);

                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.PersonalMessage(x, client)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                UserPool.WUsers.ForEachWhere(x => x.QueuePacket(WebOutbound.PersMsgTo(x, client.Name, client.PersonalMessage)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined && x.Extended);

                if (client.Extended)
                {
                    client.QueuePacket(WebOutbound.PerMsgBotTo(client));

                    if (Avatars.GotServerAvatar)
                        client.QueuePacket(Avatars.Server(client));

                    UserPool.AUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.AvatarTo(client, x.Name, x.Avatar)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && x.Avatar.Length > 0 && !x.Quarantined);

                    UserPool.WUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.AvatarTo(client, x.Name, x.Avatar)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                    if (ServerCore.Linker.Busy)
                        foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                            leaf.Users.ForEachWhere(x => client.QueuePacket(WebOutbound.AvatarTo(client, x.Name, x.Avatar)),
                                x => x.Vroom == client.Vroom && x.Link.Visible && x.Avatar.Length > 0);

                    UserPool.AUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.PersMsgTo(client, x.Name, x.PersonalMessage)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && x.PersonalMessage.Length > 0 && !x.Quarantined);

                    UserPool.WUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.PersMsgTo(client, x.Name, x.PersonalMessage)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                    UserPool.AUsers.ForEachWhere(x =>
                    {
                        AresFont f = (AresFont)x.Font;
                        client.QueuePacket(WebOutbound.FontTo(client, x.Name, f.oldN, f.oldT));
                    }, x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined && x.Font.Enabled);

                    if (ServerCore.Linker.Busy)
                        foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                            leaf.Users.ForEachWhere(x => client.QueuePacket(WebOutbound.PersMsgTo(client, x.Name, x.PersonalMessage)),
                                x => x.Vroom == client.Vroom && x.Link.Visible && x.PersonalMessage.Length > 0);
                }

                FloodControl.Remove(client);

                if (client.SocketConnected)
                    IdleManager.Set(client);

                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafJoin(ServerCore.Linker, client));

                Events.Joined(client);

                if (client.Owner)
                    client.Level = ILevel.Host;
            }
            else
            {
                if (hijack != null && hijack is AresClient)
                    ((AresClient)hijack).SendDepart();

                client.LoggedIn = true;
                client.QueuePacket(WebOutbound.AckTo(client, client.Name));
                client.QueuePacket(WebOutbound.TopicFirstTo(client, Settings.Get<String>("topic")));
                client.QueuePacket(WebOutbound.UserlistEndTo(client));
                client.QueuePacket(WebOutbound.PerMsgBotTo(client));
                client.QueuePacket(Avatars.Server(client));

                CaptchaItem cap = Captcha.Create();
                client.CaptchaWord = cap.Word;
                Events.CaptchaSending(client);
                client.QueuePacket(WebOutbound.NoSuchTo(client, String.Empty));

                foreach (String str in cap.Lines)
                    client.QueuePacket(WebOutbound.NoSuchTo(client, str));

                client.QueuePacket(WebOutbound.NoSuchTo(client, String.Empty));
            }
        }

        private static void Text(ib0tClient client, String args, ulong time)
        {
            String text = args;

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
                        client.QueuePacket(WebOutbound.NoSuchTo(client, String.Empty));

                        foreach (String str in cap.Lines)
                            client.QueuePacket(WebOutbound.NoSuchTo(client, str));

                        client.QueuePacket(WebOutbound.NoSuchTo(client, String.Empty));
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

                        UserPool.AUsers.ForEachWhere(x =>
                        {
                            if (x.SupportsHTML)
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
                        }, x => x.LoggedIn && x.Vroom == client.Vroom && !x.IgnoreList.Contains(client.Name) && !x.Quarantined);

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

        private static void Emote(ib0tClient client, String args, ulong time)
        {
            if (!client.Captcha)
                return;

            String text = args;

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

                            UserPool.AUsers.ForEachWhere(x =>
                            {
                                if (x.SupportsHTML)
                                    if (js_style != null)
                                        x.SendPacket(js_style);

                                x.SendPacket(TCPOutbound.Emote(x, client.Name, text));
                            }, x => x.LoggedIn && x.Vroom == client.Vroom && !x.IgnoreList.Contains(client.Name) && !x.Quarantined);

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

        private static void Command(ib0tClient client, String args)
        {
            Command cmd = new Command { Text = args, Args = String.Empty };
            Helpers.PopulateCommand(cmd);
            Events.Command(client, args, cmd.Target, cmd.Args);
        }
    }
}
