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

                case LinkHub.LinkMsg.MSG_LINK_HUB_USERLIST_ITEM:
                    HubUserlistItem(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_LEAF_CONNECTED:
                    HubLeafConnected(link, packet);
                    break;

                case LinkHub.LinkMsg.MSG_LINK_HUB_LEAF_DISCONNECTED:
                    HubLeafDisconnected(link, packet);
                    break;
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
            Events.LinkLeafConnected(leaf);
        }

        private static void HubLeafDisconnected(LinkClient link, TCPPacketReader packet)
        {
            uint leaf_ident = packet;
            Leaf leaf = link.Leaves.Find(x => x.Ident == leaf_ident);

            if (leaf != null)
            {
                // part packets for all visible items in leaf.Users
                link.Leaves.RemoveAll(x => x.Ident == leaf_ident);
                Events.LinkLeafDisconnected(leaf);
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
                user.Name = packet.ReadString(link);
                user.OrgName = user.Name;
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
                leaf.Users.Add(user);
                // check if visible, and send join packet if required
            }
        }

        private static void Error(LinkClient link, TCPPacketReader packet)
        {
            ServerCore.Log("LINK ERROR: " + ((core.LinkHub.LinkError)(byte)packet));
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
                Events.LinkHubConnected();
        }


    }
}
