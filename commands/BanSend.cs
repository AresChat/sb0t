using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using iconnect;

namespace commands
{
    class BanSend
    {
        private static List<ushort> list { get; set; }
        private static IPAddress last { get; set; }

        public static void Reset()
        {
            list = new List<ushort>();
            last = IPAddress.Any;
        }

        public static void Rejected(IUser client, String text)
        {
            if (last.Equals(client.ExternalIP))
                return;

            last = client.ExternalIP;
            ILevel l = Server.GetLevel("bansend");
            String bot = Server.Chatroom.BotName;

            Server.Users.All(x =>
            {
                if (!x.Link.IsLinked)
                    if (x.Level >= l)
                        if (list.Contains(x.ID))
                            x.PM(bot, "BANSEND: " + text);
            });
        }

        public static void Add(IUser client)
        {
            if (list.FindIndex(x => x == client.ID) == -1)
                list.Add(client.ID);
        }

        public static void Remove(IUser client)
        {
            list.RemoveAll(x => x == client.ID);
        }

        public static bool Has(IUser client)
        {
            return list.FindIndex(x => x == client.ID) > -1;
        }
    }
}
