using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    class TCPOutbound
    {
        public static byte[] Ack(AresClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, client.Name);
            packet.WriteString(client, Settings.Get<String>("name"));
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_LOGIN_ACK);
        }

        public static byte[] NoSuch(AresClient client, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, text, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_NOSUCH);
        }

        public static byte[] Join(AresClient client, AresClient target)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt16(target.FileCount);
            packet.WriteUInt32(0);
            packet.WriteIP(target.ExternalIP);
            packet.WriteUInt16(target.DataPort);
            packet.WriteIP(target.NodeIP);
            packet.WriteUInt16(target.NodePort);
            packet.WriteByte(0);
            packet.WriteString(client, target.Name);
            packet.WriteIP(target.LocalIP);
            packet.WriteByte(target.Browsable ? (byte)1 : (byte)0);
            packet.WriteByte((byte)target.Level);
            packet.WriteByte(target.Age);
            packet.WriteByte(target.Sex);
            packet.WriteByte(target.Country);
            packet.WriteString(client, target.Region);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_JOIN);
        }

        public static byte[] Part(AresClient client, AresClient target)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, target.Name);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_PART);
        }

        public static byte[] Userlist(AresClient client, AresClient target)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt16(target.FileCount);
            packet.WriteUInt32(0);
            packet.WriteIP(target.ExternalIP);
            packet.WriteUInt16(target.DataPort);
            packet.WriteIP(target.NodeIP);
            packet.WriteUInt16(target.NodePort);
            packet.WriteByte(0);
            packet.WriteString(client, target.Name);
            packet.WriteIP(target.LocalIP);
            packet.WriteByte(target.Browsable ? (byte)1 : (byte)0);
            packet.WriteByte((byte)target.Level);
            packet.WriteByte(target.Age);
            packet.WriteByte(target.Sex);
            packet.WriteByte(target.Country);
            packet.WriteString(client, target.Region);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_CHANNEL_USER_LIST);
        }

        public static byte[] UserlistBot(AresClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt16(0);
            packet.WriteUInt32(0);
            packet.WriteIP("0.0.0.0");
            packet.WriteUInt16(0);
            packet.WriteIP("0.0.0.0");
            packet.WriteUInt16(0);
            packet.WriteByte(0);
            packet.WriteString(client, Settings.Get<String>("bot"));
            packet.WriteIP("0.0.0.0");
            packet.WriteByte(1);
            packet.WriteByte(3);
            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteString(client, String.Empty);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_CHANNEL_USER_LIST);
        }

        public static byte[] UserListEnd()
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteByte(0);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_CHANNEL_USER_LIST_END);
        }

        public static byte[] TopicFirst(AresClient client, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, text, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_TOPIC_FIRST);
        }

        public static byte[] OpChange(AresClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteByte(client.Level > 0 ? (byte)1 : (byte)0);
            packet.WriteByte(0);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_OPCHANGE);
        }

        public static byte[] MyFeatures(AresClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, Settings.VERSION);
            packet.WriteByte((byte)(client.Browsable ? 7 : 3));
            packet.WriteByte(63);
            packet.WriteByte(Settings.Get<byte>("language"));
            packet.WriteUInt32(client.Cookie);
            packet.WriteByte(1);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_MYFEATURES);
        }

        public static byte[] Url(AresClient client, String addr, String tag)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, addr);
            packet.WriteString(client, tag);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_URL);
        }

        public static byte[] AvatarCleared(AresClient client, AresClient target)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, target.Name);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_AVATAR);
        }

        public static byte[] Avatar(AresClient client, AresClient target)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, target.Name);
            packet.WriteBytes(target.Avatar);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_AVATAR);
        }

        public static byte[] PersonalMessage(AresClient client, AresClient target)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, target.Name);
            packet.WriteString(client, target.PersonalMessage, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_PERSONAL_MESSAGE);
        }

        public static byte[] PersonalMessageBot(AresClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, Settings.Get<String>("bot"));
            packet.WriteString(client, Settings.VERSION, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_PERSONAL_MESSAGE);
        }

        public static byte[] FastPing()
        {
            return new TCPPacketWriter().ToAresPacket(TCPMsg.MSG_CHAT_SERVER_FASTPING);
        }

        public static byte[] CryptoKey(AresClient client)
        {
            byte[] guid = client.Guid.ToByteArray();
            byte[] key = client.Encryption.IV.Concat(client.Encryption.Key).ToArray();

            for (int i = 0; i < guid.Length; i += 2)
                key = Crypto.e67(key, BitConverter.ToUInt16(guid, i));

            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteBytes(key);
            byte[] data = packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_CRYPTO_KEY);
            packet = new TCPPacketWriter();
            packet.WriteBytes(data);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }
    }
}
