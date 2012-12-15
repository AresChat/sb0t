using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace commands
{
    class IPSend
    {
        private static List<ushort> list { get; set; }

        public static void Reset()
        {
            list = new List<ushort>();
        }

        public static void Join(IUser client)
        {
            ILevel l  = Server.GetLevel("ipsend");
            String bot = Server.Chatroom.BotName;

            Server.Users.All(x =>
            {
                if (!x.Link.IsLinked)
                    if (x.Level >= l)
                        if (list.Contains(x.ID))
                            x.PM(bot, "IPSEND: " + client.Name + " " +
                                client.ExternalIP + " " + client.LocalIP + " " + client.DataPort);
            });
        }

        public static void Add(IUser client)
        {
            if (list.FindIndex(x => x == client.ID) == -1)
            {
                list.Add(client.ID);
                String bot = Server.Chatroom.BotName;

                Server.Users.All(x => client.PM(bot, "IPSEND: " + x.Name +
                    " " + x.ExternalIP + " " + x.LocalIP + " " + x.DataPort));
            }
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
