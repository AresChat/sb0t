using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace commands
{
    class Lowered
    {
        private static List<Guid> list { get; set; }

        public static void Clear()
        {
            list = new List<Guid>();
        }

        public static void Add(IUser client)
        {
            if (list.FindIndex(x => x.Equals(client.Guid)) == -1)
                list.Add(client.Guid);
        }

        public static void Remove(IUser client)
        {
            list.RemoveAll(x => x.Equals(client.Guid));
        }

        public static bool IsLowered(IUser client)
        {
            return list.FindIndex(x => x.Equals(client.Guid)) > -1;
        }
    }
}
