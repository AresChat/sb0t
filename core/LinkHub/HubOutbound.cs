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

namespace core.LinkHub
{
    class HubOutbound
    {
        public static byte[] LinkError(LinkError code)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteByte((byte)code);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_ERROR);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubAck(Leaf leaf)
        {
            byte[] guid = leaf.Guid.ToByteArray();

            using (MD5 md5 = MD5.Create())
                guid = md5.ComputeHash(guid);

            byte[] key = leaf.IV.Concat(leaf.Key).ToArray();

            for (int i = 0; i < guid.Length; i += 2)
                key = Crypto.e67(key, BitConverter.ToUInt16(guid, i));

            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteBytes(key);
            packet.WriteUInt32(leaf.Ident);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_ACK);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubLeafDisconnected(Leaf x, Leaf leaf)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(leaf.Ident);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_LEAF_DISCONNECTED);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubLeafConnected(Leaf x, Leaf leaf)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(leaf.Ident);
            packet.WriteString(x, leaf.Name);
            packet.WriteIP(leaf.ExternalIP);
            packet.WriteUInt16(leaf.Port);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_LEAF_CONNECTED);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubUserlistItem(Leaf x, uint ident, LinkUser user)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteString(x, user.OrgName);
            packet.WriteString(x, user.Name);
            packet.WriteString(x, user.Version);
            packet.WriteGuid(user.Guid);
            packet.WriteUInt16(user.FileCount);
            packet.WriteIP(user.ExternalIP);
            packet.WriteIP(user.LocalIP);
            packet.WriteUInt16(user.Port);
            packet.WriteString(x, user.DNS);
            packet.WriteByte((byte)(user.Browsable ? 1 : 0));
            packet.WriteByte(user.Age);
            packet.WriteByte(user.Sex);
            packet.WriteByte(user.Country);
            packet.WriteString(x, user.Region);
            packet.WriteByte((byte)user.Level);
            packet.WriteUInt16(user.Vroom);
            packet.WriteByte((byte)(user.CustomClient ? 1 : 0));
            packet.WriteByte((byte)(user.Muzzled ? 1 : 0));
            packet.WriteByte((byte)(user.WebClient ? 1 : 0));
            packet.WriteByte((byte)(user.Encrypted ? 1 : 0));
            packet.WriteByte((byte)(user.Registered ? 1 : 0));
            packet.WriteByte((byte)(user.Idle ? 1 : 0));
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_USERLIST_ITEM);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubUserUpdated(Leaf x, uint ident, LinkUser user)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteString(x, user.Name);
            packet.WriteByte((byte)user.Level);
            packet.WriteByte((byte)(user.Muzzled ? 1 : 0));
            packet.WriteByte((byte)(user.Registered ? 1 : 0));
            packet.WriteByte((byte)(user.Idle ? 1 : 0));
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_HUB_USER_UPDATED);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubNickChanged(Leaf x, uint ident, String old_name, String new_name)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteString(x, old_name);
            packet.WriteString(x, new_name, false);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_NICK_CHANGED);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubVroomChanged(Leaf x, uint ident, LinkUser user)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteString(x, user.Name);
            packet.WriteUInt16(user.Vroom);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_VROOM_CHANGED);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubCustomName(Leaf x, uint ident, LinkUser user)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteString(x, user.Name);
            packet.WriteString(x, user.CustomName, false);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_CUSTOM_NAME);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubAvatar(Leaf x, uint ident, LinkUser user)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteString(x, user.Name);
            packet.WriteBytes(user.Avatar);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_AVATAR);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubPersonalMessage(Leaf x, uint ident, LinkUser user)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteString(x, user.Name);
            packet.WriteString(x, user.PersonalMessage, false);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_PERSONAL_MESSAGE);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubPart(Leaf x, uint ident, LinkUser user)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteString(x, user.Name);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_PART);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubPong()
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_PONG);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubPrivateIgnored(Leaf x, String sender, String target)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, sender);
            packet.WriteString(x, target, false);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_PRIVATE_IGNORED);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubPrivateText(Leaf x, uint ident, String sender, String target, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteString(x, sender);
            packet.WriteString(x, target);
            packet.WriteString(x, text);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_PRIVATE_TEXT);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubPublicText(Leaf x, uint ident, String name, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteString(x, name);
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_PUBLIC_TEXT);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubEmoteText(Leaf x, uint ident, String name, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteString(x, name);
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_EMOTE_TEXT);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubPublicToUser(Leaf x, String target, String sender, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, target);
            packet.WriteString(x, sender);
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_PUBLIC_TO_USER);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubEmoteToUser(Leaf x, String target, String sender, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, target);
            packet.WriteString(x, sender);
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_EMOTE_TO_USER);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubPublicToLeaf(Leaf x, String sender, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, sender);
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_PUBLIC_TO_LEAF);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubEmoteToLeaf(Leaf x, String sender, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, sender);
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_EMOTE_TO_LEAF);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubIUser(Leaf x, String target, String command, String[] args)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, target);
            packet.WriteString(x, command);

            foreach (String str in args)
                packet.WriteString(x, str);

            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_HUB_IUSER);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubIUserBin(Leaf x, String target, String command, byte[] args)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, target);
            packet.WriteString(x, command);
            packet.WriteBytes(args);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_HUB_IUSER_BIN);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubNudge(Leaf x, String target, String sender)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, target);
            packet.WriteString(x, sender, false);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_NUDGE);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubScribbleUser(Leaf x, String target, String sender, uint height, byte[] img)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, target);
            packet.WriteString(x, sender);
            packet.WriteUInt32(height);
            packet.WriteBytes(img);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_SCRIBBLE_USER);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubScribbleLeaf(Leaf x, String sender, uint height, byte[] img)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, sender);
            packet.WriteUInt32(height);
            packet.WriteBytes(img);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_SCRIBBLE_LEAF);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubAdmin(Leaf x, LinkUser admin, String command, LinkUser target, String args)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(admin.Ident);
            packet.WriteString(x, admin.Name);
            packet.WriteString(x, target.Name);
            packet.WriteString(x, command);
            packet.WriteString(x, args);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_HUB_ADMIN);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubNoAdmin(Leaf x, uint ident, String name)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteString(x, name, false);
            byte[] buf = packet.ToLinkPacket(LinkHub.LinkMsg.MSG_LINK_HUB_NO_ADMIN);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubBrowse(Leaf x, uint source, String browsee, String browser, ushort browse_ident, byte mime)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(source);
            packet.WriteString(x, browsee);
            packet.WriteString(x, browser);
            packet.WriteUInt16(browse_ident);
            packet.WriteByte(mime);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_BROWSE);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubBrowseData(Leaf x, String browser, byte[] data)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, browser);
            packet.WriteBytes(data);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_BROWSE_DATA);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubCustomDataTo(Leaf x, String sender, String target, String ident, byte[] data)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, sender);
            packet.WriteString(x, target);
            packet.WriteString(x, ident);
            packet.WriteBytes(data);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_CUSTOM_DATA_TO);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubCustomDataAll(Leaf x, ushort vroom, String sender, String ident, byte[] data)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt16(vroom);
            packet.WriteString(x, sender);
            packet.WriteString(x, ident);
            packet.WriteBytes(data);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_CUSTOM_DATA_ALL);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubPrintAll(Leaf x, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_PRINT_ALL);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubPrintVroom(Leaf x, ushort vroom, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt16(vroom);
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_PRINT_VROOM);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }

        public static byte[] HubPrintLevel(Leaf x, iconnect.ILevel level, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteByte((byte)level);
            packet.WriteString(x, text, false);
            byte[] buf = packet.ToLinkPacket(LinkMsg.MSG_LINK_HUB_PRINT_LEVEL);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_LINK_PROTO);
        }
    }
}
