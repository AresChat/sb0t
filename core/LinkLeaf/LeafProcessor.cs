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
using System.Security.Cryptography;

namespace core.LinkLeaf
{
    class LeafProcessor
    {
        public static void Eval(LinkClient link, core.LinkHub.LinkMsg msg, TCPPacketReader packet, ulong time)
        {
            switch (msg)
            {
                case LinkHub.LinkMsg.MSG_LINK_ERROR:
                    Error(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_ACK:
                    HubAck(link, packet, time);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_PONG:
                    link.LastPong = time;
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_PART:
                    HubPart(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_USERLIST_ITEM:
                    HubUserlistItem(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_LEAF_CONNECTED:
                    HubLeafConnected(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_LEAF_DISCONNECTED:
                    HubLeafDisconnected(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_USER_UPDATED:
                    HubUserUpdated(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_NICK_CHANGED:
                    HubNickChanged(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_VROOM_CHANGED:
                    HubVroomChanged(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_AVATAR:
                    HubAvatar(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_PERSONAL_MESSAGE:
                    HubPersonalMessage(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_IUSER:
                    HubIUser(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_IUSER_BIN:
                    HubIUserBin(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_CUSTOM_NAME:
                    HubCustomName(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_ADMIN:
                    HubAdmin(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_PUBLIC_TEXT:
                    HubPublicText(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_EMOTE_TEXT:
                    HubEmoteText(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_PRIVATE_TEXT:
                    HubPrivateText(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_PRIVATE_IGNORED:
                    HubPrivateIgnored(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_NO_ADMIN:
                    HubNoAdmin(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_BROWSE:
                    HubBrowse(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_BROWSE_DATA:
                    HubBrowseData(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_CUSTOM_DATA_TO:
                    HubCustomDataTo(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_CUSTOM_DATA_ALL:
                    HubCustomDataAll(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_PRINT_ALL:
                    HubPrintAll(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_PRINT_VROOM:
                    HubPrintVroom(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_PRINT_LEVEL:
                    HubPrintLevel(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_PUBLIC_TO_USER:
                    HubPublicToUser(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_EMOTE_TO_USER:
                    HubEmoteToUser(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_PUBLIC_TO_LEAF:
                    HubPublicToLeaf(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_EMOTE_TO_LEAF:
                    HubEmoteToLeaf(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_NUDGE:
                    HubNudge(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_SCRIBBLE_USER:
                    HubScribbleUser(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_SCRIBBLE_LEAF:
                    HubScribbleLeaf(link, packet);
                    break;
            }
        }

        private static void HubScribbleUser(LinkClient link, TCPPacketReader packet)
        {
            String target = packet.ReadString(link);
            String sender = packet.ReadString(link);
            int height = (int)((uint)packet);
            byte[] img = packet;
            AresClient ac = UserPool.AUsers.Find(x => x.Name == target && x.LoggedIn && !x.Quarantined);

            if (ac != null)
            {
                if (ac.CustomClient)
                    ac.Scribble(sender, img, height);
            }
            else
            {
                ib0t.ib0tClient ic = UserPool.WUsers.Find(x => x.Name == target && x.LoggedIn && !x.Quarantined);

                if (ic != null)
                    if (ic.Extended)
                        ic.Scribble(sender, img, height);
            }
        }

        private static void HubScribbleLeaf(LinkClient link, TCPPacketReader packet)
        {
            String sender = packet.ReadString(link);
            int height = (int)((uint)packet);
            byte[] img = packet;

            UserPool.AUsers.ForEachWhere(x => x.Scribble(sender, img, height),
                x => x.LoggedIn && !x.Quarantined && x.CustomClient);

            UserPool.WUsers.ForEachWhere(x => x.Scribble(sender, img, height),
                x => x.LoggedIn && !x.Quarantined && x.Extended);
        }

        private static void HubNudge(LinkClient link, TCPPacketReader packet)
        {
            String target = packet.ReadString(link);
            String sender = packet.ReadString(link);
            AresClient ac = UserPool.AUsers.Find(x => x.Name == target && x.LoggedIn && !x.Quarantined);

            if (ac != null)
                ac.Nudge(sender);
        }

        private static void HubPublicToUser(LinkClient link, TCPPacketReader packet)
        {
            String target = packet.ReadString(link);
            String sender = packet.ReadString(link);
            String text = packet.ReadString(link);
            AresClient ac = UserPool.AUsers.Find(x => x.Name == target && x.LoggedIn && !x.Quarantined);

            if (ac != null)
                ac.SendPacket(TCPOutbound.Public(ac, sender, text));
            else
            {
                ib0t.ib0tClient ic = UserPool.WUsers.Find(x => x.Name == target && x.LoggedIn && !x.Quarantined);

                if (ic != null)
                    ic.QueuePacket(ib0t.WebOutbound.PublicTo(ic, sender, text));
            }
        }

        private static void HubEmoteToUser(LinkClient link, TCPPacketReader packet)
        {
            String target = packet.ReadString(link);
            String sender = packet.ReadString(link);
            String text = packet.ReadString(link);
            AresClient ac = UserPool.AUsers.Find(x => x.Name == target && x.LoggedIn && !x.Quarantined);

            if (ac != null)
                ac.SendPacket(TCPOutbound.Emote(ac, sender, text));
            else
            {
                ib0t.ib0tClient ic = UserPool.WUsers.Find(x => x.Name == target && x.LoggedIn && !x.Quarantined);

                if (ic != null)
                    ic.QueuePacket(ib0t.WebOutbound.EmoteTo(ic, sender, text));
            }
        }

        private static void HubPublicToLeaf(LinkClient link, TCPPacketReader packet)
        {
            String sender = packet.ReadString(link);
            String text = packet.ReadString(link);

            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Public(x, sender, text)),
                x => x.LoggedIn && !x.Quarantined);

            UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.PublicTo(x, sender, text)),
                x => x.LoggedIn && !x.Quarantined);
        }

        private static void HubEmoteToLeaf(LinkClient link, TCPPacketReader packet)
        {
            String sender = packet.ReadString(link);
            String text = packet.ReadString(link);

            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Emote(x, sender, text)),
                x => x.LoggedIn && !x.Quarantined);

            UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.EmoteTo(x, sender, text)),
                x => x.LoggedIn && !x.Quarantined);
        }

        private static void HubPrintAll(LinkClient link, TCPPacketReader packet)
        {
            String text = packet.ReadString(link);

            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.NoSuch(x, text)),
                x => x.LoggedIn && !x.Quarantined);

            UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.NoSuchTo(x, text)),
                x => x.LoggedIn && !x.Quarantined);
        }

        private static void HubPrintVroom(LinkClient link, TCPPacketReader packet)
        {
            ushort vroom = packet;
            String text = packet.ReadString(link);

            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.NoSuch(x, text)),
                x => x.LoggedIn && !x.Quarantined && x.Vroom == vroom);

            UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.NoSuchTo(x, text)),
                x => x.LoggedIn && !x.Quarantined && x.Vroom == vroom);
        }

        private static void HubPrintLevel(LinkClient link, TCPPacketReader packet)
        {
            iconnect.ILevel level = (iconnect.ILevel)((byte)packet);
            String text = packet.ReadString(link);

            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.NoSuch(x, text)),
                x => x.LoggedIn && !x.Quarantined && x.Level > level);

            UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.NoSuchTo(x, text)),
                x => x.LoggedIn && !x.Quarantined && x.Level > level);
        }

        private static void HubCustomDataAll(LinkClient link, TCPPacketReader packet)
        {
            ushort vroom = packet;
            String sender = packet.ReadString(link);
            String ident = packet.ReadString(link);
            byte[] data = packet;

            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.CustomData(x, sender, ident, data)),
                x => x.LoggedIn && x.Vroom == vroom && x.CustomClient && !x.Quarantined);
        }

        private static void HubCustomDataTo(LinkClient link, TCPPacketReader packet)
        {
            String sender = packet.ReadString(link);
            String target = packet.ReadString(link);
            String ident = packet.ReadString(link);
            byte[] data = packet;
            AresClient client = UserPool.AUsers.Find(x => x.Name == target && !x.Quarantined && x.LoggedIn);

            if (client != null)
                client.SendPacket(TCPOutbound.CustomData(client, sender, ident, data));
        }

        private static void HubBrowseData(LinkClient link, TCPPacketReader packet)
        {
            String name = packet.ReadString(link);
            AresClient target = UserPool.AUsers.Find(x => x.Name == name && !x.Quarantined && x.LoggedIn);

            if (target != null)
            {
                byte[] data = packet;
                target.SendPacket(data);
            }
        }

        private static void HubBrowse(LinkClient link, TCPPacketReader packet)
        {
            uint leaf_ident = packet;
            Leaf leaf = link.Leaves.Find(x => x.Ident == leaf_ident);

            if (leaf != null)
            {
                String browsee = packet.ReadString(link);
                String browser = packet.ReadString(link);
                ushort browse_ident = packet;
                byte mime = packet;
                AresClient target = UserPool.AUsers.Find(x => x.Name == browsee && !x.Quarantined && x.LoggedIn);

                if (target == null)
                    link.SendPacket(LeafOutbound.LeafBrowseData(link, leaf.Ident, browser, TCPOutbound.BrowseError(browse_ident)));
                else if (target.SharedFiles.Count == 0)
                    link.SendPacket(LeafOutbound.LeafBrowseData(link, leaf.Ident, browser, TCPOutbound.BrowseError(browse_ident)));
                else
                {
                    var linq = from x in target.SharedFiles
                               where (mime == 0 || mime == 255) || (byte)x.Mime == (mime == 8 ? 0 : mime)
                               select TCPOutbound.BrowseItem(browse_ident, x);

                    link.SendPacket(LeafOutbound.LeafBrowseData(link, leaf.Ident, browser, TCPOutbound.StartOfBrowse(browse_ident, (ushort)linq.Count())));

                    List<byte> buffer = new List<byte>();

                    foreach (byte[] x in linq)
                    {
                        buffer.AddRange(x);

                        if (buffer.Count > 1024)
                        {
                            link.SendPacket(LeafOutbound.LeafBrowseData(link, leaf.Ident, browser, TCPOutbound.ClientCompressed(buffer.ToArray())));
                            buffer.Clear();
                        }
                    }

                    if (buffer.Count > 0)
                        link.SendPacket(LeafOutbound.LeafBrowseData(link, leaf.Ident, browser, TCPOutbound.ClientCompressed(buffer.ToArray())));

                    link.SendPacket(LeafOutbound.LeafBrowseData(link, leaf.Ident, browser, TCPOutbound.EndOfBrowse(browse_ident)));
                }
            }
        }

        private static void HubNoAdmin(LinkClient link, TCPPacketReader packet)
        {
            uint ident = packet;
            Leaf leaf = link.Leaves.Find(x => x.Ident == ident);

            if (leaf != null)
            {
                String name = packet.ReadString(link);
                IClient user = UserPool.AUsers.Find(x => x.Name == name && x.LoggedIn && !x.Quarantined);

                if (user == null)
                    user = UserPool.WUsers.Find(x => x.Name == name && x.LoggedIn && !x.Quarantined);

                if (user != null)
                    Events.LinkedAdminDisabled(leaf, user);
            }
        }

        private static void HubPrivateIgnored(LinkClient link, TCPPacketReader packet)
        {
            String sender_name = packet.ReadString(link);
            String target_name = packet.ReadString(link);
            AresClient sender = UserPool.AUsers.Find(x => x.Name == sender_name && x.LoggedIn && !x.Quarantined);

            if (sender != null)
                sender.SendPacket(TCPOutbound.IsIgnoringYou(sender, target_name));
        }

        private static void HubPrivateText(LinkClient link, TCPPacketReader packet)
        {
            uint sender_ident = packet;
            Leaf leaf = link.Leaves.Find(x => x.Ident == sender_ident);

            if (leaf != null)
            {
                String sender_name = packet.ReadString(link);
                LinkUser sender = leaf.Users.Find(x => x.Name == sender_name);

                if (sender != null)
                {
                    String target_name = packet.ReadString(link);
                    IClient target = UserPool.AUsers.Find(x => x.Name == target_name && x.LoggedIn && !x.Quarantined);

                    if (target == null)
                        target = UserPool.WUsers.Find(x => x.Name == target_name && x.LoggedIn && !x.Quarantined);

                    if (target != null)
                    {
                        if (target.IgnoreList.Contains(sender.Name) || sender.Muzzled)
                            link.SendPacket(LeafOutbound.LeafPrivateIgnored(link, sender.Link.Ident, sender.Name, target.Name));
                        else
                        {
                            String text = packet.ReadString(link);
                            PMEventArgs args = new PMEventArgs { Cancel = false, Text = text };
                            Events.PrivateSending(sender, target, args);

                            if (!args.Cancel && !String.IsNullOrEmpty(args.Text))
                            {
                                target.IUser.PM(sender.Name, args.Text);
                                Events.PrivateSent(sender, target, args.Text);
                            }
                        }
                    }
                }
            }
        }

        private static void HubPublicText(LinkClient link, TCPPacketReader packet)
        {
            uint ident = packet;
            Leaf leaf = link.Leaves.Find(x => x.Ident == ident);

            if (leaf != null)
            {
                String name = packet.ReadString(link);
                LinkUser user = leaf.Users.Find(x => x.Name == name);

                if (user != null)
                {
                    String text = packet.ReadString(link);
                    Events.TextReceived(user, text);
                    FloodControl.LinkPost();
                    text = Events.TextSending(user, text);

                    if (!String.IsNullOrEmpty(text))
                    {
                        byte[] js_style = null;

                        UserPool.AUsers.ForEachWhere(x =>
                        {
                            if (x.SupportsHTML)
                            {
                                if (String.IsNullOrEmpty(user.CustomName) || x.BlockCustomNames)
                                {
                                    if (x.SupportsHTML)
                                        if (js_style != null)
                                            x.SendPacket(js_style);

                                    x.SendPacket(TCPOutbound.Public(x, user.Name, text));
                                }
                                else
                                {
                                    if (x.SupportsHTML)
                                        if (js_style != null)
                                            x.SendPacket(js_style);

                                    x.SendPacket(TCPOutbound.NoSuch(x, user.CustomName + text));
                                }
                            }
                            else
                            {
                                if (String.IsNullOrEmpty(user.CustomName) || x.BlockCustomNames)
                                    x.SendPacket(TCPOutbound.Public(x, user.Name, text));
                                else
                                    x.SendPacket(TCPOutbound.NoSuch(x, user.CustomName + text));
                            }
                        }, x => x.LoggedIn && x.Vroom == user.Vroom && !x.IgnoreList.Contains(user.Name) && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(String.IsNullOrEmpty(user.CustomName) ?
                            ib0t.WebOutbound.PublicTo(x, user.Name, text) : ib0t.WebOutbound.NoSuchTo(x, user.CustomName + text)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.IgnoreList.Contains(user.Name) && !x.Quarantined);

                        Events.TextSent(user, text);
                    }
                }
            }
        }

        private static void HubEmoteText(LinkClient link, TCPPacketReader packet)
        {
            uint ident = packet;
            Leaf leaf = link.Leaves.Find(x => x.Ident == ident);

            if (leaf != null)
            {
                String name = packet.ReadString(link);
                LinkUser user = leaf.Users.Find(x => x.Name == name);

                if (user != null)
                {
                    String text = packet.ReadString(link);
                    Events.EmoteReceived(user, text);
                    FloodControl.LinkPost();
                    text = Events.EmoteSending(user, text);

                    if (!String.IsNullOrEmpty(text))
                    {
                        byte[] js_style = null;

                        UserPool.AUsers.ForEachWhere(x =>
                        {
                            if (x.SupportsHTML)
                                if (js_style != null)
                                    x.SendPacket(js_style);

                            x.SendPacket(TCPOutbound.Emote(x, user.Name, text));
                        }, x => x.LoggedIn && x.Vroom == user.Vroom && !x.IgnoreList.Contains(user.Name) && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.EmoteTo(x, user.Name, text)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.IgnoreList.Contains(user.Name) && !x.Quarantined);

                        Events.EmoteSent(user, text);
                    }
                }
            }
        }

        private static void HubAdmin(LinkClient link, TCPPacketReader packet)
        {
            uint sender_ident = packet;
            Leaf leaf = link.Leaves.Find(x => x.Ident == sender_ident);

            if (leaf != null)
            {
                String sender_name = packet.ReadString(link);

                if (!Settings.Get<bool>("link_admin"))
                {
                    link.SendPacket(LeafOutbound.LeafNoAdmin(link, leaf.Ident, sender_name));
                    return;
                }

                LinkUser admin = leaf.Users.Find(x => x.Name == sender_name);

                if (admin != null)
                {
                    String target_name = packet.ReadString(link);
                    IClient target = UserPool.AUsers.Find(x => x.Name == target_name && x.LoggedIn && !x.Quarantined);

                    if (target == null)
                        target = UserPool.WUsers.Find(x => x.Name == target_name && x.LoggedIn && !x.Quarantined);

                    if (target != null)
                    {
                        String command = packet.ReadString(link);
                        String args = packet.ReadString(link);
                        Events.Command(admin, command, target, args);
                    }
                }
            }
        }

        private static void HubCustomName(LinkClient link, TCPPacketReader packet)
        {
            uint ident = packet;
            String name = packet.ReadString(link);
            String cname = packet.ReadString(link);
            Leaf leaf = link.Leaves.Find(x => x.Ident == ident);

            if (leaf != null)
            {
                LinkUser user = leaf.Users.Find(x => x.Name == name);

                if (user != null)
                    user.SetCustomName(cname);
            }
        }

        private static void HubIUser(LinkClient link, TCPPacketReader packet)
        {
            String _name = packet.ReadString(link);
            IClient client = UserPool.AUsers.Find(x => x.LoggedIn && !x.Quarantined && x.Name == _name);

            if (client == null)
                client = UserPool.WUsers.Find(x => x.LoggedIn && !x.Quarantined && x.Name == _name);

            if (client != null)
            {
                String command = packet.ReadString(link);
                String name, text;

                switch (command)
                {
                    case "print":
                        text = packet.ReadString(link);
                        client.IUser.Print(text);
                        break;

                    case "muzzle":
                        if (Settings.Get<bool>("link_admin"))
                            if (client.Level == iconnect.ILevel.Regular)
                                client.IUser.Muzzled = true;
                        break;

                    case "unmuzzle":
                        if (Settings.Get<bool>("link_admin"))
                            client.IUser.Muzzled = false;
                        break;

                    case "customname":
                        if (Settings.Get<bool>("link_admin"))
                        {
                            text = packet.ReadString(link);
                            client.IUser.CustomName = text;
                        }
                        break;

                    case "personalmessage":
                        if (Settings.Get<bool>("link_admin"))
                        {
                            text = packet.ReadString(link);
                            client.IUser.PersonalMessage = text;
                        }
                        break;

                    case "name":
                        if (Settings.Get<bool>("link_admin"))
                        {
                            name = packet.ReadString(link);
                            client.IUser.Name = name;
                        }
                        break;

                    case "ban":
                        if (Settings.Get<bool>("link_admin"))
                            if (client.Level == iconnect.ILevel.Regular)
                                client.IUser.Ban();
                        break;

                    case "disconnect":
                        if (Settings.Get<bool>("link_admin"))
                            if (client.Level == iconnect.ILevel.Regular)
                                client.IUser.Disconnect();
                        break;

                    case "redirect":
                        if (Settings.Get<bool>("link_admin"))
                        {
                            text = packet.ReadString(link);
                            client.IUser.Redirect(text);
                        }
                        break;

                    case "sendtext":
                        if (Settings.Get<bool>("link_admin"))
                        {
                            text = packet.ReadString(link);
                            client.IUser.SendText(text);
                        }
                        break;

                    case "sendemote":
                        if (Settings.Get<bool>("link_admin"))
                        {
                            text = packet.ReadString(link);
                            client.IUser.SendEmote(text);
                        }
                        break;

                    case "pm":
                        if (Settings.Get<bool>("link_admin"))
                        {
                            name = packet.ReadString(link);
                            text = packet.ReadString(link);
                            client.IUser.PM(name, text);
                        }
                        break;

                    case "topic":
                        if (Settings.Get<bool>("link_admin"))
                        {
                            text = packet.ReadString(link);
                            client.IUser.Topic(text);
                        }
                        break;

                    case "restoreavatar":
                        if (Settings.Get<bool>("link_admin"))
                            client.IUser.RestoreAvatar();
                        break;

                    case "url":
                        if (Settings.Get<bool>("link_admin"))
                        {
                            name = packet.ReadString(link);
                            text = packet.ReadString(link);
                            client.IUser.URL(name, text);
                        }
                        break;
                }
            }
        }

        private static void HubIUserBin(LinkClient link, TCPPacketReader packet)
        {
            String name = packet.ReadString(link);
            IClient client = UserPool.AUsers.Find(x => x.LoggedIn && !x.Quarantined && x.Name == name);

            if (client == null)
                client = UserPool.WUsers.Find(x => x.LoggedIn && !x.Quarantined && x.Name == name);

            if (client != null)
            {
                String command = packet.ReadString(link);
                byte[] args = packet;

                switch (command)
                {
                    case "binary":
                        client.IUser.BinaryWrite(args);
                        break;

                    case "avatar":
                        if (Settings.Get<bool>("link_admin"))
                            client.IUser.Avatar = args;
                        break;

                    case "vroom":
                        if (Settings.Get<bool>("link_admin"))
                            client.IUser.Vroom = BitConverter.ToUInt16(args, 0);
                        break;
                }
            }
        }

        private static void HubAvatar(LinkClient link, TCPPacketReader packet)
        {
            uint leaf_ident = packet;
            Leaf leaf = link.Leaves.Find(x => x.Ident == leaf_ident);

            if (leaf != null)
            {
                String name = packet.ReadString(link);
                LinkUser user = leaf.Users.Find(x => x.Name == name);
                byte[] buffer = packet;

                if (user != null)
                    if (Events.AvatarReceived(user))
                    {
                        user.SetAvatar(buffer);

                        if (user.Link.Visible)
                        {
                            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Avatar(x, user)),
                                x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);

                            UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.AvatarTo(x, user.Name, user.Avatar)),
                                x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined && x.Extended);
                        }
                    }
            }
        }

        private static void HubPersonalMessage(LinkClient link, TCPPacketReader packet)
        {
            uint leaf_ident = packet;
            Leaf leaf = link.Leaves.Find(x => x.Ident == leaf_ident);

            if (leaf != null)
            {
                String name = packet.ReadString(link);
                LinkUser user = leaf.Users.Find(x => x.Name == name);
                String text = packet.ReadString(link);

                if (user != null)
                    if (Events.PersonalMessageReceived(user, text))
                    {
                        user.SetPersonalMessage(text);

                        if (user.Link.Visible)
                        {
                            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.PersonalMessage(x, user)),
                                x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);

                            UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.PersMsgTo(x, user.Name, user.PersonalMessage)),
                                x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined && x.Extended);
                        }
                    }
            }
        }

        private static void HubPart(LinkClient link, TCPPacketReader packet)
        {
            uint leaf_ident = packet;
            Leaf leaf = link.Leaves.Find(x => x.Ident == leaf_ident);

            if (leaf != null)
            {
                String name = packet.ReadString(link);
                LinkUser user = leaf.Users.Find(x => x.Name == name);

                if (user != null)
                {
                    Events.Parting(user);

                    if (user.Link.Visible)
                    {
                        IClient other = null;

                        foreach (Leaf l in link.Leaves)
                            if (l.Ident != leaf.Ident)
                            {
                                other = l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom);

                                if (other != null)
                                {
                                    l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom).LinkCredentials.Visible = true;
                                    break;
                                }
                            }

                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(other == null ? TCPOutbound.Part(x, user) : TCPOutbound.UpdateUserStatus(x, other)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(other == null ? ib0t.WebOutbound.PartTo(x, user.Name) : ib0t.WebOutbound.UpdateTo(x, user.Name, user.Level)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);
                    }

                    leaf.Users.RemoveAll(x => x.Name == name);
                }

                Events.Parted(user);
            }
        }

        private static void HubNickChanged(LinkClient link, TCPPacketReader packet)
        {
            uint leaf_ident = packet;
            Leaf leaf = link.Leaves.Find(x => x.Ident == leaf_ident);

            if (leaf != null)
            {
                String name = packet.ReadString(link);
                LinkUser user = leaf.Users.Find(x => x.Name == name);

                if (user != null)
                {
                    String new_name = packet.ReadString(link);

                    if (user.Link.Visible)
                    {
                        IClient other = null;

                        foreach (Leaf l in link.Leaves)
                            if (l.Ident != leaf.Ident)
                            {
                                other = l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom);

                                if (other != null)
                                {
                                    l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom).LinkCredentials.Visible = true;
                                    break;
                                }
                            }

                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(other == null ? TCPOutbound.Part(x, user) : TCPOutbound.UpdateUserStatus(x, other)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(other == null ? ib0t.WebOutbound.PartTo(x, user.Name) : ib0t.WebOutbound.UpdateTo(x, user.Name, user.Level)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);
                    }

                    user.SetName(new_name);
                    user.LinkCredentials.Visible = UserPool.AUsers.Find(x => x.LoggedIn && x.Name == user.Name && !x.Quarantined && x.Vroom == user.Vroom) == null;

                    if (user.Link.Visible)
                        user.LinkCredentials.Visible = UserPool.WUsers.Find(x => x.LoggedIn && x.Name == user.Name && !x.Quarantined && x.Vroom == user.Vroom) == null;

                    if (user.Link.Visible)
                        foreach (Leaf l in link.Leaves)
                            if (l.Ident != leaf.Ident)
                            {
                                user.LinkCredentials.Visible = l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom) == null;

                                if (!user.Link.Visible)
                                    break;
                            }

                    if (user.Link.Visible)
                    {
                        UserPool.AUsers.ForEachWhere(x =>
                        {
                            x.SendPacket(TCPOutbound.Join(x, user));

                            if (user.Avatar.Length > 0)
                                x.SendPacket(TCPOutbound.Avatar(x, user));

                            if (user.PersonalMessage.Length > 0)
                                x.SendPacket(TCPOutbound.PersonalMessage(x, user));
                        },
                        x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x =>
                        {
                            x.QueuePacket(ib0t.WebOutbound.JoinTo(x, user.Name, user.Level));

                            if (user.Avatar.Length > 0)
                                x.QueuePacket(ib0t.WebOutbound.AvatarTo(x, user.Name, user.Avatar));

                            if (user.PersonalMessage.Length > 0)
                                x.QueuePacket(ib0t.WebOutbound.PersMsgTo(x, user.Name, user.PersonalMessage));
                        },
                        x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined && x.Extended);
                    }
                }
            }
        }

