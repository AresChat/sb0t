using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography;

namespace core.LinkLeaf
{
    class LeafOutbound
    {
        public static byte[] LeafLogin()
        {
            List<byte> list = new List<byte>();
            list.AddRange(Encoding.UTF8.GetBytes(Settings.Name));
            list.AddRange(Settings.Get<byte[]>("guid"));
            list.Reverse();
            byte[] buf = list.ToArray();

            using (SHA1 sha1 = SHA1.Create())
                buf = sha1.ComputeHash(buf);

            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            packet.WriteUInt16(Settings.LINK_PROTO);
            packet.WriteUInt16(Settings.Port);
            buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_LOGIN);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafPing()
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_PING);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafUserlistItem(LinkClient x, IClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, client.OrgName);
            packet.WriteString(x, client.Name);
            packet.WriteString(x, client.Version);
            packet.WriteGuid(client.Guid);
            packet.WriteUInt16(client.FileCount);
            packet.WriteIP(client.ExternalIP);
            packet.WriteIP(client.LocalIP);
            packet.WriteUInt16(client.DataPort);
            packet.WriteString(x, client.DNS);
            packet.WriteByte((byte)(client.Browsable ? 1 : 0));
            packet.WriteByte(client.Age);
            packet.WriteByte(client.Sex);
            packet.WriteByte(client.Country);
            packet.WriteString(x, client.Region);
            packet.WriteByte((byte)client.Level);
            packet.WriteUInt16(client.Vroom);
            packet.WriteByte((byte)(client.CustomClient ? 1 : 0));
            packet.WriteByte((byte)(client.Muzzled ? 1 : 0));
            packet.WriteByte((byte)(client.WebClient ? 1 : 0));
            packet.WriteByte((byte)(client.Encryption.Mode == EncryptionMode.Encrypted ? 1 : 0));
            packet.WriteByte((byte)(client.Registered ? 1 : 0));
            packet.WriteByte((byte)(client.Idled ? 1 : 0));
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_USERLIST_ITEM);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafJoin(LinkClient x, IClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, client.OrgName);
            packet.WriteString(x, client.Name);
            packet.WriteString(x, client.Version);
            packet.WriteGuid(client.Guid);
            packet.WriteUInt16(client.FileCount);
            packet.WriteIP(client.ExternalIP);
            packet.WriteIP(client.LocalIP);
            packet.WriteUInt16(client.DataPort);
            packet.WriteString(x, client.DNS);
            packet.WriteByte((byte)(client.Browsable ? 1 : 0));
            packet.WriteByte(client.Age);
            packet.WriteByte(client.Sex);
            packet.WriteByte(client.Country);
            packet.WriteString(x, client.Region);
            packet.WriteByte((byte)client.Level);
            packet.WriteUInt16(client.Vroom);
            packet.WriteByte((byte)(client.CustomClient ? 1 : 0));
            packet.WriteByte((byte)(client.Muzzled ? 1 : 0));
            packet.WriteByte((byte)(client.WebClient ? 1 : 0));
            packet.WriteByte((byte)(client.Encryption.Mode == EncryptionMode.Encrypted ? 1 : 0));
            packet.WriteByte((byte)(client.Registered ? 1 : 0));
            packet.WriteByte((byte)(client.Idled ? 1 : 0));
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_JOIN);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafPart(LinkClient x, IClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, client.Name, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_PART);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafAvatar(LinkClient x, IClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, client.Name);
            packet.WriteBytes(client.Avatar);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_AVATAR);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafPersonalMessage(LinkClient x, IClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, client.Name);
            packet.WriteString(x, client.PersonalMessage, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_PERSONAL_MESSAGE);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafCustomName(LinkClient x, IClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, client.Name);
            packet.WriteString(x, client.CustomName, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_CUSTOM_NAME);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafUserlistEnd()
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_USERLIST_END);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafUserUpdated(LinkClient x, IClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, client.Name);
            packet.WriteByte((byte)client.Level);
            packet.WriteByte((byte)(client.Muzzled ? 1 : 0));
            packet.WriteByte((byte)(client.Registered ? 1 : 0));
            packet.WriteByte((byte)(client.Idled ? 1 : 0));
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_USER_UPDATED);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafNameChanged(LinkClient x, String old_name, String new_name)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, old_name);
            packet.WriteString(x, new_name, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_NICK_CHANGED);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafVroomChanged(LinkClient x, IClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, client.Name);
            packet.WriteUInt16(client.Vroom);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_VROOM_CHANGED);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafIUser(LinkClient x, LinkUser target, String command, params String[] args)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(target.Link.Ident);
            packet.WriteString(x, target.Name);
            packet.WriteString(x, command);

            foreach (String str in args)
                packet.WriteString(x, str);

            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_IUSER);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafIUserBin(LinkClient x, LinkUser target, String command, byte[] args)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(target.Link.Ident);
            packet.WriteString(x, target.Name);
            packet.WriteString(x, command);
            packet.WriteBytes(args);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_IUSER_BIN);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

    }
}
