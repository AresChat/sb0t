using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using iconnect;

namespace core
{
    class PartedClient : IRecord
    {
        public ulong Time { get; set; }
        public String Name { get; set; }
        public IPAddress ExternalIP { get; set; }
        public IPAddress LocalIP { get; set; }
        public String Version { get; set; }
        public ushort DataPort { get; set; }
        public Guid Guid { get; set; }
        public String DNS { get; set; }
        public uint JoinTime { get; set; }
    }
}
