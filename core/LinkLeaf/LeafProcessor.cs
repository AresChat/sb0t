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
                Events.LinkConnected();
        }


    }
}
