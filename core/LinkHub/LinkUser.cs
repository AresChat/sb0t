using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using iconnect;

namespace core.LinkHub
{
    class LinkUser
    {
        public String OrgName { get; set; }
        public String Name { get; set; }
        public String Version { get; set; }
        public Guid Guid { get; set; }
        public ushort FileCount { get; set; }
        public IPAddress ExternalIP { get; set; }
        public IPAddress LocalIP { get; set; }
        public ushort Port { get; set; }
        public String DNS { get; set; }
        public bool Browsable { get; set; }
        public byte Age { get; set; }
        public byte Sex { get; set; }
        public byte Country { get; set; }
        public String Region { get; set; }
        public ILevel Level { get; set; }
        public ushort Vroom { get; set; }
        public bool CustomClient { get; set; }
        public bool Muzzled { get; set; }
        public bool WebClient { get; set; }
        public bool Encrypted { get; set; }
        public bool Registered { get; set; }
        public bool Idle { get; set; }
        public String CustomName { get; set; }
        public String PersonalMessage { get; set; }
        public byte[] Avatar { get; set; }
    }
}
