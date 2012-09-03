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

        public static void Build()
        {
            AUsers = new List<AresClient>();
        }

        public static void Destroy()
        {
            AUsers.ForEach(x => x.Disconnect());
            AUsers.Clear();
            AUsers = null;
        }

        public static void CreateAresClient(Socket sock, uint time)
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
    }
}
