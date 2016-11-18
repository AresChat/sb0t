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

namespace core.LinkHub
{
    class HubProcessor
    {
        public static void Eval(Leaf leaf, LinkMsg msg, TCPPacketReader packet, ulong time, LinkMode mode)
        {
            switch (msg)
            {
                case LinkMsg.MSG_LINK_LEAF_AVATAR:
                    LeafAvatar(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_CUSTOM_NAME:
                    LeafCustomName(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_EMOTE_TEXT:
                    LeafEmoteText(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_LOGIN:
                    LeafLogin(leaf, packet, time, mode);
                    break;

                case LinkMsg.MSG_LINK_LEAF_PERSONAL_MESSAGE:
                    LeafPersonalMessage(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_PRIVATE_IGNORED:
                    LeafPrivateIgnored(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_PRIVATE_TEXT:
                    LeafPrivateText(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_PUBLIC_TEXT:
                    LeafPublicText(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_USERLIST_END:
                    LeafUserlistEnd(leaf);
                    break;

                case LinkMsg.MSG_LINK_LEAF_USERLIST_ITEM:
                    LeafUserlistItem(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_JOIN:
                    LeafJoin(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_PART:
                    LeafPart(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_PING:
                    LeafPing(leaf, time);
                    break;

                case LinkMsg.MSG_LINK_LEAF_USER_UPDATED:
                    LeafUserUpdated(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_NICK_CHANGED:
                    LeafNickChanged(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_VROOM_CHANGED:
                    LeafVroomChanged(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_IUSER:
                    LeafIUser(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_IUSER_BIN:
                    LeafIUserBin(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_ADMIN:
                    LeafAdmin(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_NO_ADMIN:
                    LeafNoAdmin(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_BROWSE:
                    LeafBrowse(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_BROWSE_DATA:
                    LeafBrowseData(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_CUSTOM_DATA_TO:
                    LeafCustomDataTo(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_CUSTOM_DATA_ALL:
                    LeafCustomDataAll(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_PRINT_ALL:
                    LeafPrintAll(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_PRINT_VROOM:
                    LeafPrintVroom(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_PRINT_LEVEL:
                    LeafPrintLevel(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_PUBLIC_TO_USER:
                    LeafPublicToUser(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_EMOTE_TO_USER:
                    LeafEmoteToUser(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_PUBLIC_TO_LEAF:
                    LeafPublicToLeaf(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_EMOTE_TO_LEAF:
                    LeafEmoteToLeaf(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_NUDGE:
                    LeafNudge(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_SCRIBBLE_USER:
                    LeafScribbleUser(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_SCRIBBLE_LEAF:
                    LeafScribbleLeaf(leaf, packet);
                    break;
            }
        }

        private static void LeafScribbleUser(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint leaf_ident = packet;
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == leaf_ident && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
            {
                String target = packet.ReadString(leaf);
                String sender = packet.ReadString(leaf);
                uint height = packet;
                byte[] img = packet;
                l.SendPacket(HubOutbound.HubScribbleUser(l, target, sender, height, img));
            }
        }

        private static void LeafScribbleLeaf(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint leaf_ident = packet;
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == leaf_ident && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
            {
                String sender = packet.ReadString(leaf);
                uint height = packet;
                byte[] img = packet;
                l.SendPacket(HubOutbound.HubScribbleLeaf(l, sender, height, img));
            }
        }

        private static void LeafNudge(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint leaf_ident = packet;
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == leaf_ident && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
            {
                String target = packet.ReadString(leaf);
                String sender = packet.ReadString(leaf);
                l.SendPacket(HubOutbound.HubNudge(l, target, sender));
            }
        }

        private static void LeafPublicToLeaf(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint leaf_ident = packet;
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == leaf_ident && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
            {
                String sender = packet.ReadString(leaf);
                String text = packet.ReadString(leaf);
                l.SendPacket(HubOutbound.HubPublicToLeaf(l, sender, text));
            }
        }

        private static void LeafEmoteToLeaf(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint leaf_ident = packet;
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == leaf_ident && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
            {
                String sender = packet.ReadString(leaf);
                String text = packet.ReadString(leaf);
                l.SendPacket(HubOutbound.HubEmoteToLeaf(l, sender, text));
            }
        }

        private static void LeafPublicToUser(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint leaf_ident = packet;
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == leaf_ident && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
            {
                String target = packet.ReadString(leaf);
                String sender = packet.ReadString(leaf);
                String text = packet.ReadString(leaf);
                l.SendPacket(HubOutbound.HubPublicToUser(l, target, sender, text));
            }
        }

        private static void LeafEmoteToUser(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint leaf_ident = packet;
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == leaf_ident && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
            {
                String target = packet.ReadString(leaf);
                String sender = packet.ReadString(leaf);
                String text = packet.ReadString(leaf);
                leaf.SendPacket(HubOutbound.HubEmoteToUser(leaf, target, sender, text));
            }
        }

        private static void LeafPrintAll(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint leaf_ident = packet;
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == leaf_ident && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
            {
                String text = packet.ReadString(leaf);
                l.SendPacket(HubOutbound.HubPrintAll(l, text));
            }
        }

        private static void LeafPrintVroom(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint leaf_ident = packet;
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == leaf_ident && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
            {
                ushort vroom = packet;
                String text = packet.ReadString(leaf);
                l.SendPacket(HubOutbound.HubPrintVroom(l, vroom, text));
            }
        }

        private static void LeafPrintLevel(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint leaf_ident = packet;
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == leaf_ident && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
            {
                iconnect.ILevel level = (iconnect.ILevel)((byte)packet);
                String text = packet.ReadString(leaf);
                l.SendPacket(HubOutbound.HubPrintLevel(l, level, text));
            }
        }

        private static void LeafCustomDataAll(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            ushort vroom = packet;
            String sender = packet.ReadString(leaf);
            String ident = packet.ReadString(leaf);
            byte[] data = packet;

            LeafPool.Leaves.ForEachWhere(x => x.SendPacket(HubOutbound.HubCustomDataAll(x, vroom, sender, ident, data)),
                x => x.Ident != leaf.Ident && x.LoginPhase == LinkLogin.Ready);
        }

        private static void LeafCustomDataTo(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint leaf_ident = packet;
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == leaf_ident && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
            {
                String sender = packet.ReadString(leaf);
                String target = packet.ReadString(leaf);
                String ident = packet.ReadString(leaf);
                byte[] data = packet;
                l.SendPacket(HubOutbound.HubCustomDataTo(l, sender, target, ident, data));
            }
        }

        private static void LeafBrowseData(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint leaf_ident = packet;
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == leaf_ident && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
            {
                String browser = packet.ReadString(leaf);
                byte[] data = packet;
                l.SendPacket(HubOutbound.HubBrowseData(l, browser, data));
            }
        }

        private static void LeafBrowse(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint leaf_ident = packet;
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == leaf_ident && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
            {
                String browsee = packet.ReadString(leaf);
                String browser = packet.ReadString(leaf);
                ushort browse_ident = packet;
                byte mime = packet;
                l.SendPacket(HubOutbound.HubBrowse(l, leaf.Ident, browsee, browser, browse_ident, mime));
            }
        }

        private static void LeafNoAdmin(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint ident = packet;
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == ident && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
            {
                String name = packet.ReadString(leaf);
                l.SendPacket(HubOutbound.HubNoAdmin(l, leaf.Ident, name));
            }
        }

        private static void LeafAdmin(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint target_ident = packet;
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == target_ident && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
            {
                String sender_name = packet.ReadString(leaf);
                LinkUser admin = leaf.Users.Find(x => x.Name == sender_name);
                String target_name = packet.ReadString(leaf);
                LinkUser target = l.Users.Find(x => x.Name == target_name);

                if (admin != null && target_name != null)
                {
                    String command = packet.ReadString(leaf);
                    String args = packet.ReadString(leaf);
                    l.SendPacket(HubOutbound.HubAdmin(l, admin, command, target, args));
                }
            }
        }

        private static void LeafIUser(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint target_leaf = packet;
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == target_leaf && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
            {
                String name = packet.ReadString(leaf);
                String command = packet.ReadString(leaf);
                List<String> args = new List<String>();

                while (packet.Remaining > 0)
                    args.Add(packet.ReadString(leaf));

                l.SendPacket(HubOutbound.HubIUser(l, name, command, args.ToArray()));
            }
        }

        private static void LeafIUserBin(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint target_leaf = packet;
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == target_leaf && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
            {
                String name = packet.ReadString(leaf);
                String command = packet.ReadString(leaf);
                byte[] args = packet;
                l.SendPacket(HubOutbound.HubIUserBin(l, name, command, args));
            }
        }

        private static void LeafNickChanged(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            String name = packet.ReadString(leaf);
            LinkUser user = leaf.Users.Find(x => x.Name == name);

            if (user != null)
            {
                user.Name = packet.ReadString(leaf);

                LeafPool.Leaves.ForEachWhere(x => x.SendPacket(HubOutbound.HubNickChanged(x, leaf.Ident, name, user.Name)),
                    x => x.Ident != leaf.Ident && x.LoginPhase == LinkLogin.Ready);
            }
        }

        private static void LeafVroomChanged(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            String name = packet.ReadString(leaf);
            LinkUser user = leaf.Users.Find(x => x.Name == name);

            if (user != null)
            {
                user.Vroom = packet;

                LeafPool.Leaves.ForEachWhere(x => x.SendPacket(HubOutbound.HubVroomChanged(x, leaf.Ident, user)),
                    x => x.Ident != leaf.Ident && x.LoginPhase == LinkLogin.Ready);
            }
        }

        private static void LeafUserUpdated(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            String name = packet.ReadString(leaf);
            LinkUser user = leaf.Users.Find(x => x.Name == name);

            if (user != null)
            {
                user.Level = (iconnect.ILevel)((byte)packet);
                user.Muzzled = ((byte)packet) == 1;
                user.Registered = ((byte)packet) == 1;
                user.Idle = ((byte)packet) == 1;

                LeafPool.Leaves.ForEachWhere(x => x.SendPacket(HubOutbound.HubUserUpdated(x, leaf.Ident, user)),
                    x => x.Ident != leaf.Ident && x.LoginPhase == LinkLogin.Ready);
            }
        }

        private static void LeafEmoteText(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            String name = packet.ReadString(leaf);
            String text = packet.ReadString(leaf);

            LeafPool.Leaves.ForEachWhere(x => x.SendPacket(HubOutbound.HubEmoteText(x, leaf.Ident, name, text)),
                x => x.Ident != leaf.Ident && x.LoginPhase == LinkLogin.Ready);
        }

        private static void LeafPublicText(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            String name = packet.ReadString(leaf);
            String text = packet.ReadString(leaf);

            LeafPool.Leaves.ForEachWhere(x => x.SendPacket(HubOutbound.HubPublicText(x, leaf.Ident, name, text)),
                x => x.Ident != leaf.Ident && x.LoginPhase == LinkLogin.Ready);
        }

        private static void LeafPrivateText(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint target_leaf = packet;
            String target_name = packet.ReadString(leaf);
            String sender_name = packet.ReadString(leaf);
            String text = packet.ReadString(leaf);
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == target_leaf && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
                l.SendPacket(HubOutbound.HubPrivateText(l, leaf.Ident, sender_name, target_name, text));
        }

        private static void LeafPrivateIgnored(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            uint sender_leaf = packet;
            String sender = packet.ReadString(leaf);
            String target = packet.ReadString(leaf);
            Leaf l = LeafPool.Leaves.Find(x => x.Ident == sender_leaf && x.LoginPhase == LinkLogin.Ready);

            if (l != null)
                l.SendPacket(HubOutbound.HubPrivateIgnored(l, sender, target));
        }

        private static void LeafJoin(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            LinkUser user = new LinkUser(leaf.Ident);
            user.OrgName = packet.ReadString(leaf);
            user.Name = packet.ReadString(leaf);
            user.Version = packet.ReadString(leaf);
            user.Guid = packet;
            user.FileCount = packet;
            user.ExternalIP = packet;
            user.LocalIP = packet;
            user.Port = packet;
            user.DNS = packet.ReadString(leaf);
            user.Browsable = ((byte)packet) == 1;
            user.Age = packet;
            user.Sex = packet;
            user.Country = packet;
            user.Region = packet.ReadString(leaf);
            user.Level = (iconnect.ILevel)((byte)packet);
            user.Vroom = packet;
            user.CustomClient = ((byte)packet) == 1;
            user.Muzzled = ((byte)packet) == 1;
            user.WebClient = ((byte)packet) == 1;
            user.Encrypted = ((byte)packet) == 1;
            user.Registered = ((byte)packet) == 1;
            user.Idle = ((byte)packet) == 1;
            user.PersonalMessage = String.Empty;
            user.CustomName = String.Empty;
            user.Avatar = new byte[] { };
            leaf.Users.Add(user);

            LeafPool.Leaves.ForEachWhere(x => x.SendPacket(HubOutbound.HubUserlistItem(x, leaf.Ident, user)),
                x => x.Ident != leaf.Ident && x.LoginPhase == LinkLogin.Ready);
        }

        private static void LeafPart(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            String name = packet.ReadString(leaf);
            LinkUser user = leaf.Users.Find(x => x.Name == name);

            if (user != null)
            {
                LeafPool.Leaves.ForEachWhere(x => x.SendPacket(HubOutbound.HubPart(x, leaf.Ident, user)),
                    x => x.Ident != leaf.Ident && x.LoginPhase == LinkLogin.Ready);

                leaf.Users.RemoveAll(x => x.Name == name);
            }
        }

        private static void LeafPing(Leaf leaf, ulong time)
        {
            leaf.Time = time;
            leaf.SendPacket(HubOutbound.HubPong());
        }

        private static void LeafPersonalMessage(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase == LinkLogin.AwaitingLogin)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            String name = packet.ReadString(leaf);
            LinkUser user = leaf.Users.Find(x => x.Name == name);

            if (user != null)
            {
                user.PersonalMessage = packet.ReadString(leaf);

                if (leaf.LoginPhase == LinkLogin.Ready)
                    LeafPool.Leaves.ForEachWhere(x => x.SendPacket(HubOutbound.HubPersonalMessage(x, leaf.Ident, user)),
                        x => x.Ident != leaf.Ident && x.LoginPhase == LinkLogin.Ready);
            }
        }

        private static void LeafAvatar(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase == LinkLogin.AwaitingLogin)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            String name = packet.ReadString(leaf);
            LinkUser user = leaf.Users.Find(x => x.Name == name);

            if (user != null)
            {
                user.Avatar = packet;

                if (leaf.LoginPhase == LinkLogin.Ready)
                    LeafPool.Leaves.ForEachWhere(x => x.SendPacket(HubOutbound.HubAvatar(x, leaf.Ident, user)),
                        x => x.Ident != leaf.Ident && x.LoginPhase == LinkLogin.Ready);
            }
        }

        private static void LeafCustomName(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase == LinkLogin.AwaitingLogin)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            String name = packet.ReadString(leaf);
            LinkUser user = leaf.Users.Find(x => x.Name == name);

            if (user != null)
            {
                user.CustomName = packet.ReadString(leaf);

                if (leaf.LoginPhase == LinkLogin.Ready)
                    LeafPool.Leaves.ForEachWhere(x => x.SendPacket(HubOutbound.HubCustomName(x, leaf.Ident, user)),
                        x => x.Ident != leaf.Ident && x.LoginPhase == LinkLogin.Ready);
            }
        }

        private static void LeafUserlistEnd(Leaf leaf)
        {
            if (leaf.LoginPhase != LinkLogin.AwaitingUserlist)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            leaf.LoginPhase = LinkLogin.Ready;

            LeafPool.Leaves.ForEachWhere(x => x.SendPacket(HubOutbound.HubLeafConnected(x, leaf)),
                x => x.Ident != leaf.Ident && x.LoginPhase == LinkLogin.Ready);

            foreach (LinkUser u in leaf.Users)
            {
                LeafPool.Leaves.ForEachWhere(x => x.SendPacket(HubOutbound.HubUserlistItem(x, leaf.Ident, u)),
                    x => x.Ident != leaf.Ident && x.LoginPhase == LinkLogin.Ready);

                if (u.CustomName.Length > 0)
                    LeafPool.Leaves.ForEachWhere(x => x.SendPacket(HubOutbound.HubCustomName(x, leaf.Ident, u)),
                        x => x.Ident != leaf.Ident && x.LoginPhase == LinkLogin.Ready);

                if (u.Avatar.Length > 0)
                    LeafPool.Leaves.ForEachWhere(x => x.SendPacket(HubOutbound.HubAvatar(x, leaf.Ident, u)),
                        x => x.Ident != leaf.Ident && x.LoginPhase == LinkLogin.Ready);

                if (u.PersonalMessage.Length > 0)
                    LeafPool.Leaves.ForEachWhere(x => x.SendPacket(HubOutbound.HubPersonalMessage(x, leaf.Ident, u)),
                        x => x.Ident != leaf.Ident && x.LoginPhase == LinkLogin.Ready);
            }

            LeafPool.Leaves.ForEachWhere(x =>
            {
                leaf.SendPacket(HubOutbound.HubLeafConnected(leaf, x));

                foreach (LinkUser u in x.Users)
                {
                    leaf.SendPacket(HubOutbound.HubUserlistItem(leaf, x.Ident, u));

                    if (u.CustomName.Length > 0)
                        leaf.SendPacket(HubOutbound.HubCustomName(leaf, x.Ident, u));

                    if (u.Avatar.Length > 0)
                        leaf.SendPacket(HubOutbound.HubAvatar(leaf, x.Ident, u));

                    if (u.PersonalMessage.Length > 0)
                        leaf.SendPacket(HubOutbound.HubPersonalMessage(leaf, x.Ident, u));
                }
            },
            x => x.Ident != leaf.Ident && x.LoginPhase == LinkLogin.Ready);
        }

        private static void LeafUserlistItem(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.AwaitingUserlist)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            LinkUser user = new LinkUser(leaf.Ident);
            user.OrgName = packet.ReadString(leaf);
            user.Name = packet.ReadString(leaf);
            user.Version = packet.ReadString(leaf);
            user.Guid = packet;
            user.FileCount = packet;
            user.ExternalIP = packet;
            user.LocalIP = packet;
            user.Port = packet;
            user.DNS = packet.ReadString(leaf);
            user.Browsable = ((byte)packet) == 1;
            user.Age = packet;
            user.Sex = packet;
            user.Country = packet;
            user.Region = packet.ReadString(leaf);
            user.Level = (iconnect.ILevel)((byte)packet);
            user.Vroom = packet;
            user.CustomClient = ((byte)packet) == 1;
            user.Muzzled = ((byte)packet) == 1;
            user.WebClient = ((byte)packet) == 1;
            user.Encrypted = ((byte)packet) == 1;
            user.Registered = ((byte)packet) == 1;
            user.Idle = ((byte)packet) == 1;
            user.PersonalMessage = String.Empty;
            user.CustomName = String.Empty;
            user.Avatar = new byte[] { };
            leaf.Users.Add(user);
        }

        private static void LeafLogin(Leaf leaf, TCPPacketReader packet, ulong time, LinkMode mode)
        {
            if (leaf.LoginPhase != LinkLogin.AwaitingLogin)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            byte[] credentials = packet.ReadBytes(20);
            leaf.Protocol = packet;
            leaf.Port = packet;

            Leaf existing = LeafPool.Leaves.Find(x => x.ExternalIP.Equals(leaf.ExternalIP) && x.Port == leaf.Port && x.Ident != leaf.Ident);

            if (existing != null)
                existing.Disconnect();

            TrustedLeafItem item = TrustedLeavesManager.GetTrusted(leaf.ExternalIP, leaf.Port, credentials);

            if (item == null)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.Untrusted));
                leaf.Disconnect();
                return;
            }

            if (mode != LinkMode.Hub)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.Unavailable));
                leaf.Disconnect();
                return;
            }

            leaf.Name = item.Name;
            leaf.Guid = item.Guid;
            
            if (leaf.Protocol < Settings.LINK_PROTO)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.ExpiredProtocol));
                leaf.Disconnect();
                return;
            }

            leaf.LoginPhase = LinkLogin.AwaitingUserlist;
            leaf.Key = Crypto.CreateKey;
            leaf.IV = Crypto.CreateIV;
            leaf.SendPacket(HubOutbound.HubAck(leaf));
        }


    }
}
