using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using iconnect;

namespace commands
{
    class Motd
    {
        private static List<String> lines = new List<String>();

        public static void LoadMOTD()
        {
            try
            {
                if (File.Exists(Server.DataPath + "motd.txt"))
                    lines = new List<String>(File.ReadAllLines(Server.DataPath + "motd.txt"));
                else
                {
                    lines.Add("304Welcome to my room +n :-)");
                    lines.Add("303Have fun!");
                    File.WriteAllLines(Server.DataPath + "motd.txt", lines.ToArray(), Encoding.UTF8);
                }
            }
            catch { }
        }

        public static void ViewMOTD(IUser client)
        {
            String _str = Helpers.CountryCodeToString(client.Country);
            List<String> list = new List<String>();
            Server.Users.Ares(x => list.Add(x.Name));
            Server.Users.Web(x => list.Add(x.Name));
            list.RemoveAll(x => x == client.Name);
            String rnd_user = client.Name;

            if (list.Count > 0)
            {
                int index = (int)Math.Floor(new Random().NextDouble() * list.Count);
                rnd_user = list[index];
            }

            if (_str != "?")
                if (client.Region.Length > 0)
                    _str = client.Region + ", " + _str;

            if (_str == "?")
                if (client.Region.Length > 0)
                    _str = client.Region;

            if (_str == "?")
                _str = "unknown";

            foreach (String str in lines)
            {
                String s = str;
                s = s.Replace("+n", client.Name);
                s = s.Replace("+ip", client.ExternalIP.ToString());
                s = s.Replace("+id", client.ID.ToString());
                s = s.Replace("+f", client.FileCount.ToString());
                s = s.Replace("+v", client.Version);
                s = s.Replace("+p", client.DataPort.ToString());
                s = s.Replace("+uc", Server.Stats.CurrentUserCount.ToString());
                s = s.Replace("+rn", Server.Chatroom.Name);
                s = s.Replace("+ut", Helpers.GetUptime);
                s = s.Replace("+ru", rnd_user);
                s = s.Replace("+l", _str);
                client.Print(Helpers.SetColors(s));
            }
        }

        public static int MotdSize
        {
            get
            {
                return Encoding.UTF8.GetByteCount(String.Join(String.Empty, lines.ToArray()));
            }
        }
    }
}
