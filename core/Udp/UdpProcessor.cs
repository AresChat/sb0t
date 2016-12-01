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

namespace core.Udp
{
    class UdpProcessor
    {
        public static void Eval(UdpItem item, UdpListener udp, ulong time)
        {
            switch (item.Msg)
            {
                case UdpMsg.OP_SERVERLIST_SENDINFO:
                    SendInfo(item, udp, time);
                    break;

                case UdpMsg.OP_SERVERLIST_ADDIPS:
                    AddIps(item, udp, time);
                    break;

                case UdpMsg.OP_SERVERLIST_ACKIPS:
                    AckIps(item, udp, time);
                    break;

                case UdpMsg.OP_SERVERLIST_SENDNODES:
                    SendNodes(item, udp, time);
                    break;

                case UdpMsg.OP_SERVERLIST_WANTCHECKFIREWALL:
                    WantCheckFirewall(item, udp, time);
                    break;

                case UdpMsg.OP_SERVERLIST_READYTOCHECKFIREWALL:
                    ReadyToCheckFirewall(item, udp, time);
                    break;

                case UdpMsg.OP_SERVERLIST_PROCEEDCHECKFIREWALL:
                    ProceedCheckFirewall(item, udp, time);
                    break;

                case UdpMsg.OP_SERVERLIST_CHECKFIREWALLBUSY:
                    CheckFirewallBusy(item, udp, time);
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
            {
                UdpNodeManager.Add(item.EndPoint);
            }

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
                       where x.NodePort > 0 && (x.Version.StartsWith("Ares 2.") || x.Version.StartsWith("Ares_2."))
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

            udp.AddChecker(((IPEndPoint)item.EndPoint).Address);
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
