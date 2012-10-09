using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace core.LinkLeaf
{
    class Leaf
    {
        public uint Ident { get; set; }
        public String Name { get; set; }
        public IPAddress ExternalIP { get; set; }
        public ushort Port { get; set; }
        public List<LinkUser> Users { get; set; }

        public Leaf()
        {
            this.Users = new List<LinkUser>();
        }
    }
}
