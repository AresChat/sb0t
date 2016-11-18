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
    class AvatarPMManager
    {
        private static List<Guid> avatars { get; set; }
        private static List<Item> pms { get; set; }

        private class Item
        {
            public Guid Guid { get; set; }
            public String Text { get; set; }
        }

        public static void Reset()
        {
            avatars = new List<Guid>();
            pms = new List<Item>();
        }

        public static void AddPM(IUser client, String msg)
        {
            int i = pms.FindIndex(x => x.Equals(client.Guid));

            if (i == -1)
                pms.Add(new Item
                {
                    Guid = client.Guid,
                    Text = msg
                });
            else pms[i].Text = msg;
        }

        public static void AddAvatar(IUser client)
        {
            if (avatars.FindIndex(x => x.Equals(client.Guid)) == -1)
                avatars.Add(client.Guid);
        }

        public static String GetPM(IUser client)
        {
            Item i = pms.Find(x => x.Guid.Equals(client.Guid));

            if (i == null)
                return null;

            return i.Text;
        }

        public static bool CanAvatar(IUser client)
        {
            return avatars.FindIndex(x => x.Equals(client.Guid)) == -1;
        }
    }
}
