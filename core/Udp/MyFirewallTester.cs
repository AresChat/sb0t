using System;
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
            this.IsTesting = false;
            this.checking_me.Clear();
            ServerCore.Log("TCP firewall test failed");
        }
    }
}
