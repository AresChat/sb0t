using System;
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
                    HubAvatar(link, packet);
                    break;
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

                        if (user.Visible)
                            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Avatar(x, user)),
                                x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);
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

                        if (user.Visible)
                            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.PersonalMessage(x, user)),
                                x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);
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

                    if (user.Visible)
                    {
                        IClient other = null;

                        foreach (Leaf l in link.Leaves)
                            if (l.Ident != leaf.Ident)
                            {
                                other = l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom);

                                if (other != null)
                                {
                                    l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom).Visible = true;
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

                    if (user.Visible)
                    {
                        IClient other = null;

                        foreach (Leaf l in link.Leaves)
                            if (l.Ident != leaf.Ident)
                            {
                                other = l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom);

                                if (other != null)
                                {
                                    l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom).Visible = true;
                                    break;
                                }
                            }

                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(other == null ? TCPOutbound.Part(x, user) : TCPOutbound.UpdateUserStatus(x, other)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(other == null ? ib0t.WebOutbound.PartTo(x, user.Name) : ib0t.WebOutbound.UpdateTo(x, user.Name, user.Level)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);
                    }

                    user.SetName(new_name);
                    user.Visible = UserPool.AUsers.Find(x => x.LoggedIn && x.Name == user.Name && !x.Quarantined && x.Vroom == user.Vroom) == null;

                    if (user.Visible)
                        user.Visible = UserPool.WUsers.Find(x => x.LoggedIn && x.Name == user.Name && !x.Quarantined && x.Vroom == user.Vroom) == null;

                    if (user.Visible)
                        foreach (Leaf l in link.Leaves)
                            if (l.Ident != leaf.Ident)
                            {
                                user.Visible = l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom) == null;

                                if (!user.Visible)
                                    break;
                            }

                    if (user.Visible)
                    {
                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Join(x, user)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.JoinTo(x, user.Name, user.Level)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);
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

                    if (user.Visible)
                    {
                        IClient other = null;

                        foreach (Leaf l in link.Leaves)
                            if (l.Ident != leaf.Ident)
                            {
                                other = l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom);

                                if (other != null)
                                {
                                    l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom).Visible = true;
                                    break;
                                }
                            }

                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(other == null ? TCPOutbound.Part(x, user) : TCPOutbound.UpdateUserStatus(x, other)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(other == null ? ib0t.WebOutbound.PartTo(x, user.Name) : ib0t.WebOutbound.UpdateTo(x, user.Name, user.Level)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);
                    }

                    user.SetVroom(new_vroom);
                    user.Visible = UserPool.AUsers.Find(x => x.LoggedIn && x.Name == user.Name && !x.Quarantined && x.Vroom == user.Vroom) == null;

                    if (user.Visible)
                        user.Visible = UserPool.WUsers.Find(x => x.LoggedIn && x.Name == user.Name && !x.Quarantined && x.Vroom == user.Vroom) == null;

                    if (user.Visible)
                        foreach (Leaf l in link.Leaves)
                            if (l.Ident != leaf.Ident)
                            {
                                user.Visible = l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom) == null;

                                if (!user.Visible)
                                    break;
                            }

                    if (user.Visible)
                    {
                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Join(x, user)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.JoinTo(x, user.Name, user.Level)),
                            x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);
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

                    user.Muzzled = ((byte)packet) == 1;

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

                    if (user.Visible)
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
                    if (user.Visible)
                    {
                        IClient other = null;

                        foreach (Leaf l in link.Leaves)
                            if (l.Ident != leaf.Ident)
                            {
                                other = l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom);

                                if (other != null)
                                {
                                    l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom).Visible = true;
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
                user.Level = (iconnect.ILevel)(byte)packet;
                user.Vroom = packet;
                user.CustomClient = ((byte)packet) == 1;
                user.Muzzled = ((byte)packet) == 1;
                user.WebClient = ((byte)packet) == 1;
                user.Encrypted = ((byte)packet) == 1;
                user.Registered = ((byte)packet) == 1;
                user.Idle = ((byte)packet) == 1;
                user.IdleStart = Time.Now;
                user.Visible = UserPool.AUsers.Find(x => x.LoggedIn && x.Name == user.Name && !x.Quarantined && x.Vroom == user.Vroom) == null;

                if (user.Visible)
                    user.Visible = UserPool.WUsers.Find(x => x.LoggedIn && x.Name == user.Name && !x.Quarantined && x.Vroom == user.Vroom) == null;

                if (user.Visible)
                    foreach (Leaf l in link.Leaves)
                        if (l.Ident != leaf.Ident)
                        {
                            user.Visible = l.Users.Find(x => x.Name == user.Name && x.Vroom == user.Vroom) == null;

                            if (!user.Visible)
                                break;
                        }

                if (user.Visible)
                {
                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Join(x, user)),
                        x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);

                    UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.JoinTo(x, user.Name, user.Level)),
                        x => x.LoggedIn && x.Vroom == user.Vroom && !x.Quarantined);
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
