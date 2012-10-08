using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.LinkHub
{
    class LinkPacket
    {
        public LinkMsg Msg { get; set; }
        public TCPPacketReader Packet { get; set; }
    }
}
