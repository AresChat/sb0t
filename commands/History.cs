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
    class History
    {
        private class Item
        {
            public uint Time { get; set; }
            public String Name { get; set; }
            public String Text { get; set; }
            public ItemType Type { get; set; }
        }

        private enum ItemType { Public, Emote }

        private static List<Item> list { get; set; }

        public static void Reset()
        {
            list = new List<Item>();
        }

        public static void Add(String name, String text, bool emote)
        {
            list.Add(new Item
            {
                Name = name,
                Text = text,
                Time = Server.Time,
                Type = emote ? ItemType.Emote : ItemType.Public
            });

            if (list.Count > 20)
                list.RemoveAt(0);
        }

        public static void Show(IUser client)
        {
            if (list.Count > 0)
            {
                uint time = Server.Time;

                foreach (Item i in list)
                {
                    String text = "[-";
                    uint offset = (time - i.Time);

                    if (offset < 60)
                    {
                        text += "00:00:" +
                            (offset < 10 ? ("0" + offset) : offset.ToString());
                    }
                    else if (offset < 3600)
                    {
                        uint m = (offset / 60);
                        uint s = (offset - (m * 60));

                        text += "00:" +
                            (m < 10 ? ("0" + m) : m.ToString()) + ":" +
                            (s < 10 ? ("0" + s) : s.ToString());
                    }
                    else
                    {
                        uint m = (offset / 60);
                        uint s = (offset - (m * 60));
                        uint h = (m / 60);
                        m -= (h * 60);

                        text += (h < 10 ? ("0" + h) : h.ToString()) + ":" +
                            (m < 10 ? ("0" + m) : m.ToString()) + ":" +
                            (s < 10 ? ("0" + s) : s.ToString());
                    }

                    text += "] " + i.Text;

                    if (i.Type == ItemType.Public)
                        Server.PublicToTarget(client, i.Name, text);
                    else
                        Server.EmoteToTarget(client, i.Name, text);
                }

                client.Print(Template.Text(Category.Notification, 7));
            }
        }
    }
}
