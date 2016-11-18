/*
    sb0t ares chat server
    Copyright (C) 2016  AresChat

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using iconnect;

namespace core.Udp
{
    class UdpChannelItem : IChannelItem
    {
        public String Name { get; set; }
        public String Topic { get; set; }
        public String Version { get; set; }
        public ushort Users { get; set; }
        public ushort Port { get; set; }
        public IPAddress IP { get; set; }
        public byte Language { get; set; }
        public IPEndPoint[] Servers { get; set; }

        public UdpChannelItem() { }

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
    }
}
