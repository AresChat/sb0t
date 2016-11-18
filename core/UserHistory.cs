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

namespace core
{
    class UserHistory
    {
        public static List<PartedClient> list { get; set; }

        public static void Initialize()
        {
            list = new List<PartedClient>();
        }

        public static void AddUser(IClient client, ulong time)
        {
            list.RemoveAll(i => i.ExternalIP.Equals(client.ExternalIP) && i.Name == client.Name);
            
            list.Insert(0, new PartedClient
            {
                ExternalIP = client.ExternalIP,
                Name = client.Name,
                Time = time,
                DataPort = client.DataPort,
                DNS = client.DNS,
                Guid = client.Guid,
                JoinTime = Helpers.UnixTime,
                LocalIP  = client.LocalIP,
                Version = client.Version
            });
        }

        public static bool IsJoinFlooding(IClient client, ulong time)
        {
            return (from x in list
                    where x.ExternalIP.Equals(client.ExternalIP)
                    && (x.Time + 15000) > time
                    select x).Count() > 0;
        }
    }
}
