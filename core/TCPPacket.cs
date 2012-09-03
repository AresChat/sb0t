using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    class TCPPacket
    {
        public TCPMsg Msg { get; set; }
        public TCPPacketReader Packet { get; set; }
    }
}
