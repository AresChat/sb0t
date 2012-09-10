using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace core
{
    class UserPool
    {
        public static List<AresClient> AUsers;
        public static List<ib0t.ib0tClient> WUsers;

        public static void Build()
        {
            AUsers = new List<AresClient>();
            WUsers = new List<ib0t.ib0tClient>();
        }

        public static void Destroy()
        {
            AUsers.ForEach(x => x.Disconnect());
            AUsers.Clear();
            AUsers = null;
            WUsers.ForEach(x => x.Disconnect());
            WUsers.Clear();
            WUsers = null;
        }

        public static void CreateAresClient(Socket sock, ulong time)
        {
            for (ushort u = 0; u < ushort.MaxValue; u++)
            {
                int index = AUsers.FindIndex(x => x.ID == u);

                if (index == -1)
                {
                    AUsers.Add(new AresClient(sock, time, u));
                    AUsers.Sort((x, y) => x.ID.CompareTo(y.ID));
                    break;
                }
            }
        }

        public static void CreateIb0tClient(AresClient client, ulong time)
        {
            for (ushort u = 700; u < ushort.MaxValue; u++)
            {
                int index = AUsers.FindIndex(x => x.ID == u);

                if (index == -1)
                {
                    WUsers.Add(new ib0t.ib0tClient(client, time, u));
                    WUsers.Sort((x, y) => x.ID.CompareTo(y.ID));
                    client.Sock = null;
                    AUsers.RemoveAll(x => x.ID == client.ID);
                    break;
                }
            }
        }
    }
}
