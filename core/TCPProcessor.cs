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
            Events.PacketReceived(client, packet.Msg, packet.Packet.ToArray());

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

                default:
                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.NoSuch(x, client.ID + " : " + packet.Msg)), x => x.LoggedIn);
                    break;
            }
        }

        private static void PersonalMessage(AresClient client, TCPPacketReader packet)
        {
            String text = packet.ReadString(client);

            if (text.Length > 30)
                text = text.Substring(0, 30);

            if (client.PersonalMessage != text)
                if (Events.PersonalMessageReceived(client, text))
                    client.PersonalMessage = text;
        }

        private static void Avatar(AresClient client, TCPPacketReader packet)
        {
            byte[] avatar = packet;

            if (!client.Avatar.SequenceEqual(avatar))
                if (Events.AvatarReceived(client))
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
            client.LocalIP = packet;
            packet.SkipBytes(4);
            byte flag = packet;
            client.Browsable = flag > 2;
            client.Compression = flag > 3;
            client.CurrentUploads = packet;
            client.MaxUploads = packet;
            client.CurrentQueued = packet;
            client.Age = packet;
            client.Sex = packet;
            client.Country = packet;
            client.Region = packet.ReadString(client);
            client.Encryption = crypto == 250;

            if (client.Encryption)
            {
                client.EncryptionKey = Crypto.CreateKey;
                client.EncryptionIV = Crypto.CreateIV;
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
            client.SendPacket(TCPOutbound.Url(client, Settings.Get<String>("link", "url"), Settings.Get<String>("text", "url")));
            client.SendPacket(TCPOutbound.PersonalMessageBot(client));

            UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.Avatar(client, x)),
                x => x.LoggedIn && x.Vroom == client.Vroom && x.Avatar.Length > 0);

            UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.PersonalMessage(client, x)),
                x => x.LoggedIn && x.Vroom == client.Vroom && x.PersonalMessage.Length > 0);
            
            Events.Joined(client);
        }

    }
}
