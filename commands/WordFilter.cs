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
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using iconnect;

namespace commands
{
    public class WordFilter
    {
        public static void FilterPM(IUser client, IPrivateMsg msg)
        {
            String ip = client.ExternalIP.ToString();

            foreach (Item item in list)
                if (msg.Contains(item.Trigger))
                    switch (item.Type)
                    {
                        case FilterType.Ban:
                            if (client.Level == ILevel.Regular)
                            {
                                if (!client.Link.IsLinked)
                                {
                                    Server.Print(Template.Text(Category.Filter, 16).Replace("+n", client.Name).Replace("+ip", ip), true);
                                    client.Ban();
                                }

                                msg.Cancel = true;
                                return;
                            }
                            break;

                        case FilterType.Kill:
                            if (client.Level == ILevel.Regular)
                            {
                                if (!client.Link.IsLinked)
                                {
                                    Server.Print(Template.Text(Category.Filter, 15).Replace("+n", client.Name).Replace("+ip", ip), true);
                                    client.Disconnect();
                                }

                                msg.Cancel = true;
                                return;
                            }
                            break;

                        case FilterType.Redirect:
                            if (client.Level == ILevel.Regular)
                            {
                                if (!client.Link.IsLinked)
                                {
                                    Server.Print(Template.Text(Category.Filter, 17).Replace("+n", client.Name).Replace("+ip", ip), true);
                                    client.Redirect(item.Args);
                                }

                                msg.Cancel = true;
                                return;
                            }
                            break;
                    }
        }

        public static String FilterBefore(IUser client, String text)
        {
            String ip = client.ExternalIP.ToString();

            foreach (Item item in list)
                if (text.ToUpper().Contains(item.Trigger.ToUpper()))
                    switch (item.Type)
                    {
                        case FilterType.Muzzle:
                            if (client.Link.IsLinked && client.Level == ILevel.Regular)
                                return String.Empty;
                            else if (client.Level == ILevel.Regular)
                            {
                                Server.Print(Template.Text(Category.Filter, 9).Replace("+n", client.Name), true);
                                client.Muzzled = true;
                                return String.Empty;
                            }
                            break;

                        case FilterType.Kill:
                            if (client.Link.IsLinked && client.Level == ILevel.Regular)
                                return String.Empty;
                            else if (client.Level == ILevel.Regular)
                            {
                                Server.Print(Template.Text(Category.Filter, 10).Replace("+n", client.Name), true);
                                client.Disconnect();
                                return String.Empty;
                            }
                            break;

                        case FilterType.Ban:
                            if (client.Link.IsLinked && client.Level == ILevel.Regular)
                                return String.Empty;
                            else if (client.Level == ILevel.Regular)
                            {
                                Server.Print(Template.Text(Category.Filter, 11).Replace("+n", client.Name), true);
                                client.Ban();
                                return String.Empty;
                            }
                            break;

                        case FilterType.Censor:
                            if (client.Link.IsLinked && client.Level == ILevel.Regular)
                                return String.Empty;
                            else if (client.Level == ILevel.Regular)
                            {
                                client.Print(Template.Text(Category.Filter, 12).Replace("+n", client.Name));
                                return String.Empty;
                            }
                            break;

                        case FilterType.Move:
                            if (client.Link.IsLinked && client.Level == ILevel.Regular)
                                return String.Empty;
                            else if (client.Level == ILevel.Regular)
                            {
                                ushort u;

                                if (ushort.TryParse(item.Args, out u))
                                {
                                    Server.Print(Template.Text(Category.Filter, 13).Replace("+n", client.Name), true);
                                    client.Vroom = u;
                                }

                                return String.Empty;
                            }
                            break;

                        case FilterType.Redirect:
                            if (client.Link.IsLinked && client.Level == ILevel.Regular)
                                return String.Empty;
                            else if (client.Level == ILevel.Regular)
                            {
                                Server.Print(Template.Text(Category.Filter, 14).Replace("+n", client.Name), true);
                                client.Redirect(item.Args);
                                client.Disconnect();
                                return String.Empty;
                            }
                            break;

                        case FilterType.Announce:
                            if (Settings.AdminAnnounce)
                                if (client.Level == ILevel.Regular)
                                    break;

                            if (client.Level > ILevel.Regular)
                                if (text.StartsWith("#addline") || text.StartsWith("#remline") || text.StartsWith("#addwordfilter"))
                                    break;

                            String[] lines = item.Args.Split(new String[] { "\r\n" }, StringSplitOptions.None);
                            String reply = String.Empty;

                            if (text.StartsWith(item.Trigger) && text.Length > item.Trigger.Length)
                                reply = text.Substring(item.Trigger.Length + 1);

                            foreach (String str in lines)
                                Server.Print(str.Replace("+n", client.Name).Replace("+ip", ip).Replace("+r", reply), true);
                            break;

                        case FilterType.Replace:
                            if (client.Level == ILevel.Regular)
                                text = Regex.Replace(text, Regex.Escape(item.Trigger), item.Args, RegexOptions.IgnoreCase);
                            break;

                        case FilterType.Scribble:
                            if (!client.WebClient && !client.Link.IsLinked && !text.StartsWith("#addwordfilter"))
                            {
                                uint time = Server.Time;

                                if (client.Level > ILevel.Regular || (time >= (client.LastScribble + 30)))
                                {
                                    client.LastScribble = time;
                                    String html = "<img src=\"" + item.Args + "\" style=\"max-width: 320px; max-height: 320px;\" alt=\"\" />";

                                    Server.Users.Ares(x =>
                                    {
                                        if (x.Vroom == client.Vroom && !x.Quarantined && x.SupportsHTML && !x.IgnoreList.Contains(client.Name))
                                            x.SendHTML(html);
                                    });
                                }
                            }
                            break;
                    }

            return text;
        }

