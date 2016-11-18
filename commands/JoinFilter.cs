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
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using iconnect;

namespace commands
{
    public class JoinFilter
    {
        public static bool IsPreFiltered(IUser client)
        {
            String ip = client.ExternalIP.ToString();
            ushort u;

            foreach (Item item in list)
            {
                switch (item.Type)
                {
                    case FilterType.Censor:
                        if (client.Name.ToUpper().Contains(item.Trigger.ToUpper()))
                            client.Name = Regex.Replace(client.Name, Regex.Escape(item.Trigger), String.Empty, RegexOptions.IgnoreCase);
                        break;

                    case FilterType.PortBan:
                        if (ushort.TryParse(item.Trigger, out u))
                            if (client.DataPort == u)
                                return true;
                        break;

                    case FilterType.IPBan:
                        if (ip.StartsWith(item.Trigger))
                            return true;
                        break;

                    case FilterType.NameBan:
                        if (client.Name == item.Trigger)
                            return true;
                        break;

                    case FilterType.DNSBan:
                        if (client.DNS.ToUpper().Contains(item.Trigger.ToUpper()))
                            return true;
                        break;

                    case FilterType.Move:
                        if (client.Name.ToUpper().Contains(item.Trigger.ToUpper()) || ip.StartsWith(item.Trigger))
                            if (ushort.TryParse(item.Args, out u))
                                client.Vroom = u;
                        break;

                    case FilterType.Redirect:
                        if (client.Name.ToUpper().Contains(item.Trigger.ToUpper()) || ip.StartsWith(item.Trigger))
                        {
                            client.Redirect(item.Args);
                            return true;
                        }
                        break;

                    case FilterType.ChangeName:
                        if (client.Name.ToUpper().Contains(item.Trigger.ToUpper()) || ip.StartsWith(item.Trigger))
                            client.Name = item.Args;
                        break;

                    case FilterType.VersionBan:
                        if (client.Version.ToUpper().Contains(item.Trigger.ToUpper()))
                            return true;
                        break;
                }
            }

            return false;
        }

