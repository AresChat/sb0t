/*
    sb0t ares chat server
    Copyright (C) 2017  AresChat

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
using iconnect;

namespace commands
{
    class AsnBans
    {
        private static List<uint> list { get; set; }

        public static void Add(uint asn)
        {
            list.Add(asn);
            Update();
        }

        public static void RemoveAsn(uint asn)
        {
            if (list.Contains(asn))
            {
                list.Remove(asn);
                Update();
            }
        }

        public static uint RemoveIndex(int index)
        {
            uint asn = 0;

            if(list.Count >= index)
            {
                asn = list[index];
                list.RemoveAt(index);
                Update();

                return asn;
            }

            return 0;
        }

        public static void List(IUser client)
        {
            int counter = 0;

            if (list.Count == 0)
                client.Print(Template.Text(Category.Notification, 1));

            foreach (var str in list)
                client.Print($"{counter++} - AS{str}");
        }

        public static void Clear()
        {
            list = new List<uint>();
            Update();
        }

        public static void Load()
        {
            list = new List<uint>();

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.PreserveWhitespace = true;
                xml.Load(Path.Combine(Server.DataPath, "asnbans.xml"));

                XmlNodeList nodes = xml.GetElementsByTagName("item");

                foreach (XmlNode n in nodes)
                    list.Add(uint.Parse(n.InnerText));
            }
            catch { }
        }

        private static void Update()
        {
            XmlDocument xml = new XmlDocument();
            xml.PreserveWhitespace = true;
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", null, null));
            XmlNode root = xml.AppendChild(xml.CreateElement("asnbans"));

            foreach (var i in list)
            {
                XmlNode item = root.OwnerDocument.CreateNode(XmlNodeType.Element, "item", root.BaseURI);
                root.AppendChild(item);
                item.InnerText = i.ToString();
            }

            try { xml.Save(Path.Combine(Server.DataPath, "asnbans.xml")); }
            catch { }
        }

        public static bool IsBanned(IUser client)
        {
            return list.Contains(client.GetASN());
        }
    }
}
