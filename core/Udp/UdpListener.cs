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
using System.Net.Sockets;

namespace core.Udp
{
    class UdpListener
    {
        public IPEndPoint EndPoint { get; private set; }
        public bool Showing { get; private set; }
        public bool ServerAddressReceived { get; set; }

        private Socket Sock { get; set; }
        private Queue<UdpItem> data_out = new Queue<UdpItem>();
        private Queue<UdpItem> data_in = new Queue<UdpItem>();
        private List<FirewallTest> firewall_tests = new List<FirewallTest>();
        private MyFirewallTester TcpTester { get; set; }
        private ulong Timer_1_Second { get; set; }
        private ulong Timer_1_Minute { get; set; }
        private ulong Timer_15_Minutes { get; set; }

        public UdpListener(IPEndPoint ep)
        {
            this.EndPoint = ep;
            this.TcpTester = new MyFirewallTester();
        }

        public void Start()
        {
            UdpStats.Reset();
            UdpNodeManager.Initialize();

            this.TcpTester.PopulateNodes();
            this.Showing = Settings.Get<bool>("udp");
            this.Timer_1_Second = Time.Now;
            this.Timer_1_Minute = Time.Now;
            this.Timer_15_Minutes = Time.Now;
            this.Sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.Sock.Blocking = false;
            this.Sock.Bind(this.EndPoint);
        }

        public void Stop()
        {
            try { this.Sock.Shutdown(SocketShutdown.Both); }
            catch { }
            try { this.Sock.Close(); }
            catch { }
            try { this.Sock.Dispose(); }
            catch { }
            try { this.Sock = null; }
            catch { }
        }

        public FirewallTest TestRemoteFirewall
        {
            get
            {
                if (this.firewall_tests.Count < 4)
                {
                    FirewallTest fw = new FirewallTest();
                    this.firewall_tests.Add(fw);
                    return fw;
                }

                return null;
            }
        }

        public void StartRemoteFirewallTest(uint cookie)
        {
            this.firewall_tests.ForEachWhere(x => x.Start(), x => x.Cookie == cookie);
        }

        public void SendDatagram(UdpItem item)
        {
            this.data_out.Enqueue(item);
        }

        private bool Pending
        {
            get { return this.Sock.Available > 0; }
        }

        public void AddChecker(IPAddress ip)
        {
            this.TcpTester.AddChecker(ip);
        }

        public bool IsTcpChecker(Socket sock)
        {
            IPAddress ip = ((IPEndPoint)sock.RemoteEndPoint).Address;

            if (this.TcpTester.IsChecker(ip))
            {
                try { sock.Disconnect(false); }
                catch { }
                try { sock.Shutdown(SocketShutdown.Both); }
                catch { }
                try { sock.Close(); }
                catch { }
                try { sock.Dispose(); }
                catch { }
                try { sock = null; }
                catch { }

                return true;
            }

            return false;
        }
        
        public void ServiceUdp(ulong time)
        {
            this.SendReceive();

            while (this.data_in.Count > 0)
                try
                {
                    UdpProcessor.Eval(this.data_in.Dequeue(), this, time);
                }
                catch { }

            if ((this.Timer_1_Second + 1000) < time)
            {
                this.Timer_1_Second = time;
                this.firewall_tests.ForEachWhere(x => x.Stop(), x => (x.Time + 10000) < time);
                this.firewall_tests.RemoveAll(x => x.Completed);

                if (this.TcpTester.IsTesting)
                    this.TcpTester.TestNext(this);
                else if (this.Showing)
                    this.Push(time);
            }

            if ((this.Timer_1_Minute + 60000) < time)
            {
                this.Timer_1_Minute = time;
                this.TcpTester.Timeout();
                UdpNodeManager.Expire(time);
            }

            if ((this.Timer_15_Minutes + 900000) < time)
            {
                this.Timer_15_Minutes = time;
                UdpNodeManager.Update(time);
                ServerCore.Log("local node list updated [" + UdpStats.SENDINFO + ":" + UdpStats.ACKINFO + ":" + UdpStats.ADDIPS + ":" + UdpStats.ACKIPS + "]");
                UdpStats.Reset();
            }
        }

        private void Push(ulong time)
        {
            UdpNode node = UdpNodeManager.NextPusher(time);

            if (node != null)
                this.SendDatagram(new UdpItem
                {
                    Data = UdpOutbound.AddIps(node.IP, time),
                    EndPoint = node.EndPoint,
                    Msg = UdpMsg.OP_SERVERLIST_ADDIPS
                });
        }

        private void SendReceive()
        {
            while (this.data_out.Count > 0)
            {
                UdpItem item = this.data_out.Peek();

                try
                {
                    int size = this.Sock.SendTo(item.Data, item.EndPoint);

                    if (size == item.Data.Length)
                    {
                        this.data_out.Dequeue();
                        UdpStats.Record(item.Msg);
                    }
                    else throw new Exception();
                }
                catch
                {
                    if (item.Attempts++ > 2)
                        this.data_out.Dequeue();
                }
            }

            EndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            byte[] buffer = new byte[8192];

            while (this.Pending)
            {
                try
                {
                    int size = this.Sock.ReceiveFrom(buffer, ref ep);

                    if (size > 0)
                    {
                        UdpItem item = new UdpItem();
                        item.Msg = (UdpMsg)buffer[0];
                        item.Data = buffer.Skip(1).Take(size).ToArray();
                        item.EndPoint = ep;
                        this.data_in.Enqueue(item);
                    }
                }
                catch { break; }
            }
        }

    }
}
