using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    class UserHistory
    {
        public static List<PartedClient> list { get; set; }

        public static void Initialize()
        {
            list = new List<PartedClient>();
        }

        public static void AddUser(IClient client, ulong time)
        {
            list.RemoveAll(i => i.ExternalIP.Equals(client.ExternalIP) && i.Name == client.Name);
            
            list.Insert(0, new PartedClient
            {
                ExternalIP = client.ExternalIP,
                Name = client.Name,
                Time = time,
                DataPort = client.DataPort,
                DNS = client.DNS,
                Guid = client.Guid,
                JoinTime = Helpers.UnixTime,
                LocalIP  = client.LocalIP,
                Version = client.Version
            });
        }

        public static bool IsJoinFlooding(IClient client, ulong time)
        {
            return (from x in list
                    where x.ExternalIP.Equals(client.ExternalIP)
                    && (x.Time + 15000) > time
                    select x).Count() > 0;
        }
    }
}
