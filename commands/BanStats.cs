using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using iconnect;

namespace commands
{
    class BanStats
    {
        private class Item
        {
            public String Name { get; set; }
            public IPAddress IP { get; set; }
            public String Banner { get; set; }
        }

        private static List<Item> list { get; set; }

        public static void Reset()
        {
            list = new List<Item>();
        }

        public static void Add(IUser admin, IUser target)
        {
            list.Add(new Item
            {
                Banner = admin.Name,
                IP = target.ExternalIP,
                Name = target.Name
            });
        }

        public static void View(IUser client)
        {
            foreach (Item i in list)
                client.Print(Template.Text(Category.BanStats, 0).Replace("+n", i.Name).Replace("+a", i.Banner).Replace("+ip", i.IP.ToString()));
        }
    }
}
