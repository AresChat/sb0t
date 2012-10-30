using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Net;
using iconnect;

namespace commands
{
    class Muzzles
    {
        private class Item
        {
            public IPAddress IP { get; set; }
            public Guid Guid { get; set; }
        }

        private static List<Item> list { get; set; }

        public static bool IsMuzzled(IUser client)
        {
            return list.Find(x => x.IP.Equals(client.ExternalIP) || x.Guid.Equals(client.Guid)) != null;
        }

        public static void Clear()
        {
            list = new List<Item>();
            Save();
            Server.Users.Ares(x => { if (x.Muzzled)x.Muzzled = false; });
            Server.Users.Web(x => { if (x.Muzzled)x.Muzzled = false; });
        }

        public static void Load()
        {
            list = new List<Item>();

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.PreserveWhitespace = true;
                xml.Load(Path.Combine(Server.DataPath, "muzzles.xml"));

                XmlNodeList nodes = xml.GetElementsByTagName("item");

                foreach (XmlElement e in nodes)
                    list.Add(new Item
                    {
                        IP = IPAddress.Parse(e.GetElementsByTagName("ip")[0].InnerText),
                        Guid = new Guid(e.GetElementsByTagName("guid")[0].InnerText)
                    });
            }
            catch { }
        }

        public static void AddMuzzle(IUser client)
        {
            if (list.Find(x => x.IP.Equals(client.ExternalIP) || x.Guid.Equals(client.Guid)) == null)
            {
                list.Add(new Item
                {
                    Guid = client.Guid,
                    IP = client.ExternalIP
                });

                Save();
            }
        }

        public static void RemoveMuzzle(IUser client)
        {
            if (list.RemoveAll(x => x.IP.Equals(client.ExternalIP) || x.Guid.Equals(client.Guid)) > 0)
                Save();
        }

        private static void Save()
        {
            XmlDocument xml = new XmlDocument();
            xml.PreserveWhitespace = true;
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", null, null));
            XmlNode root = xml.AppendChild(xml.CreateElement("muzzles"));

            foreach (Item i in list)
            {
                XmlNode item = root.OwnerDocument.CreateNode(XmlNodeType.Element, "item", root.BaseURI);
                root.AppendChild(item);
                XmlNode ip = item.OwnerDocument.CreateNode(XmlNodeType.Element, "ip", item.BaseURI);
                item.AppendChild(ip);
                ip.InnerText = i.IP.ToString();
                XmlNode guid = item.OwnerDocument.CreateNode(XmlNodeType.Element, "guid", item.BaseURI);
                item.AppendChild(guid);
                guid.InnerText = i.Guid.ToString();
            }

            try { xml.Save(Path.Combine(Server.DataPath, "muzzles.xml")); }
            catch { }
        }

    }
}
