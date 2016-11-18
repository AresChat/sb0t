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
using System.IO;
using System.Xml;
using System.Net;
using iconnect;

namespace commands
{
    class Muzzles
    {
        private class Item
        {
            public IPAddress IP { get; set; }
            public Guid Guid { get; set; }
            public uint Time { get; set; }
            public String Name { get; set; }
        }

        private static List<Item> list { get; set; }

        public static void Tick(uint time)
        {
            uint target = (uint)(Settings.MuzzleTimeout * 60);

            if (target > 0)
                for (int i = (list.Count - 1); i > -1; i--)
                {
                    Item m = list[i];

                    if ((time - m.Time) > target)
                    {
                        list.RemoveAt(i);
                        Server.Print(Template.Text(Category.Timeouts, 1).Replace("+n", m.Name), true);

                        Server.Users.Ares(x =>
                        {
                            if (x.ExternalIP.Equals(m.IP) || x.Guid.Equals(m.Guid))
                                if (x.Muzzled)
                                    x.Muzzled = false;
                        });

                        Server.Users.Web(x =>
                        {
                            if (x.ExternalIP.Equals(m.IP) || x.Guid.Equals(m.Guid))
                                if (x.Muzzled)
                                    x.Muzzled = false;
                        });
                    }
                }
        }

        public static bool IsMuzzled(IUser client)
        {
            return list.Find(x => x.IP.Equals(client.ExternalIP) || x.Guid.Equals(client.Guid)) != null;
        }

        public static void Clear()
        {
            list = new List<Item>();
            Save();
            Server.Users.Ares(x => { if (x.Muzzled)x.Muzzled = false; });
            Server.Users.Web(x => { if (x.Muzzled)x.Muzzled = false; });
        }

        public static void Load()
        {
            list = new List<Item>();

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.PreserveWhitespace = true;
                xml.Load(Path.Combine(Server.DataPath, "muzzles.xml"));

                XmlNodeList nodes = xml.GetElementsByTagName("item");

                foreach (XmlElement e in nodes)
                    list.Add(new Item
                    {
                        IP = IPAddress.Parse(e.GetElementsByTagName("ip")[0].InnerText),
                        Guid = new Guid(e.GetElementsByTagName("guid")[0].InnerText),
                        Time = uint.Parse(e.GetElementsByTagName("time")[0].InnerText),
                        Name = e.GetElementsByTagName("name")[0].InnerText
                    });
            }
            catch { }
        }

        public static void AddMuzzle(IUser client)
        {
            if (list.Find(x => x.IP.Equals(client.ExternalIP) || x.Guid.Equals(client.Guid)) == null)
            {
                list.Add(new Item
                {
                    Guid = client.Guid,
                    IP = client.ExternalIP,
                    Time = Server.Time,
                    Name = client.Name
                });

                Save();
            }
        }

        public static void RemoveMuzzle(IUser client)
        {
            if (list.RemoveAll(x => x.IP.Equals(client.ExternalIP) || x.Guid.Equals(client.Guid)) > 0)
                Save();
        }

        private static void Save()
        {
            XmlDocument xml = new XmlDocument();
            xml.PreserveWhitespace = true;
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", null, null));
            XmlNode root = xml.AppendChild(xml.CreateElement("muzzles"));

            foreach (Item i in list)
            {
                XmlNode item = root.OwnerDocument.CreateNode(XmlNodeType.Element, "item", root.BaseURI);
                root.AppendChild(item);
                XmlNode ip = item.OwnerDocument.CreateNode(XmlNodeType.Element, "ip", item.BaseURI);
                item.AppendChild(ip);
                ip.InnerText = i.IP.ToString();
                XmlNode guid = item.OwnerDocument.CreateNode(XmlNodeType.Element, "guid", item.BaseURI);
                item.AppendChild(guid);
                guid.InnerText = i.Guid.ToString();
                XmlNode time = item.OwnerDocument.CreateNode(XmlNodeType.Element, "time", item.BaseURI);
                item.AppendChild(time);
                time.InnerText = i.Time.ToString();
                XmlNode name = item.OwnerDocument.CreateNode(XmlNodeType.Element, "name", item.BaseURI);
                item.AppendChild(name);
                name.InnerText = i.Name;
            }

            try { xml.Save(Path.Combine(Server.DataPath, "muzzles.xml")); }
            catch { }
        }

    }
}
