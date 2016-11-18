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

namespace core
{
    class IdleManager
    {
        private static List<Idle> items { get; set; }

        public static void Reset()
        {
            if (items == null)
                items = new List<Idle>();

            items.Clear();
        }

        public static void Add(IClient client)
        {
            items.RemoveAll(x => x.Name == client.Name &&
                x.IP.Equals(client.ExternalIP) && x.Guid.Equals(client.Guid));

            client.Idled = true;
            client.IdleStart = Time.Now;

            items.Add(new Idle
            {
                Guid = client.Guid,
                IP = client.ExternalIP,
                Name = client.Name,
                Time = client.IdleStart
            });
        }

        public static void Remove(IClient client)
        {
            items.RemoveAll(x => x.Name == client.Name &&
                x.IP.Equals(client.ExternalIP) && x.Guid.Equals(client.Guid));

            client.Idled = false;
        }

        public static void Set(IClient client)
        {
            Idle i = items.Find(x => x.Name == client.Name &&
                x.IP.Equals(client.ExternalIP) && x.Guid.Equals(client.Guid));

            if (i != null)
            {
                client.Idled = true;
                client.IdleStart = i.Time;
            }
        }

        public static bool CheckIfCanIdle(IClient client)
        {
            // user is already idle
            if (client.Idled)
            {
                return false;
            }

            // last idle was less than 5 minutes ago don't let them idle again
            if(client.IdleStart > 0 && ((Time.Now - client.IdleStart) / 1000) < 5 * 60)
            {
                return false;
            }

            return true;
        }
    }

    class Idle
    {
        public String Name { get; set; }
        public IPAddress IP { get; set; }
        public Guid Guid { get; set; }
        public ulong Time { get; set; }
    }
}
