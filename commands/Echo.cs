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
using iconnect;

namespace commands
{
    class Echo
    {
        private class Item
        {
            public Guid Guid { get; set; }
            public String Text { get; set; }
        }

        private static List<Item> list { get; set; }

        public static void Add(IUser client, String text)
        {
            Item i = list.Find(x => x.Guid.Equals(client.Guid));

            if (i != null)
                i.Text = text;
            else
                list.Add(new Item
                {
                    Guid = client.Guid,
                    Text = text
                });
        }

        public static void Remove(IUser client)
        {
            list.RemoveAll(x => x.Guid.Equals(client.Guid));
        }

        public static void Clear()
        {
            list = new List<Item>();
        }

        public static String IsEcho(IUser client)
        {
            Item i = list.Find(x => x.Guid.Equals(client.Guid));

            if (i != null)
                return i.Text;

            return null;
        }
    }
}
