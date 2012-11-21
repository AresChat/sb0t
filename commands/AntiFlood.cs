using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace commands
{
    class AntiFlood
    {
        private static List<ushort> list { get; set; }

        public static void Reset()
        {
            list = new List<ushort>();
        }

        public static void Add(IUser client)
        {
            list.RemoveAll(x => x == client.ID);
            list.Add(client.ID);
        }

        public static void Remove(IUser client)
        {
            list.RemoveAll(x => x == client.ID);
        }

        public static bool CanFlood(IUser client)
        {
            return list.FindIndex(x => x == client.ID) > -1;
        }
    }
}
