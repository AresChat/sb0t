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
using iconnect;

namespace commands
{
    class PMBlocking
    {
        private static List<Guid> list { get; set; }

        public static bool IsBlocking(IUser client)
        {
            return list.FindIndex(x => x.Equals(client.Guid)) > -1;
        }

        public static void Add(IUser client)
        {
            if (list.FindIndex(x => x.Equals(client.Guid)) == -1)
            {
                list.Add(client.Guid);
                Update();
            }
        }

        public static void Remove(IUser client)
        {
            if (list.RemoveAll(x => x.Equals(client.Guid)) > 0)
                Update();
        }

        public static void Load()
        {
            list = new List<Guid>();

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.PreserveWhitespace = true;
                xml.Load(Path.Combine(Server.DataPath, "pmblock.xml"));

                XmlNodeList nodes = xml.GetElementsByTagName("item");

                foreach (XmlNode n in nodes)
                    list.Add(new Guid(n.InnerText));
            }
            catch { }
        }

        private static void Update()
        {
            XmlDocument xml = new XmlDocument();
            xml.PreserveWhitespace = true;
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", null, null));
            XmlNode root = xml.AppendChild(xml.CreateElement("pmblock"));

            foreach (Guid i in list)
            {
                XmlNode item = root.OwnerDocument.CreateNode(XmlNodeType.Element, "item", root.BaseURI);
                root.AppendChild(item);
                item.InnerText = i.ToString();
            }

            try { xml.Save(Path.Combine(Server.DataPath, "pmblock.xml")); }
            catch { }
        }
    }
}
