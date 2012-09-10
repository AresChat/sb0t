using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace core.ib0t
{
    class ib0tClient : IClient
    {
        public ushort ID { get; private set; }
        public IPAddress ExternalIP { get; set; }
        public String DNS { get; set; }
        public Guid Guid { get; set; }
        public ushort FileCount { get; set; }
        public ushort DataPort { get; set; }
        public IPAddress NodeIP { get; set; }
        public ushort NodePort { get; set; }
        public String Name { get; set; }
        public String OrgName { get; set; }
        public String Version { get; set; }
        public IPAddress LocalIP { get; set; }
        public bool Browsable { get; set; }
        public byte Age { get; set; }
        public byte Sex { get; set; }
        public byte Country { get; set; }
        public String Region { get; set; }
        public bool FastPing { get; set; }
        public Level Level { get; set; }
        public ushort Vroom { get; set; }
        public bool Ghosting { get; set; }
        public List<String> IgnoreList { get; set; }
        public Font Font { get; set; }
        public bool CustomClient { get; set; }
        public List<String> CustomClientTags { get; set; }
        public bool Muzzled { get; set; }
        public String CustomName { get; set; }
    }
}
