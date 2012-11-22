using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using iconnect;

namespace commands
{
    class WordFilter
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
            {
                switch (item.Type)
                {
                    case FilterType.Muzzle:
                        break;

                    case FilterType.Kill:
                        break;

                    case FilterType.Ban:
                        break;

                    case FilterType.Censor:
                        break;

                    case FilterType.Move:
                        break;

                    case FilterType.Redirect:
                        break;

                    case FilterType.Announce:
                        break;

                    case FilterType.Replace:
                        break;
                }
            }

            return text;
        }

        public static void FilterAfter(IUser client, String text)
        {
            String ip = client.ExternalIP.ToString();

            foreach (Item item in list)
            {
                switch (item.Type)
                {
                    case FilterType.PM:
                        break;

                    case FilterType.Clone:
                        break;

                    case FilterType.Echo:
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
            Echo
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
                            if (split.Length > 3)
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
