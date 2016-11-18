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
    class MyFirewallTester
    {
        private List<IPAddress> checking_me = new List<IPAddress>();
        private Queue<EndPoint> nodes = new Queue<EndPoint>();
        private bool checking_now { get; set; }

        public bool IsTesting { get; private set; }

        public void PopulateNodes()
        {
            foreach (UdpNode node in UdpNodeManager.GetServers())
                this.nodes.Enqueue(node.EndPoint);
        }

        public void AddChecker(IPAddress ip)
        {
            this.checking_me.Add(ip);
        }

        public MyFirewallTester()
        {
            this.IsTesting = true;
        }

        public void TestNext(UdpListener udp)
        {
            if (this.nodes.Count > 0)
                udp.SendDatagram(new UdpItem
                {
                    Data = UdpOutbound.WantCheckFirewall(),
                    EndPoint = this.nodes.Dequeue(),
                    Msg = UdpMsg.OP_SERVERLIST_WANTCHECKFIREWALL
                });
        }

        public bool IsChecker(IPAddress ip)
        {
            if (!this.IsTesting)
                return false;

            if (this.checking_me.Find(x => x.Equals(ip)) != null)
            {
                this.IsTesting = false;
                this.checking_me.Clear();
                ServerCore.Log("TCP firewall test succeeded");
                return true;
            }

            return false;
        }

        public void Timeout()
        {
            if (this.IsTesting)
            {
                this.IsTesting = false;
                this.checking_me.Clear();
                ServerCore.Log("TCP firewall test failed");
            }
        }
    }
}
