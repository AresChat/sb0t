using System;
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
                    break;

                case LinkMsg.MSG_LINK_LEAF_LOGIN:
                    LeafLogin(leaf, packet, time, mode);
                    break;

                case LinkMsg.MSG_LINK_LEAF_PERSONAL_MESSAGE:
                    LeafPersonalMessage(leaf, packet);
                    break;

                case LinkMsg.MSG_LINK_LEAF_PRIVATE_IGNORED:
                    break;

                case LinkMsg.MSG_LINK_LEAF_PRIVATE_TEXT:
                    break;

                case LinkMsg.MSG_LINK_LEAF_PUBLIC_TEXT:
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
            }
        }

        private static void LeafJoin(Leaf leaf, TCPPacketReader packet)
        {
            if (leaf.LoginPhase != LinkLogin.Ready)
            {
                leaf.SendPacket(HubOutbound.LinkError(LinkError.BadProtocol));
                leaf.Disconnect();
                return;
            }

            LinkUser user = new LinkUser();
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

            LinkUser user = new LinkUser();
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
            TrustedLeafItem item = TrustedLeavesManager.GetTrusted(credentials);

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
            leaf.Protocol = packet;
            leaf.Port = packet;

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
