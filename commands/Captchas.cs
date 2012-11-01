using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace commands
{
    class Captchas
    {
        private class Item
        {
            public Guid Guid { get; set; }
            public int Attempts { get; set; }
        }

        private static List<Item> list { get; set; }

        public static void Add(IUser client)
        {
            Item i = list.Find(x => x.Guid.Equals(client.Guid));

            if (i != null)
                i.Attempts++;
            else
                list.Add(new Item
                {
                    Guid = client.Guid,
                    Attempts = 0
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

        public static int Count(IUser client)
        {
            return list.Find(x => x.Guid.Equals(client.Guid)).Attempts;
        }
    }
}