        public static void DoPostFilter(IUser client)
        {
            String ip = client.ExternalIP.ToString();

            foreach (Item item in list)
            {
                switch (item.Type)
                {
                    case FilterType.DisableAvatar:
                        if (client.Name.ToUpper().Contains(item.Trigger.ToUpper()) || ip.StartsWith(item.Trigger))
                            AvatarPMManager.AddAvatar(client);
                        break;

                    case FilterType.Vspy:
                        if (client.Name.ToUpper().Contains(item.Trigger.ToUpper()) || ip.StartsWith(item.Trigger))
                            if (!client.Link.IsLinked)
                                VSpy.Add(client);
                        break;

                    case FilterType.AntiFlood:
                        if (client.Name.ToUpper().Contains(item.Trigger.ToUpper()) || ip.StartsWith(item.Trigger))
                            if (!client.Link.IsLinked)
                                AntiFlood.Add(client);
                        break;

                    case FilterType.IPSend:
                        if (client.Name.ToUpper().Contains(item.Trigger.ToUpper()) || ip.StartsWith(item.Trigger))
                            if (!client.Link.IsLinked)
                                IPSend.Add(client);
                        break;

                    case FilterType.LogSend:
                        if (client.Name.ToUpper().Contains(item.Trigger.ToUpper()) || ip.StartsWith(item.Trigger))
                            if (!client.Link.IsLinked)
                                LogSend.Add(client);
                        break;

                    case FilterType.BanSend:
                        if (client.Name.ToUpper().Contains(item.Trigger.ToUpper()) || ip.StartsWith(item.Trigger))
                            if (!client.Link.IsLinked)
                                BanSend.Add(client);
                        break;

                    case FilterType.PM:
                        if (client.Name.ToUpper().Contains(item.Trigger.ToUpper()) || ip.StartsWith(item.Trigger))
                            client.PM(Server.Chatroom.BotName, item.Args.Replace("+n", client.Name).Replace("+ip", ip));
                        break;

                    case FilterType.Announce:
                        if (client.Name.ToUpper().Contains(item.Trigger.ToUpper()) || ip.StartsWith(item.Trigger))
                            client.Print(item.Args.Replace("+n", client.Name).Replace("+ip", ip));
                        break;

                    case FilterType.Clone:
                        if (client.Name.ToUpper().Contains(item.Trigger.ToUpper()) || ip.StartsWith(item.Trigger))
                            if (item.Args.StartsWith("/me "))
                                client.SendEmote(item.Args.Substring(4).Replace("+n", client.Name).Replace("+ip", ip));
                            else
                                client.SendText(item.Args.Replace("+n", client.Name).Replace("+ip", ip));
                        break;

                    case FilterType.ChangeMessage:
                        if (client.Name.ToUpper().Contains(item.Trigger.ToUpper()) || ip.StartsWith(item.Trigger))
                        {
                            AvatarPMManager.AddPM(client, item.Args);
                            client.PersonalMessage = item.Args;
                        }
                        break;

                    case FilterType.Scribble:
                        if (!client.WebClient && !client.Link.IsLinked)
                            if (client.Name.ToUpper().Contains(item.Trigger.ToUpper()) || ip.StartsWith(item.Trigger))
                            {
                                String html = "<img src=\"" + item.Args + "\" style=\"max-width: 320px; max-height: 320px;\" alt=\"\" />";

                                Server.Users.Ares(x =>
                                {
                                    if (x.Vroom == client.Vroom && !x.Quarantined && x.SupportsHTML)
                                        x.SendHTML(html);
                                });
                            }
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
                xml.Load(Path.Combine(Server.DataPath, "joinfilters.xml"));

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
            XmlNode root = xml.AppendChild(xml.CreateElement("joinfilters"));

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

            try { xml.Save(Path.Combine(Server.DataPath, "joinfilters.xml")); }
            catch { }
        }

        private enum FilterType
        {
            // without args
            DisableAvatar,
            Censor,
            Vspy,
            AntiFlood,
            IPSend,
            LogSend,
            BanSend,
            PortBan,
            IPBan,
            NameBan,
            DNSBan,
            VersionBan,

            // with args
            Move,
            Redirect,
            PM,
            Announce,
            Clone,
            ChangeName,
            ReserveName,
            ChangeMessage,
            Scribble
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
                        if (i > 11)
                            if (split.Length > 2)
                            {
                                item.Args = String.Join(", ", new List<String>(split).GetRange(2, (split.Length - 2)).ToArray());

                                if (item.Args.Length == 0)
                                    break;

                                if (i == 13)
                                    if (Server.Hashlinks.Decrypt(item.Args) == null)
                                        break;
                            }
                            else break;
                        else if (split.Length != 2)
                            break;

                        item.Type = (FilterType)Enum.Parse(typeof(FilterType), types[i], true);
                      //  list.RemoveAll(x => x.Trigger == item.Trigger);
                        list.Add(item);

                        Save();

                        Server.Print(Template.Text(Category.Filter, 0).Replace("+t", item.Trigger).Replace("+f",
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
                        if (i > 11)
                            if (split.Length > 2)
                            {
                                item.Args = String.Join(", ", new List<String>(split).GetRange(2, (split.Length - 2)).ToArray());

                                if (item.Args.Length == 0)
                                    break;

                                if (i == 13)
                                    if (Server.Hashlinks.Decrypt(item.Args) == null)
                                        break;
                            }
                            else break;
                        else if (split.Length != 2)
                            break;

                        item.Type = (FilterType)Enum.Parse(typeof(FilterType), types[i], true);
                      //  list.RemoveAll(x => x.Trigger == item.Trigger);
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

                Server.Print(Template.Text(Category.Filter, 1).Replace("+t",
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
