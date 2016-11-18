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

namespace commands
{
    class BanStats
    {
        private class Item
        {
            public String Name { get; set; }
            public IPAddress IP { get; set; }
            public String Banner { get; set; }
        }

        private static List<Item> list { get; set; }

        public static void Reset()
        {
            list = new List<Item>();
        }

        public static void Add(IUser admin, IUser target)
        {
            list.Add(new Item
            {
                Banner = admin.Name,
                IP = target.ExternalIP,
                Name = target.Name
            });
        }

        public static void View(IUser client)
        {
            foreach (Item i in list)
                client.Print(Template.Text(Category.BanStats, 0).Replace("+n", i.Name).Replace("+a", i.Banner).Replace("+ip", i.IP.ToString()));
        }
    }
}