        public static void FilterAfter(IUser client, String text)
        {
            String ip = client.ExternalIP.ToString();

            foreach (Item item in list)
                if (text.ToUpper().Contains(item.Trigger.ToUpper()))
                    switch (item.Type)
                    {
                        case FilterType.PM:
                            client.PM(Server.Chatroom.BotName, item.Args.Replace("+n", client.Name).Replace("+ip", ip));
                            break;

                        case FilterType.Clone:
                            if (client.Level == ILevel.Regular)
                            {
                                if (item.Args.StartsWith("/me "))
                                    client.SendEmote(item.Args.Substring(4).Replace("+n", client.Name).Replace("+ip", ip));
                                else
                                    client.SendText(item.Args.Replace("+n", client.Name).Replace("+ip", ip));
                            }
                            break;

                        case FilterType.Echo:
                            if (!client.Link.IsLinked && client.Level == ILevel.Regular)
                                Echo.Add(client, item.Args);
                            break;
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
                xml.Load(Path.Combine(Server.DataPath, "wordfilters.xml"));

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
            XmlNode root = xml.AppendChild(xml.CreateElement("wordfilters"));

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

            try { xml.Save(Path.Combine(Server.DataPath, "wordfilters.xml")); }
            catch { }
        }

        private enum FilterType
        {
            // without args
            Muzzle,
            Kill,
            Ban,
            Censor,

            // with args
            PM,
            Move,
            Redirect,
            Announce,
            Clone,
            Replace,
            Echo,
            Scribble
        }

        public static void AddLine(IUser admin, int ident, String text)
        {
            if (ident >= 0 && ident < list.Count)
                if (list[ident].Type == FilterType.Announce)
                {
                    list[ident].Args += "\r\n" + text;
                    Save();

                    Server.Print(Template.Text(Category.Filter, 20).Replace("+t",
                        list[ident].Trigger).Replace("+n", Settings.Stealth ? Server.Chatroom.BotName : admin.Name), true);
                }
        }

        public static void RemLine(IUser admin, int ident, int line)
        {
            if (ident >= 0 && ident < list.Count)
                if (list[ident].Type == FilterType.Announce)
                {
                    List<String> lines = new List<String>(list[ident].Args.Split(new String[] { "\r\n" }, StringSplitOptions.None));

                    if (line >= 0 && line < lines.Count)
                    {
                        String name = list[ident].Trigger;
                        lines.RemoveAt(line);

                        if (lines.Count == 0)
                        {
                            list.RemoveAt(ident);
                            Save();

                            Server.Print(Template.Text(Category.Filter, 8).Replace("+t",
                                name).Replace("+n", Settings.Stealth ? Server.Chatroom.BotName : admin.Name), true);
                        }
                        else
                        {
                            list[ident].Args = String.Join("\r\n", lines.ToArray());
                            Save();

                            Server.Print(Template.Text(Category.Filter, 21).Replace("+t",
                                list[ident].Trigger).Replace("+n", Settings.Stealth ? Server.Chatroom.BotName : admin.Name), true);
                        }
                    }
                }
        }

        public static void ViewFilter(IUser admin, int ident)
        {
            if (ident >= 0 && ident < list.Count)
                if (list[ident].Type == FilterType.Announce)
                {
                    String[] lines = list[ident].Args.Split(new String[] { "\r\n" }, StringSplitOptions.None);

                    for (int i = 0; i < lines.Length; i++)
                        admin.Print("line " + i + ": " + lines[i]);
                }
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
                        if (i > 3)
                            if (split.Length > 2)
                            {
                                item.Args = String.Join(", ", new List<String>(split).GetRange(2, (split.Length - 2)).ToArray());

                                if (item.Args.Length == 0)
                                    break;

                                if (i == 6)
                                    if (Server.Hashlinks.Decrypt(item.Args) == null)
                                        break;
                            }
                            else break;
                        else if (split.Length != 2)
                            break;

                        item.Type = (FilterType)Enum.Parse(typeof(FilterType), types[i], true);
                        list.RemoveAll(x => x.Trigger == item.Trigger);
                        list.Add(item);
                        Save();

                        Server.Print(Template.Text(Category.Filter, 7).Replace("+t", item.Trigger).Replace("+f",
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
                        if (i > 3)
                            if (split.Length > 2)
                            {
                                item.Args = String.Join(", ", new List<String>(split).GetRange(2, (split.Length - 2)).ToArray());

                                if (item.Args.Length == 0)
                                    break;

                                if (i == 6)
                                    if (Server.Hashlinks.Decrypt(item.Args) == null)
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

                Server.Print(Template.Text(Category.Filter, 8).Replace("+t",
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
