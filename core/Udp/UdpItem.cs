using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace core.Udp
{
    class UdpItem
    {
        public byte[] Data { get; set; }
        public EndPoint EndPoint { get; set; }
        public uint Attempts { get; set; }
        public UdpMsg Msg { get; set; }
    }
}
