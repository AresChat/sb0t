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
    class Greets
    {
        private static List<String> list { get; set; }

        public static void Add(String str)
        {
            if (!String.IsNullOrEmpty(str))
                if (list.Find(x => x == str) == null)
                {
                    list.Add(str);
                    Update();
                }
        }

        public static String Remove(int index)
        {
            if (index < 0 || index >= list.Count)
                return null;

            String str = list[index];
            list.RemoveAt(index);
            Update();
            return str;
        }

        public static void List(IUser client)
        {
            if (list.Count == 0)
                client.Print(Template.Text(Category.Greetings, 3));

            for (int i = 0; i < list.Count; i++)
                client.Print(i + " - " + list[i]);
        }

        private static int greet_index;

        public static String GetGreet(IUser client)
        {
            if (list.Count > 0)
            {
                if (greet_index >= list.Count)
                    greet_index = 0;

                String s = list[greet_index++];
                String _str = Helpers.CountryCodeToString(client.Country);
                List<String> list1 = new List<String>();
                Server.Users.Ares(x => list1.Add(x.Name));
                Server.Users.Web(x => list1.Add(x.Name));
                list1.RemoveAll(x => x == client.Name);
                String rnd_user = client.Name;

                if (list1.Count > 0)
                {
                    int index = (int)Math.Floor(new Random().NextDouble() * list1.Count);
                    rnd_user = list1[index];
                }

                if (_str != "?")
                    if (client.Region.Length > 0)
                        _str = client.Region + ", " + _str;

                if (_str == "?")
                    if (client.Region.Length > 0)
                        _str = client.Region;

                if (_str == "?")
                    _str = "unknown";

                s = s.Replace("+n", client.Name);
                s = s.Replace("+ip", client.ExternalIP.ToString());
                s = s.Replace("+id", client.ID.ToString());
                s = s.Replace("+f", client.FileCount.ToString());
                s = s.Replace("+v", client.Version);
                s = s.Replace("+p", client.DataPort.ToString());
                s = s.Replace("+uc", Server.Stats.CurrentUserCount.ToString());
                s = s.Replace("+rn", Server.Chatroom.Name);
                s = s.Replace("+ut", Helpers.GetUptime);
                s = s.Replace("+ru", rnd_user);
                s = s.Replace("+l", _str);

                return s;
            }

            return "hello " + client.Name;
        }

        public static void Load()
        {
            greet_index = 0;
            list = new List<String>();

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.PreserveWhitespace = true;
                xml.Load(Path.Combine(Server.DataPath, "greetings.xml"));

                XmlNodeList nodes = xml.GetElementsByTagName("item");

                foreach (XmlNode n in nodes)
                    list.Add(n.InnerText);
            }
            catch { }
        }

        private static void Update()
        {
            XmlDocument xml = new XmlDocument();
            xml.PreserveWhitespace = true;
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", null, null));
            XmlNode root = xml.AppendChild(xml.CreateElement("greetings"));

            foreach (String i in list)
            {
                XmlNode item = root.OwnerDocument.CreateNode(XmlNodeType.Element, "item", root.BaseURI);
                root.AppendChild(item);
                item.InnerText = i;
            }

            try { xml.Save(Path.Combine(Server.DataPath, "greetings.xml")); }
            catch { }
        }

        private static String _pm { get; set; }

        public static void SetPM(String text)
        {
            _pm = text;
            Settings.PMGreetMsgText = _pm;
        }

        public static String GetPM(IUser client)
        {
            if (String.IsNullOrEmpty(_pm))
            {
                _pm = Settings.PMGreetMsgText;

                if (String.IsNullOrEmpty(_pm))
                    SetPM("welcome to my room +n");
            }

            String _str = Helpers.CountryCodeToString(client.Country);
            List<String> list1 = new List<String>();
            Server.Users.Ares(x => list1.Add(x.Name));
            Server.Users.Web(x => list1.Add(x.Name));
            list1.RemoveAll(x => x == client.Name);
            String rnd_user = client.Name;

            if (list1.Count > 0)
            {
                int index = (int)Math.Floor(new Random().NextDouble() * list1.Count);
                rnd_user = list1[index];
            }

            if (_str != "?")
                if (client.Region.Length > 0)
                    _str = client.Region + ", " + _str;

            if (_str == "?")
                if (client.Region.Length > 0)
                    _str = client.Region;

            if (_str == "?")
                _str = "unknown";

            String s = _pm;
            s = s.Replace("+n", client.Name);
            s = s.Replace("+ip", client.ExternalIP.ToString());
            s = s.Replace("+id", client.ID.ToString());
            s = s.Replace("+f", client.FileCount.ToString());
            s = s.Replace("+v", client.Version);
            s = s.Replace("+p", client.DataPort.ToString());
            s = s.Replace("+uc", Server.Stats.CurrentUserCount.ToString());
            s = s.Replace("+rn", Server.Chatroom.Name);
            s = s.Replace("+ut", Helpers.GetUptime);
            s = s.Replace("+ru", rnd_user);
            s = s.Replace("+l", _str);
            return s;
        }
    }
}
