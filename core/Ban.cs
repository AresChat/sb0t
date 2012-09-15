using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace core
{
    class Ban
    {
        public String Name { get; set; }
        public String Version { get; set; }
        public Guid Guid { get; set; }
        public IPAddress ExternalIP { get; set; }
        public IPAddress LocalIP { get; set; }
        public ushort Port { get; set; }
    }
}
