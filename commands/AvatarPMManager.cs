using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace commands
{
    class AvatarPMManager
    {
        private static List<Guid> avatars { get; set; }
        private static List<Item> pms { get; set; }

        private class Item
        {
            public Guid Guid { get; set; }
            public String Text { get; set; }
        }

        public static void Reset()
        {
            avatars = new List<Guid>();
            pms = new List<Item>();
        }

        public static void AddPM(IUser client, String msg)
        {
            int i = pms.FindIndex(x => x.Equals(client.Guid));

            if (i == -1)
                pms.Add(new Item
                {
                    Guid = client.Guid,
                    Text = msg
                });
            else pms[i].Text = msg;
        }

        public static void AddAvatar(IUser client)
        {
            if (avatars.FindIndex(x => x.Equals(client.Guid)) == -1)
                avatars.Add(client.Guid);
        }

        public static String GetPM(IUser client)
        {
            Item i = pms.Find(x => x.Guid.Equals(client.Guid));

            if (i == null)
                return null;

            return i.Text;
        }

        public static bool CanAvatar(IUser client)
        {
            return avatars.FindIndex(x => x.Equals(client.Guid)) == -1;
        }
    }
}
