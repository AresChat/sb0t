using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    class TCPProcessor
    {
        public static void Eval(AresClient client, TCPPacket packet, ulong time)
        {
            if (!client.LoggedIn && packet.Msg > TCPMsg.MSG_CHAT_CLIENT_LOGIN)
                throw new Exception("unordered login routine");

            switch (packet.Msg)
            {
                case TCPMsg.MSG_CHAT_CLIENT_LOGIN:
                case TCPMsg.MSG_CHAT_CLIENT_RELOGIN:
                    Login(client, packet.Packet, time, packet.Msg == TCPMsg.MSG_CHAT_CLIENT_RELOGIN);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_UPDATE_STATUS:
                    client.Time = time;
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
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_AUTHREGISTER:
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_AUTOLOGIN:
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_DUMMY:
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_DIRCHATPUSH:
                    DirChatPush(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_SEND_SUPERNODES:
                    client.SendPacket(TCPOutbound.SuperNodes());
                    break;

                default:
                    Events.UnhandledProtocol(client, packet.Msg, packet.Packet, time);
                    break;
            }
        }

        private static void DirChatPush(AresClient client, TCPPacketReader packet)
        {
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
            bool ignore = (byte)packet != 0;
            String name = packet.ReadString(client);
            AresClient target = UserPool.AUsers.Find(x => x.Name == name);

            if (target != null)
                if (!ignore)
                {
                    client.IgnoreList.RemoveAll(x => x == name);
                    Events.IgnoredStateChanged(client, target, ignore);
                }
                else if (Events.Ignoring(client, target))
                    if (client.SocketConnected && target.SocketConnected)
                    {
                        if (!client.IgnoreList.Contains(name))
                            client.IgnoreList.Add(name);

                        Events.IgnoredStateChanged(client, target, ignore);
                    }
        }

        private static void Command(AresClient client, String text)
        {
            Command cmd = new Command { Text = text, Args = String.Empty };
            Helpers.PopulateCommand(cmd);
            Events.Command(client, text, cmd.Target, cmd.Args);
        }

        private static void Private(AresClient client, TCPPacketReader packet)
        {
            String name = packet.ReadString(client);
            String text = packet.ReadString(client);
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
                    Events.BotPrivateSending(client, args);

                    if (!args.Cancel && !String.IsNullOrEmpty(args.Text) && client.SocketConnected)
                    {
                        Events.BotPrivateSent(client, args.Text);

                        if (text.StartsWith("#"))
                            Command(client, text.Substring(1));
                    }
                }
            }
            else
            {
                AresClient target = UserPool.AUsers.Find(x => x.Name == name && x.LoggedIn);

                if (target == null)
                    client.SendPacket(TCPOutbound.OfflineUser(client, name));
                else if (target.IgnoreList.Contains(client.Name) || client.Muzzled)
                    client.SendPacket(TCPOutbound.IsIgnoringYou(client, name));
                else
                {
                    Events.PrivateSending(client, target, args);

                    if (!args.Cancel && !String.IsNullOrEmpty(args.Text) && client.SocketConnected)
                    {
                        target.SendPacket(TCPOutbound.Private(target, client.Name, args.Text));
                        Events.PrivateSent(client, target, args.Text);
                    }
                }
            }
        }

        private static void Public(AresClient client, TCPPacketReader packet)
        {
            String text = packet.ReadString(client);

            if (text.StartsWith("#login") || text.StartsWith("#register"))
            {
                Command(client, text.Substring(1));
                return;
            }

            if (text.StartsWith("#"))
                Command(client, text.Substring(1));

            if (client.SocketConnected)
                Events.TextReceived(client, text);

            if (client.SocketConnected)
            {
                text = Events.TextSending(client, text);

                if (!String.IsNullOrEmpty(text) && client.SocketConnected && !client.Muzzled)
                {
                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(String.IsNullOrEmpty(client.CustomName) ?
                        TCPOutbound.Public(x, client.Name, text) : TCPOutbound.NoSuch(x, client.CustomName + text)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.IgnoreList.Contains(client.Name));

                    Events.TextSent(client, text);
                }
            }
        }

        private static void Emote(AresClient client, TCPPacketReader packet)
        {
            String text = packet.ReadString(client);
            Events.EmoteReceived(client, text);

            if (client.SocketConnected)
            {
                text = Events.EmoteSending(client, text);

                if (!String.IsNullOrEmpty(text) && client.SocketConnected && !client.Muzzled)
                {
                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Emote(x, client.Name, text)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.IgnoreList.Contains(client.Name));

                    Events.EmoteSent(client, text);
                }
            }
        }

        
        private static void CustomDataAll(AresClient client, TCPPacketReader packet)
        {
            String ident = packet.ReadString(client);
            byte[] data = packet;

            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.CustomData(x, client.Name, ident, data)),
                x => x.LoggedIn && x.Vroom == client.Vroom && x.CustomClient);
        }

        private static void CustomData(AresClient client, TCPPacketReader packet)
        {
            String ident = packet.ReadString(client);
            String name = packet.ReadString(client);
            byte[] data = packet;
            AresClient target = UserPool.AUsers.Find(x => x.Name == name && x.LoggedIn && x.CustomClient);

            if (target != null)
                target.SendPacket(TCPOutbound.CustomData(target, client.Name, ident, data));
        }

        private static void PersonalMessage(AresClient client, TCPPacketReader packet)
        {
            String text = packet.ReadString(client);

            if (text.Length > 30)
                text = text.Substring(0, 30);

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
                            client.Avatar = avatar;
        }

        private static void Login(AresClient client, TCPPacketReader packet, ulong time, bool relogin)
        {
            if (client.LoggedIn)
                return;

            client.FastPing = relogin;
            client.Guid = packet;
            client.FileCount = packet;
            byte crypto = packet;
            client.DataPort = packet;
            client.NodeIP = packet;
            client.NodePort = packet;
            packet.SkipBytes(4);
            client.OrgName = packet.ReadString(client);
            Helpers.FormatUsername(client);
            client.Name = client.OrgName;
            client.Version = packet.ReadString(client);
            client.CustomClient = !client.Version.StartsWith("Ares 2.");
            client.LocalIP = packet;
            packet.SkipBytes(4);
            client.Browsable = packet > 2;
            client.CurrentUploads = packet;
            client.MaxUploads = packet;
            client.CurrentQueued = packet;
            client.Age = packet;
            client.Sex = packet;
            client.Country = packet;
            client.Region = packet.ReadString(client);
            client.Encryption.Mode = crypto == 250 ? EncryptionMode.Encrypted : EncryptionMode.Unencrypted;

            if (client.Encryption.Mode == EncryptionMode.Encrypted)
            {
                client.Encryption.Key = Crypto.CreateKey;
                client.Encryption.IV = Crypto.CreateIV;
                client.SendPacket(TCPOutbound.CryptoKey(client));
            }

            if (UserPool.AUsers.FindAll(x => x.ExternalIP.Equals(client.ExternalIP)).Count > 3)
            {
                Events.Rejected(client, RejectedMsg.TooManyClients);
                throw new Exception("too many clients from this ip");
            }

            if (UserHistory.IsJoinFlooding(client, time))
            {
                Events.Rejected(client, RejectedMsg.TooSoon);
                throw new Exception("joined too quickly");
            }

            AresClient hijack = UserPool.AUsers.Find(x => (x.Name == client.Name ||
                x.OrgName == client.OrgName) && x.ID != client.ID);

            if (hijack != null)
                if (hijack.ExternalIP.Equals(client.ExternalIP))
                    hijack.Disconnect(true);
                else
                {
                    Events.Rejected(client, RejectedMsg.NameInUse);
                    throw new Exception("name in use");
                }

            UserHistory.AddUser(client);

            if (BanPool.IsBanned(client))
            {
                if (hijack != null)
                    hijack.SendDepart();

                Events.Rejected(client, RejectedMsg.Banned);
                throw new Exception("banned user");
            }

            if (!Events.Joining(client))
            {
                if (hijack != null)
                    hijack.SendDepart();

                Events.Rejected(client, RejectedMsg.UserDefined);
                throw new Exception("user defined rejection");
            }

            if (hijack == null)
                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Join(x, client)),
                    x => x.LoggedIn && x.Vroom == client.Vroom);

            client.LoggedIn = true;
            client.SendPacket(TCPOutbound.Ack(client));
            client.SendPacket(TCPOutbound.MyFeatures(client));
            client.SendPacket(TCPOutbound.TopicFirst(client, Settings.Get<String>("topic")));
            client.SendPacket(TCPOutbound.UserlistBot(client));

            UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.Userlist(client, x)),
                x => x.LoggedIn && x.Vroom == client.Vroom);

            client.SendPacket(TCPOutbound.UserListEnd());
            client.SendPacket(TCPOutbound.OpChange(client));
            client.SendPacket(TCPOutbound.SupportsVoiceClips());
            client.SendPacket(TCPOutbound.SupportsCustomEmotes());
            client.SendPacket(TCPOutbound.Url(client, Settings.Get<String>("link", "url"), Settings.Get<String>("text", "url")));
            client.SendPacket(TCPOutbound.PersonalMessageBot(client));

            UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.VoiceChatUserSupport(client, x)),
                x => x.VoiceChatPrivate || x.VoiceChatPublic);

            UserPool.AUsers.ForEachWhere(x =>
            {
                foreach (CustomEmoticon c in x.EmoticonList)
                    client.SendPacket(TCPOutbound.CustomEmoteItem(client, x, c));
            },
            x => x.Vroom == client.Vroom && x.CustomEmoticons);

            UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.Avatar(client, x)),
                x => x.LoggedIn && x.Vroom == client.Vroom && x.Avatar.Length > 0);

            UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.PersonalMessage(client, x)),
                x => x.LoggedIn && x.Vroom == client.Vroom && x.PersonalMessage.Length > 0);

            UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.CustomFont(client, x)),
                x => x.LoggedIn && x.Vroom == client.Vroom && x.Font.HasFont);
            
            Events.Joined(client);
        }

    }
}
