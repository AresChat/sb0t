using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace core.Udp
{
    class UdpNode
    {
        public IPAddress IP { get; set; }
        public ushort Port { get; set; }
        public int Ack { get; set; }
        public int Try { get; set; }
        public ulong LastConnect { get; set; }
        public ulong LastSentIPS { get; set; }

        public EndPoint EndPoint
        {
            get { return new IPEndPoint(this.IP, this.Port); }
        }
    }
}
