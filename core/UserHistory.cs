using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    class UserHistory
    {
        private static List<PartedClient> list;

        public static void Initialize()
        {
            list = new List<PartedClient>();
        }

        public static void AddUser(AresClient client)
        {
            list.RemoveAll(i => i.ExternalIP.Equals(client.ExternalIP) && i.Name == client.Name);
            
            list.Insert(0, new PartedClient
            {
                ExternalIP = client.ExternalIP,
                Name = client.Name,
                Time = client.Time
            });
        }

        public static bool IsJoinFlooding(AresClient client, ulong time)
        {
            return (from x in list
                    where x.ExternalIP.Equals(client.ExternalIP)
                    && (x.Time + 15000) > time
                    select x).Count() > 0;
        }
    }
}
