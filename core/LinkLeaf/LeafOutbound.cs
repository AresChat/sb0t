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
            packet.WriteIP(Settings.HideIps ? IPAddress.Parse("0.0.0.0") : client.ExternalIP);
            packet.WriteIP(Settings.HideIps ? IPAddress.Parse("0.0.0.0") : client.LocalIP);
            packet.WriteUInt16(Settings.HideIps ? (ushort)0 : client.DataPort);
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
            packet.WriteIP(Settings.HideIps ? IPAddress.Parse("0.0.0.0") : client.ExternalIP);
            packet.WriteIP(Settings.HideIps ? IPAddress.Parse("0.0.0.0") : client.LocalIP);
            packet.WriteUInt16(Settings.HideIps ? (ushort)0 : client.DataPort);
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

        public static byte[] LeafNudge(LinkClient x, LinkUser target, String sender)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(target.LinkCredentials.Ident);
            packet.WriteString(x, target.Name);
            packet.WriteString(x, sender, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_NUDGE);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafScribbleUser(LinkClient x, LinkUser target, String sender, int height, byte[] img)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(target.LinkCredentials.Ident);
            packet.WriteString(x, target.Name);
            packet.WriteString(x, sender);
            packet.WriteUInt32((uint)height);
            packet.WriteBytes(img);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_SCRIBBLE_USER);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafScribbleLeaf(LinkClient x, uint target_leaf, String sender, int height, byte[] img)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(target_leaf);
            packet.WriteString(x, sender);
            packet.WriteUInt32((uint)height);
            packet.WriteBytes(img);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_SCRIBBLE_LEAF);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafAdmin(LinkClient x, IClient admin, String command, IClient target, String args)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(target.IUser.Link.Ident);
            packet.WriteString(x, admin.Name);
            packet.WriteString(x, target.Name);
            packet.WriteString(x, command);
            packet.WriteString(x, args, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_ADMIN);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafPublicText(LinkClient x, String name, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, name);
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_PUBLIC_TEXT);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafEmoteText(LinkClient x, String name, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, name);
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_EMOTE_TEXT);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafPublicTextToUser(LinkClient x, uint target_ident, String target, String name, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(target_ident);
            packet.WriteString(x, target);
            packet.WriteString(x, name);
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_PUBLIC_TO_USER);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafEmoteTextToUser(LinkClient x, uint target_ident, String target, String name, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(target_ident);
            packet.WriteString(x, target);
            packet.WriteString(x, name);
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_EMOTE_TO_USER);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafPublicTextToLeaf(LinkClient x, uint target_ident, String name, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(target_ident);
            packet.WriteString(x, name);
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_PUBLIC_TO_LEAF);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafEmoteTextToLeaf(LinkClient x, uint target_ident, String name, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(target_ident);
            packet.WriteString(x, name);
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_EMOTE_TO_LEAF);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafPrivateText(LinkClient x, String sender, IClient target, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(target.IUser.Link.Ident);
            packet.WriteString(x, target.Name);
            packet.WriteString(x, sender);
            packet.WriteString(x, text);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_PRIVATE_TEXT);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafPrivateIgnored(LinkClient x, uint sender_ident, String sender, String target)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(sender_ident);
            packet.WriteString(x, sender);
            packet.WriteString(x, target, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_PRIVATE_IGNORED);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafNoAdmin(LinkClient x, uint ident, String name)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteString(x, name, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_NO_ADMIN);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafBrowse(LinkClient x, uint leaf_ident, String browsee, String browser, ushort browse_ident, byte mime)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(leaf_ident);
            packet.WriteString(x, browsee);
            packet.WriteString(x, browser);
            packet.WriteUInt16(browse_ident);
            packet.WriteByte(mime);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_BROWSE);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafBrowseData(LinkClient x, uint destination, String browser, byte[] data)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(destination);
            packet.WriteString(x, browser);
            packet.WriteBytes(data);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_BROWSE_DATA);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafCustomDataTo(LinkClient x, uint destination, String sender, String target, String ident, byte[] data)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(destination);
            packet.WriteString(x, sender);
            packet.WriteString(x, target);
            packet.WriteString(x, ident);
            packet.WriteBytes(data);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_CUSTOM_DATA_TO);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafCustomDataAll(LinkClient x, ushort vroom, String sender, String ident, byte[] data)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt16(vroom);
            packet.WriteString(x, sender);
            packet.WriteString(x, ident);
            packet.WriteBytes(data);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_CUSTOM_DATA_ALL);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafPrintAll(LinkClient x, uint ident, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_PRINT_ALL);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafPrintVroom(LinkClient x, uint ident, ushort vroom, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteUInt16(vroom);
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_PRINT_VROOM);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] LeafPrintLevel(LinkClient x, uint ident, iconnect.ILevel level, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteByte((byte)level);
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_LEAF_PRINT_LEVEL);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }
    }
}
