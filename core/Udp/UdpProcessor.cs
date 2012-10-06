using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace core.Udp
{
    class UdpProcessor
    {
        public static void Eval(UdpItem item, UdpListener udp, ulong time)
        {
            switch (item.Msg)
            {
                case UdpMsg.OP_SERVERLIST_SENDINFO:
                    break;

                case UdpMsg.OP_SERVERLIST_ADDIPS:
                    break;

                case UdpMsg.OP_SERVERLIST_ACKIPS:
                    break;

                case UdpMsg.OP_SERVERLIST_SENDNODES:
                    break;

                case UdpMsg.OP_SERVERLIST_WANTCHECKFIREWALL:
                    break;

                case UdpMsg.OP_SERVERLIST_READYTOCHECKFIREWALL:
                    break;

                case UdpMsg.OP_SERVERLIST_PROCEEDCHECKFIREWALL:
                    break;

                case UdpMsg.OP_SERVERLIST_CHECKFIREWALLBUSY:
                    break;
            }
        }

        private static void SendInfo(UdpItem item, UdpListener udp, ulong time)
        {
            if (udp.Showing)
            {
                UdpStats.SENDINFO++;

                udp.SendDatagram(new UdpItem
                {
                    Data = UdpOutbound.AckInfo(time),
                    EndPoint = item.EndPoint,
                    Msg = UdpMsg.OP_SERVERLIST_ACKINFO
                });
            }
        }

        private static void AddIps(UdpItem item, UdpListener udp, ulong time)
        {
            UdpPacketReader packet = new UdpPacketReader(item.Data);
            ushort port = packet;
            UdpNode node = UdpNodeManager.Find(x => x.IP.Equals(((IPEndPoint)item.EndPoint).Address));

            if (node != null)
                node.Port = port;
            else
                UdpNodeManager.Add(item.EndPoint);

            while (packet.Remaining > 5)
            {
                UdpNode n = new UdpNode();
                n.IP = packet;
                n.Port = packet;
                UdpNodeManager.Add(n);
            }

            udp.SendDatagram(new UdpItem
            {
                Data = UdpOutbound.AckIps(((IPEndPoint)item.EndPoint).Address, time),
                EndPoint = item.EndPoint,
                Msg = UdpMsg.OP_SERVERLIST_ACKIPS
            });
        }

        private static void AckIps(UdpItem item, UdpListener udp, ulong time)
        {
            UdpStats.ACKIPS++;
            UdpPacketReader packet = new UdpPacketReader(item.Data);
            ushort port = packet;
            UdpNode node = UdpNodeManager.Find(x => x.IP.Equals(((IPEndPoint)item.EndPoint).Address));

            if (node != null)
            {
                node.Port = port;
                node.Ack++;
                node.LastConnect = time;
                node.Try = 0;
            }

            while (packet.Remaining > 5)
            {
                UdpNode n = new UdpNode();
                n.IP = packet;
                n.Port = packet;
                UdpNodeManager.Add(n);
            }
        }

        private static void SendNodes(UdpItem item, UdpListener udp, ulong time)
        {
            var linq = from x in UserPool.AUsers
                       where x.NodePort > 0 && x.Version.StartsWith("Ares 2.")
                       select new IPEndPoint(x.ExternalIP, x.DataPort);

            if (linq.Count() > 0)
            {
                List<IPEndPoint> nodes = linq.ToList();
                nodes.Randomize();

                if (nodes.Count > 20)
                    nodes = nodes.GetRange(0, 20);

                udp.SendDatagram(new UdpItem
                {
                    Data = UdpOutbound.AckNodes(nodes.ToArray()),
                    EndPoint = item.EndPoint,
                    Msg = UdpMsg.OP_SERVERLIST_ACKNODES
                });
            }
        }

        private static void WantCheckFirewall(UdpItem item, UdpListener udp, ulong time)
        {
            UdpPacketReader packet = new UdpPacketReader(item.Data);
            ushort port = packet;
            FirewallTest fw = udp.TestRemoteFirewall;

            if (fw != null)
            {
                fw.Cookie = AccountManager.NextCookie;
                fw.EndPoint = item.EndPoint;
                fw.Time = time;

                udp.SendDatagram(new UdpItem
                {
                    Data = UdpOutbound.ReadyCheckFirewall(fw),
                    EndPoint = item.EndPoint,
                    Msg = UdpMsg.OP_SERVERLIST_READYTOCHECKFIREWALL
                });
            }
            else udp.SendDatagram(new UdpItem
            {
                Data = UdpOutbound.CheckFirewallBusy(((IPEndPoint)item.EndPoint).Address, time),
                EndPoint = item.EndPoint,
                Msg = UdpMsg.OP_SERVERLIST_CHECKFIREWALLBUSY
            });
        }

        private static void ReadyToCheckFirewall(UdpItem item, UdpListener udp, ulong time)
        {
            UdpPacketReader packet = new UdpPacketReader(item.Data);
            uint cookie = packet;

            if (!udp.ServerAddressReceived)
            {
                udp.ServerAddressReceived = true;
                Settings.ExternalIP = packet;
                ServerCore.Log("server address reported as " + Settings.ExternalIP);
            }

            udp.SendDatagram(new UdpItem
            {
                Data = UdpOutbound.ProceedCheckFirewall(cookie),
                EndPoint = item.EndPoint,
                Msg = UdpMsg.OP_SERVERLIST_PROCEEDCHECKFIREWALL
            });
        }

        private static void ProceedCheckFirewall(UdpItem item, UdpListener udp, ulong time)
        {
            UdpPacketReader packet = new UdpPacketReader(item.Data);
            ushort port = packet;
            uint cookie = packet;
            udp.StartRemoteFirewallTest(cookie);
        }

        private static void CheckFirewallBusy(UdpItem item, UdpListener udp, ulong time)
        {
            UdpPacketReader packet = new UdpPacketReader(item.Data);
            ushort port = packet;

            while (packet.Remaining > 5)
            {
                UdpNode n = new UdpNode();
                n.IP = packet;
                n.Port = packet;
                UdpNodeManager.Add(n);
            }
        }
    }
}
