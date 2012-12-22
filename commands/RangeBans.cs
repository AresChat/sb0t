using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using iconnect;

namespace commands
{
    class RangeBans
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

        public static String Remove(String str)
        {
            if (!String.IsNullOrEmpty(str))
                if (list.RemoveAll(x => x == str) > 0)
                {
                    Update();
                    return str;
                }
                else
                {
                    int id;

                    if (int.TryParse(str, out id))
                        if (id >= 0 && id < list.Count)
                        {
                            String result = list[id];
                            list.RemoveAt(id);
                            Update();
                            return result;
                        }
                }

            return null;
        }

        public static void List(IUser client)
        {
            int counter = 0;

            if (list.Count == 0)
                client.Print(Template.Text(Category.Notification, 1));

            foreach (String str in list)
                client.Print((counter++) + " - " + str);
        }

        public static void Clear()
        {
            list = new List<String>();
            Update();
        }

        public static void Load()
        {
            list = new List<String>();

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.PreserveWhitespace = true;
                xml.Load(Path.Combine(Server.DataPath, "rangebans.xml"));

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
            XmlNode root = xml.AppendChild(xml.CreateElement("rangebans"));

            foreach (String i in list)
            {
                XmlNode item = root.OwnerDocument.CreateNode(XmlNodeType.Element, "item", root.BaseURI);
                root.AppendChild(item);
                item.InnerText = i;
            }

            try { xml.Save(Path.Combine(Server.DataPath, "rangebans.xml")); }
            catch { }
        }

        public static bool IsRangeBanned(IUser client)
        {
            String str = client.ExternalIP.ToString();
            return list.Find(x => str.StartsWith(x)) != null;
        }
    }
}
