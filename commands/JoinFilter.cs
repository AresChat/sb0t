using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using iconnect;

namespace commands
{
    class JoinFilter
    {
        public static bool IsPreFiltered(IUser client)
        {
            return false;
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

            // with args
            Move,
            Redirect,
            PM,
            Announce,
            Clone,
            ChangeName,
            ReserveName
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
                        if (i > 10)
                            if (split.Length > 2)
                                item.Args = String.Join(", ", new List<String>(split).GetRange(2, (split.Length - 2)).ToArray());
                            else break;

                        item.Type = (FilterType)Enum.Parse(typeof(FilterType), types[i]);
                        list.RemoveAll(x => x.Trigger == item.Trigger);
                        list.Add(item);
                        Save();

                        Server.Print(Template.Text(Category.Filter, 0).Replace("+t", item.Trigger).Replace("+f",
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
