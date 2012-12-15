using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using iconnect;

namespace commands
{
    class LogSend
    {
        private static List<ushort> list { get; set; }

        public static void Reset()
        {
            list = new List<ushort>();
        }

        public static void Log(IUser client, String text)
        {
            ILevel l = Server.GetLevel("logsend");
            String bot = Server.Chatroom.BotName;

            Server.Users.All(x =>
            {
                if (!x.Link.IsLinked)
                    if (x.Level >= l)
                        if (list.Contains(x.ID))
                            x.PM(bot, "LOGSEND: " + client.Name + "> /" + text);
            });

            try
            {
                DateTime d = DateTime.Now;
                String path = Path.Combine(Server.DataPath, "adminlog.txt");

                using (StreamWriter writer = File.Exists(path) ? File.AppendText(path) : File.CreateText(path))
                    writer.WriteLine(d.ToShortDateString() + " " + d.ToShortTimeString() + " " + client.Name + "> /" + text);
            }
            catch { }
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