        private static void HubVroomChanged(LinkClient link, TCPPacketReader packet)
        {
            uint leaf_ident = packet;
            Leaf leaf = link.Leaves.Find(x => x.Ident == leaf_ident);

            if (leaf != null)
            {
                String name = packet.ReadString(link);
                LinkUser user = leaf.Users.Find(x => x.Name == name);

                if (user != null)
                {
                    ushort new_vroom = packet;

                    if (user.Link.Visible)
                    {
                        IClient other = null;

                        foreach (Leaf l in link.Leaves)
                            if (l.Ident != leaf.Ident)
                            {
                                other = l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom);

                                if (other != null)
                                {
                                    l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom).LinkCredentials.Visible = true;
                                    break;
                                }
                            }

                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(other == null ? TCPOutbound.Part(x, user) : TCPOutbound.UpdateUserStatus(x, other)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(other == null ? ib0t.WebOutbound.PartTo(x, user.Name) : ib0t.WebOutbound.UpdateTo(x, user.Name, user.Level)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);
                    }

                    user.SetVroom(new_vroom);
                    user.LinkCredentials.Visible = UserPool.AUsers.Find(x => x.LoggedIn && x.Name == user.Name && !x.Quarantined && x.Vroom == user.Vroom) == null;

                    if (user.Link.Visible)
                        user.LinkCredentials.Visible = UserPool.WUsers.Find(x => x.LoggedIn && x.Name == user.Name && !x.Quarantined && x.Vroom == user.Vroom) == null;

                    if (user.Link.Visible)
                        foreach (Leaf l in link.Leaves)
                            if (l.Ident != leaf.Ident)
                            {
                                user.LinkCredentials.Visible = l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom) == null;

                                if (!user.Link.Visible)
                                    break;
                            }

                    if (user.Link.Visible)
                    {
                        UserPool.AUsers.ForEachWhere(x =>
                        {
                            x.SendPacket(TCPOutbound.Join(x, user));

                            if (user.Avatar.Length > 0)
                                x.SendPacket(TCPOutbound.Avatar(x, user));

                            if (user.PersonalMessage.Length > 0)
                                x.SendPacket(TCPOutbound.PersonalMessage(x, user));
                        },
                        x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x =>
                        {
                            x.QueuePacket(ib0t.WebOutbound.JoinTo(x, user.Name, user.Level));

                            if (user.Avatar.Length > 0)
                                x.QueuePacket(ib0t.WebOutbound.AvatarTo(x, user.Name, user.Avatar));

                            if (user.PersonalMessage.Length > 0)
                                x.QueuePacket(ib0t.WebOutbound.PersMsgTo(x, user.Name, user.PersonalMessage));
                        },
                        x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined && x.Extended);
                    }

                    Events.VroomChanged(user);
                }
            }
        }

        private static void HubUserUpdated(LinkClient link, TCPPacketReader packet)
        {
            uint leaf_ident = packet;
            Leaf leaf = link.Leaves.Find(x => x.Ident == leaf_ident);

            if (leaf != null)
            {
                String name = packet.ReadString(link);
                LinkUser user = leaf.Users.Find(x => x.Name == name);

                if (user != null)
                {
                    iconnect.ILevel c_level = user.Level;
                    user.Level = (iconnect.ILevel)((byte)packet);

                    if (c_level != user.Level)
                        Events.AdminLevelChanged(user);

                    user.SetMuzzled(((byte)packet) == 1);

                    bool c_bool = user.Registered;
                    user.Registered = ((byte)packet) == 1;

                    if (c_bool != user.Registered)
                    {
                        if (user.Registered)
                            Events.LoginGranted(user);
                        else
                            Events.Logout(user);
                    }

                    c_bool = user.Idle;
                    user.Idle = ((byte)packet) == 1;

                    if (c_bool != user.Idle)
                    {
                        if (user.Idle)
                        {
                            user.IdleStart = Time.Now;
                            Events.Idled(user);
                        }
                        else
                        {
                            uint seconds_away = (uint)((Time.Now - user.IdleStart) / 1000);
                            Events.Unidled(user, seconds_away);
                        }
                    }

                    if (user.Link.Visible)
                    {
                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.UpdateUserStatus(x, user)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.UpdateTo(x, user.Name, user.Level)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);
                    }
                }
            }
        }

        private static void HubLeafConnected(LinkClient link, TCPPacketReader packet)
        {
            Leaf leaf = new Leaf();
            leaf.Ident = packet;
            leaf.Name = packet.ReadString(link);
            leaf.ExternalIP = packet;
            leaf.Port = packet;
            link.Leaves.Add(leaf);
            Events.LeafJoined(leaf);
        }

        private static void HubLeafDisconnected(LinkClient link, TCPPacketReader packet)
        {
            uint leaf_ident = packet;
            Leaf leaf = link.Leaves.Find(x => x.Ident == leaf_ident);

            if (leaf != null)
            {
                foreach (LinkUser user in leaf.Users)
                    if (user.Link.Visible)
                    {
                        IClient other = null;

                        foreach (Leaf l in link.Leaves)
                            if (l.Ident != leaf.Ident)
                            {
                                other = l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom);

                                if (other != null)
                                {
                                    l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom).LinkCredentials.Visible = true;
                                    break;
                                }
                            }

                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(other == null ? TCPOutbound.Part(x, user) : TCPOutbound.UpdateUserStatus(x, other)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(other == null ? ib0t.WebOutbound.PartTo(x, user.Name) : ib0t.WebOutbound.UpdateTo(x, user.Name, user.Level)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);
                    }

                link.Leaves.RemoveAll(x => x.Ident == leaf_ident);
                Events.LeafParted(leaf);
            }
        }

        private static void HubUserlistItem(LinkClient link, TCPPacketReader packet)
        {
            uint leaf_ident = packet;
            Leaf leaf = link.Leaves.Find(x => x.Ident == leaf_ident);

            if (leaf != null)
            {
                LinkUser user = new LinkUser(leaf_ident);
                user.JoinTime = Helpers.UnixTime;
                user.OrgName = packet.ReadString(link);
                user.SetName(packet.ReadString(link));
                user.Version = packet.ReadString(link);
                user.Guid = packet;
                user.FileCount = packet;
                user.ExternalIP = packet;
                user.LocalIP = packet;
                user.DataPort = packet;
                user.DNS = packet.ReadString(link);
                user.Browsable = ((byte)packet) == 1;
                user.Age = packet;
                user.Sex = packet;
                user.Country = packet;
                user.Region = packet.ReadString(link);

                if (user.Region.Length > 30)
                {
                    user.Region = user.Region.Substring(0, 30);
                }

                user.Level = (iconnect.ILevel)(byte)packet;
                user.SetVroom((ushort)packet);
                user.CustomClient = ((byte)packet) == 1;
                user.Muzzled = ((byte)packet) == 1;
                user.WebClient = ((byte)packet) == 1;
                user.Encrypted = ((byte)packet) == 1;
                user.Registered = ((byte)packet) == 1;
                user.Idle = ((byte)packet) == 1;
                user.IdleStart = Time.Now;
                user.LinkCredentials.Visible = UserPool.AUsers.Find(x => x.LoggedIn && x.Name == user.Name && !x.Quarantined && x.Vroom == user.Vroom) == null;

                if (user.Link.Visible)
                    user.LinkCredentials.Visible = UserPool.WUsers.Find(x => x.LoggedIn && x.Name == user.Name && !x.Quarantined && x.Vroom == user.Vroom) == null;

                if (user.Link.Visible)
                    foreach (Leaf l in link.Leaves)
                        if (l.Ident != leaf.Ident)
                        {
                            user.LinkCredentials.Visible = l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom) == null;

                            if (!user.Link.Visible)
                                break;
                        }

                if (user.Link.Visible)
                {
                    UserPool.AUsers.ForEachWhere(x =>
                    {
                        x.SendPacket(TCPOutbound.Join(x, user));

                        if (user.Avatar.Length > 0)
                            x.SendPacket(TCPOutbound.Avatar(x, user));

                        if (user.PersonalMessage.Length > 0)
                            x.SendPacket(TCPOutbound.PersonalMessage(x, user));
                    },
                        x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);

                    UserPool.WUsers.ForEachWhere(x =>
                    {
                        x.QueuePacket(ib0t.WebOutbound.JoinTo(x, user.Name, user.Level));

                        if (user.Avatar.Length > 0)
                            x.QueuePacket(ib0t.WebOutbound.AvatarTo(x, user.Name, user.Avatar));

                        if (user.PersonalMessage.Length > 0)
                            x.QueuePacket(ib0t.WebOutbound.PersMsgTo(x, user.Name, user.PersonalMessage));
                    },
                        x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined && x.Extended);
                }

                leaf.Users.Add(user);
                Events.Joined(user);

                if (user.Level > iconnect.ILevel.Regular)
                    Events.AdminLevelChanged(user);
            }
        }

        private static void Error(LinkClient link, TCPPacketReader packet)
        {
            core.LinkHub.LinkError error = (core.LinkHub.LinkError)((byte)packet);
            ServerCore.Log("LINK ERROR: " + error);

            switch (error)
            {
                case LinkHub.LinkError.Unavailable:
                    Events.LinkError(LinkError.HubException_NotAcceptingLeaves);
                    break;

                case LinkHub.LinkError.ExpiredProtocol:
                    Events.LinkError(LinkError.HubException_WantsHigherProtocol);
                    break;

                case LinkHub.LinkError.Untrusted:
                    Events.LinkError(LinkError.HubException_DoesNotTrustYou);
                    break;

                case LinkHub.LinkError.HandshakeTimeout:
                    Events.LinkError(LinkError.HubException_HandshakeTimeout);
                    break;

                case LinkHub.LinkError.PingTimeout:
                    Events.LinkError(LinkError.HubException_PingTimeout);
                    break;

                case LinkHub.LinkError.BadProtocol:
                    Events.LinkError(LinkError.HubException_BadProtocol);
                    break;
            }
        }

        private static void HubAck(LinkClient link, TCPPacketReader packet, ulong time)
        {
            byte[] crypto = packet.ReadBytes(48);
            byte[] guid = Settings.Get<byte[]>("guid");

            using (MD5 md5 = MD5.Create())
                guid = md5.ComputeHash(guid);

            for (int i = (guid.Length - 2); i > -1; i -= 2)
                crypto = Crypto.d67(crypto, BitConverter.ToUInt16(guid, i));

            List<byte> list = new List<byte>(crypto);
            link.IV = list.GetRange(0, 16).ToArray();
            link.Key = list.GetRange(16, 32).ToArray();
            link.Ident = packet;
            link.LoginPhase = LinkLogin.Ready;

            UserPool.AUsers.ForEachWhere(x => link.SendPacket(LeafOutbound.LeafUserlistItem(link, x)),
                x => x.LoggedIn && !x.Quarantined);
            UserPool.AUsers.ForEachWhere(x => link.SendPacket(LeafOutbound.LeafAvatar(link, x)),
                x => x.LoggedIn && !x.Quarantined && x.Avatar.Length > 0);
            UserPool.AUsers.ForEachWhere(x => link.SendPacket(LeafOutbound.LeafPersonalMessage(link, x)),
                x => x.LoggedIn && !x.Quarantined && x.PersonalMessage.Length > 0);
            UserPool.AUsers.ForEachWhere(x => link.SendPacket(LeafOutbound.LeafCustomName(link, x)),
                x => x.LoggedIn && !x.Quarantined && !String.IsNullOrEmpty(x.CustomName));
            UserPool.WUsers.ForEachWhere(x => link.SendPacket(LeafOutbound.LeafUserlistItem(link, x)),
                x => x.LoggedIn && !x.Quarantined);
            UserPool.WUsers.ForEachWhere(x => link.SendPacket(LeafOutbound.LeafAvatar(link, x)),
                x => x.LoggedIn && !x.Quarantined);
            UserPool.WUsers.ForEachWhere(x => link.SendPacket(LeafOutbound.LeafPersonalMessage(link, x)),
                x => x.LoggedIn && !x.Quarantined);
            UserPool.WUsers.ForEachWhere(x => link.SendPacket(LeafOutbound.LeafCustomName(link, x)),
                x => x.LoggedIn && !x.Quarantined && !String.IsNullOrEmpty(x.CustomName));

            link.SendPacket(LeafOutbound.LeafUserlistEnd());

            if (!link.Local)
                Events.Linked();
        }
    }
}
