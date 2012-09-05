using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace core
{
    class PartedClient
    {
        public ulong Time { get; set; }
        public String Name { get; set; }
        public IPAddress ExternalIP { get; set; }
    }
}
