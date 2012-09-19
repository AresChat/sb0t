using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using iconnect;

namespace core
{
    class Room : IHashlinkRoom
    {
        public String Name { get; set; }
        public ushort Port { get; set; }
        public IPAddress IP { get; set; }
    }
}
