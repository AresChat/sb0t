using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace core.Udp
{
    class UdpChannelItem
    {
        public String Name { get; set; }
        public String Topic { get; set; }
        public String Version { get; set; }
        public ushort Users { get; set; }
        public ushort Port { get; set; }
        public IPAddress IP { get; set; }
        public byte Language { get; set; }
        public IPEndPoint[] Servers { get; set; }

        public UdpChannelItem(UdpNode addr)
        {
            this.IP = addr.IP;
            this.Port = addr.Port;
        }

        public UdpChannelItem(EndPoint addr, byte[] packet)
        {
            UdpPacketReader reader = new UdpPacketReader(packet.Skip(1).ToArray());
            this.IP = ((IPEndPoint)addr).Address;
            this.Port = reader;
            this.Users = reader;
            this.Name = reader;
            this.Topic = reader;
            this.Language = reader;
            this.Version = reader;
            byte count = reader;

            if (count > 0)
            {
                List<IPEndPoint> servers = new List<IPEndPoint>();

                for (int i = 0; i < count; i++)
                {
                    IPAddress ip = reader;
                    ushort port = reader;
                    servers.Add(new IPEndPoint(ip, port));
                }

                this.Servers = servers.ToArray();
            }
            else this.Servers = new IPEndPoint[] { };
        }

        public IPEndPoint ToEndPoint()
        {
            return new IPEndPoint(this.IP, this.Port);
        }
    }
}
