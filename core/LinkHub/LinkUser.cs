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
        public uint Ident { get; private set; }

        public LinkUser(uint ident)
        {
            this.Ident = ident;
        }
    }
}
