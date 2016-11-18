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
    public class FileFilter
    {
        public static void DoFilter(IUser client, String filename)
        {
            String ip = client.ExternalIP.ToString();

            foreach (Item item in list)
            {
                switch (item.Type)
                {
                    case FilterType.Announce:
                        if (filename.ToUpper().Contains(item.Trigger.ToUpper()))
                            client.Print(item.Args.Replace("+n", client.Name).Replace("+ip", ip));
                        break;

                    case FilterType.Ban:
                        if (filename.ToUpper().Contains(item.Trigger.ToUpper()))
                        {
                            Server.Print(Template.Text(Category.Filter, 6).Replace("+n", client.Name).Replace("+f", filename), true);
                            client.Ban();
                            return;
                        }
                        break;

                    case FilterType.Clone:
                        if (filename.ToUpper().Contains(item.Trigger.ToUpper()))
                            if (item.Args.StartsWith("/me "))
                                client.SendEmote(item.Args.Substring(4).Replace("+n", client.Name).Replace("+ip", ip));
                            else
                                client.SendText(item.Args.Replace("+n", client.Name).Replace("+ip", ip));
                        break;

                    case FilterType.Kill:
                        if (filename.ToUpper().Contains(item.Trigger.ToUpper()))
                        {
                            Server.Print(Template.Text(Category.Filter, 6).Replace("+n", client.Name).Replace("+f", filename), true);
                            client.Disconnect();
                            return;
                        }
                        break;

                    case FilterType.PM:
                        if (filename.ToUpper().Contains(item.Trigger.ToUpper()))
                            client.PM(Server.Chatroom.BotName, item.Args.Replace("+n", client.Name).Replace("+ip", ip));
                        break;
                }
            }
        }

        private class Item
        {
            public String Trigger { get; set; }
            public FilterType Type { get; set; }
            public String Args { get; set; }
        }

        private static List<Item> list { get; set; }

        public static void Load()
        {
            list = new List<Item>();

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.PreserveWhitespace = true;
                xml.Load(Path.Combine(Server.DataPath, "filefilters.xml"));

                XmlNodeList nodes = xml.GetElementsByTagName("item");

                foreach (XmlElement e in nodes)
                    list.Add(new Item
                    {
                        Trigger = e.GetElementsByTagName("trigger")[0].InnerText,
                        Type = (FilterType)Enum.Parse(typeof(FilterType), e.GetElementsByTagName("type")[0].InnerText),
                        Args = e.GetElementsByTagName("args")[0].InnerText
                    });
            }
            catch { }
        }

        private static void Save()
        {
            XmlDocument xml = new XmlDocument();
            xml.PreserveWhitespace = true;
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", null, null));
            XmlNode root = xml.AppendChild(xml.CreateElement("filefilters"));

            foreach (Item i in list)
            {
                XmlNode item = root.OwnerDocument.CreateNode(XmlNodeType.Element, "item", root.BaseURI);
                root.AppendChild(item);

                XmlNode trigger = item.OwnerDocument.CreateNode(XmlNodeType.Element, "trigger", item.BaseURI);
                item.AppendChild(trigger);
                trigger.InnerText = i.Trigger;

                XmlNode type = item.OwnerDocument.CreateNode(XmlNodeType.Element, "type", item.BaseURI);
                item.AppendChild(type);
                type.InnerText = i.Type.ToString();

                XmlNode args = item.OwnerDocument.CreateNode(XmlNodeType.Element, "args", item.BaseURI);
                item.AppendChild(args);
                args.InnerText = i.Args;
            }

            try { xml.Save(Path.Combine(Server.DataPath, "filefilters.xml")); }
            catch { }
        }

        private enum FilterType
        {
            // without args
            Kill,
            Ban,

            // with args
            PM,
            Announce,
            Clone
        }

        public static void Add(IUser admin, String args)
        {
            String[] split = args.Split(new String[] { ", " }, StringSplitOptions.None);

            if (split.Length < 2)
                return;

            Item item = new Item();
            item.Trigger = split[0];
            item.Args = String.Empty;
            String[] types = Enum.GetNames(typeof(FilterType));

            if (item.Trigger.Length > 0)
                for (int i = 0; i < types.Length; i++)
                    if (types[i].ToUpper() == split[1].ToUpper())
                    {
                        if (i > 1)
                            if (split.Length > 1)
                            {
                                item.Args = String.Join(", ", new List<String>(split).GetRange(2, (split.Length - 2)).ToArray());

                                if (item.Args.Length == 0)
                                    break;
                            }
                            else break;
                        else if (split.Length != 2)
                            break;

                        item.Type = (FilterType)Enum.Parse(typeof(FilterType), types[i], true);
                        list.RemoveAll(x => x.Trigger == item.Trigger);
                        list.Add(item);
                        Save();

                        Server.Print(Template.Text(Category.Filter, 3).Replace("+t", item.Trigger).Replace("+f",
                            item.Type.ToString()).Replace("+n", Settings.Stealth ? Server.Chatroom.BotName : admin.Name), true);

                        break;
                    }
        }

        public static void Add(String args, bool save)
        {
            if (list == null)
                list = new List<Item>();

            String[] split = args.Split(new String[] { ", " }, StringSplitOptions.None);

            if (split.Length < 2)
                return;

            Item item = new Item();
            item.Trigger = split[0];
            item.Args = String.Empty;
            String[] types = Enum.GetNames(typeof(FilterType));

            if (item.Trigger.Length > 0)
                for (int i = 0; i < types.Length; i++)
                    if (types[i].ToUpper() == split[1].ToUpper())
                    {
                        if (i > 1)
                            if (split.Length > 1)
                            {
                                item.Args = String.Join(", ", new List<String>(split).GetRange(2, (split.Length - 2)).ToArray());

                                if (item.Args.Length == 0)
                                    break;
                            }
                            else break;
                        else if (split.Length != 2)
                            break;

                        item.Type = (FilterType)Enum.Parse(typeof(FilterType), types[i], true);
                        list.RemoveAll(x => x.Trigger == item.Trigger);
                        list.Add(item);

                        if (save)
                            Save();

                        break;
                    }
        }

        public static void Remove(IUser admin, int index)
        {
            if (index >= 0 && index < list.Count)
            {
                Item item = list[index];
                list.RemoveAt(index);
                Save();

                Server.Print(Template.Text(Category.Filter, 4).Replace("+t",
                    item.Trigger).Replace("+n", Settings.Stealth ? Server.Chatroom.BotName : admin.Name), true);
            }
        }

        public static void View(IUser admin)
        {
            if (list.Count == 0)
                admin.Print(Template.Text(Category.Filter, 2));
            else
            {
                for (int i = 0; i < list.Count; i++)
                    admin.Print(i + " - " + list[i].Trigger + " [" + list[i].Type + "]");
            }
        }
    }
}
