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
            packet.WriteUInt16(Settings.Port);
            return packet.ToAresPacket(UdpMsg.OP_SERVERLIST_WANTCHECKFIREWALL);
        }

        public static byte[] AddIps(IPAddress target_ip, ulong time)
        {
            UdpPacketWriter packet = new UdpPacketWriter();
            packet.WriteUInt16(Settings.Port);

            UdpNode[] servers = UdpNodeManager.GetServers(target_ip, 6, time);

            foreach (UdpNode s in servers)
            {
                packet.WriteIP(s.IP);
                packet.WriteUInt16(s.Port);
            }

            return packet.ToAresPacket(UdpMsg.OP_SERVERLIST_ADDIPS);
        }

        public static byte[] AckIps(IPAddress target_ip, ulong time)
        {
            UdpPacketWriter packet = new UdpPacketWriter();
            packet.WriteUInt16(Settings.Port);

            UdpNode[] servers = UdpNodeManager.GetServers(target_ip, 6, time);

            foreach (UdpNode s in servers)
            {
                packet.WriteIP(s.IP);
                packet.WriteUInt16(s.Port);
            }

            return packet.ToAresPacket(UdpMsg.OP_SERVERLIST_ACKIPS);
        }

        public static byte[] AckInfo(ulong time)
        {
            UdpPacketWriter packet = new UdpPacketWriter();
            packet.WriteUInt16(Settings.Port);
            packet.WriteUInt16(UserPool.UserCount);
            packet.WriteString(Settings.Name);
            packet.WriteString(Settings.Topic);
            packet.WriteByte(Settings.Language);
            packet.WriteString(Settings.VERSION);
            UdpNode[] servers = UdpNodeManager.GetServers(6, time);
            packet.WriteByte((byte)servers.Length);

            foreach (UdpNode s in servers)
            {
                packet.WriteIP(s.IP);
                packet.WriteUInt16(s.Port);
            }

            return packet.ToAresPacket(UdpMsg.OP_SERVERLIST_ACKINFO);
        }

        public static byte[] AckNodes(IPEndPoint[] nodes)
        {
            UdpPacketWriter packet = new UdpPacketWriter();
            packet.WriteUInt16(Settings.Port);

            foreach (IPEndPoint ep in nodes)
            {
                packet.WriteIP(ep.Address);
                packet.WriteUInt16((ushort)ep.Port);
            }

            return packet.ToAresPacket(UdpMsg.OP_SERVERLIST_ACKNODES);
        }

        public static byte[] ReadyCheckFirewall(FirewallTest fw)
        {
            UdpPacketWriter packet = new UdpPacketWriter();
            packet.WriteUInt32(fw.Cookie);
            packet.WriteIP(((IPEndPoint)fw.EndPoint).Address);
            return packet.ToAresPacket(UdpMsg.OP_SERVERLIST_READYTOCHECKFIREWALL);
        }

        public static byte[] CheckFirewallBusy(IPAddress target_ip, ulong time)
        {
            UdpPacketWriter packet = new UdpPacketWriter();
            packet.WriteUInt16(Settings.Port);

            UdpNode[] servers = UdpNodeManager.GetServers(target_ip, 6, time);

            foreach (UdpNode s in servers)
            {
                packet.WriteIP(s.IP);
                packet.WriteUInt16(s.Port);
            }

            return packet.ToAresPacket(UdpMsg.OP_SERVERLIST_CHECKFIREWALLBUSY);
        }

        public static byte[] ProceedCheckFirewall(uint cookie)
        {
            UdpPacketWriter packet = new UdpPacketWriter();
            packet.WriteUInt16(Settings.Port);
            packet.WriteUInt32(cookie);
            return packet.ToAresPacket(UdpMsg.OP_SERVERLIST_PROCEEDCHECKFIREWALL);
        }
    }
}
