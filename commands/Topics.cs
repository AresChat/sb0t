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
    class Topics
    {
        private static uint last = 0;

        private class Item
        {
            public ushort Vroom { get; set; }
            public String Text { get; set; }
        }

        private static List<Item> list { get; set; }

        public static void UpdateClock(uint time)
        {
            if (time > (last + 30))
            {
                last = time;

                if (Settings.Clock)
                    UpdateClock();
            }
        }

        public static void EnableClock()
        {
            last = Server.Time;
            Settings.Clock = true;
            UpdateClock();
        }

        public static void DisableClock()
        {
            Settings.Clock = false;
            String topic = Server.Chatroom.Topic;

            Server.Users.Ares(x =>
            {
                if (x.Vroom == 0)
                    x.Topic(topic);
            });

            Server.Users.Web(x =>
            {
                if (x.Vroom == 0)
                    x.Topic(topic);
            });
        }

        public static void ClockTo(IUser client)
        {
            DateTime dt = DateTime.Now;
            bool am = dt.Hour < 12;
            int h = dt.Hour;
            h = h > 12 ? (h - 12) : h;
            String clock = h == 0 ? "12:" : (h + ":");
            clock += dt.Minute > 9 ? (dt.Minute + " ") : ("0" + dt.Minute + " ");
            clock += am ? "AM" : "PM";
            String str = Server.Chatroom.Topic;
            String topic = Template.Text(Category.Clock, 0);
            topic = topic.Replace("+c", clock);
            topic = topic.Replace("+t", str);
            client.Topic(topic);
        }

        public static String ClockTopic
        {
            get
            {
                DateTime dt = DateTime.Now;
                bool am = dt.Hour < 12;
                int h = dt.Hour;
                h = h > 12 ? (h - 12) : h;
                String clock = h == 0 ? "12:" : (h + ":");
                clock += dt.Minute > 9 ? (dt.Minute + " ") : ("0" + dt.Minute + " ");
                clock += am ? "AM" : "PM";
                String str = Server.Chatroom.Topic;
                String topic = Template.Text(Category.Clock, 0);
                topic = topic.Replace("+c", clock);
                topic = topic.Replace("+t", str);
                return topic;
            }
        }

        private static void UpdateClock()
        {
            DateTime dt = DateTime.Now;
            bool am = dt.Hour < 12;
            int h = dt.Hour;
            h = h > 12 ? (h - 12) : h;
            String clock = h == 0 ? "12:" : (h + ":");
            clock += dt.Minute > 9 ? (dt.Minute + " ") : ("0" + dt.Minute + " ");
            clock += am ? "AM" : "PM";
            String str = Server.Chatroom.Topic;

            String topic = Template.Text(Category.Clock, 0);
            topic = topic.Replace("+c", clock);
            topic = topic.Replace("+t", str);

            Server.Users.Ares(x =>
            {
                if (x.Vroom == 0)
                    x.Topic(topic);
            });

            Server.Users.Web(x =>
            {
                if (x.Vroom == 0)
                    x.Topic(topic);
            });
        }

        public static void AddTopic(ushort vroom, String text)
        {
            list.RemoveAll(x => x.Vroom == vroom);
            list.Add(new Item { Vroom = vroom, Text = text });
            SaveTopics();
        }

        public static void RemTopic(ushort vroom)
        {
            list.RemoveAll(x => x.Vroom == vroom);
            SaveTopics();
        }

        public static String GetTopic(ushort vroom)
        {
            Item i = list.Find(x => x.Vroom == vroom);

            if (i != null)
                return i.Text;

            return null;
        }

        public static void LoadTopics()
        {
            last = Server.Time;
            list = new List<Item>();

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.PreserveWhitespace = true;
                xml.Load(Path.Combine(Server.DataPath, "topics.xml"));

                XmlNodeList nodes = xml.GetElementsByTagName("item");

                foreach (XmlElement e in nodes)
                    list.Add(new Item
                    {
                        Vroom = ushort.Parse(e.GetElementsByTagName("vroom")[0].InnerText),
                        Text = e.GetElementsByTagName("text")[0].InnerText
                    });
            }
            catch { }
        }

        private static void SaveTopics()
        {
            XmlDocument xml = new XmlDocument();
            xml.PreserveWhitespace = true;
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", null, null));
            XmlNode root = xml.AppendChild(xml.CreateElement("topics"));

            foreach (Item i in list)
            {
                XmlNode item = root.OwnerDocument.CreateNode(XmlNodeType.Element, "item", root.BaseURI);
                root.AppendChild(item);

                XmlNode vroom = item.OwnerDocument.CreateNode(XmlNodeType.Element, "vroom", item.BaseURI);
                item.AppendChild(vroom);
                vroom.InnerText = i.Vroom.ToString();

                XmlNode text = item.OwnerDocument.CreateNode(XmlNodeType.Element, "text", item.BaseURI);
                item.AppendChild(text);
                text.InnerText = i.Text;
            }

            try { xml.Save(Path.Combine(Server.DataPath, "topics.xml")); }
            catch { }
        }
    }
}
