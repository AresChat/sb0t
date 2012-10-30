using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace commands
{
    class Paint
    {
        private class Item
        {
            public Guid Guid { get; set; }
            public String Text { get; set; }
        }

        private static List<Item> list { get; set; }

        public static void Add(IUser client, String text)
        {
            Item i = list.Find(x => x.Guid.Equals(client.Guid));

            if (i != null)
                i.Text = text;
            else
                list.Add(new Item
                {
                    Guid = client.Guid,
                    Text = text
                });
        }

        public static void Remove(IUser client)
        {
            list.RemoveAll(x => x.Guid.Equals(client.Guid));
        }

        public static void Clear()
        {
            list = new List<Item>();
        }

        public static String IsPainted(IUser client)
        {
            Item i = list.Find(x => x.Guid.Equals(client.Guid));

            if (i != null)
                return i.Text;

            return null;
        }
    }
}
