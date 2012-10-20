using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using iconnect;

namespace scripting
{
    class Hashlink : IHashlinkRoom
    {
        public String Name { get; set; }
        public IPAddress IP { get; set; }
        public ushort Port { get; set; }
    }
}
