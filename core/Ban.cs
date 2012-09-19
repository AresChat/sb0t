using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using iconnect;

namespace core
{
    class Ban : IBan
    {
        public String Name { get; set; }
        public String Version { get; set; }
        public Guid Guid { get; set; }
        public IPAddress ExternalIP { get; set; }
        public IPAddress LocalIP { get; set; }
        public ushort Port { get; set; }
        public ushort Ident { get; set; }

        private bool Unbanned { get; set; }
        
        public void Unban()
        {
            if (!this.Unbanned)
            {
                this.Unbanned = true;
                BanSystem.RemoveBan(this.Ident);
            }
        }
    }
}
