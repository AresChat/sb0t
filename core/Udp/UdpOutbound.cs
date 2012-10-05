using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace core.Udp
{
    class UdpOutbound
    {
        public static byte[] WantCheckFirewall()
        {
            UdpPacketWriter packet = new UdpPacketWriter();
            packet.WriteInt16(Settings.Port);
            return packet.ToAresPacket(UdpMsg.OP_SERVERLIST_WANTCHECKFIREWALL);
        }

        public static byte[] AddIps(IPAddress target_ip, ulong time)
        {
            UdpPacketWriter packet = new UdpPacketWriter();
            packet.WriteInt16(Settings.Port);

            UdpNode[] servers = UdpNodeManager.GetServers(target_ip, 6, time);

            foreach (UdpNode s in servers)
            {
                packet.WriteIP(s.IP);
                packet.WriteInt16(s.Port);
            }

            return packet.ToAresPacket(UdpMsg.OP_SERVERLIST_ADDIPS);
        }
    }
}
