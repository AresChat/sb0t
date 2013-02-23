using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace scripting
{
    class ErrorDispatcher
    {
        private static List<Guid> list = new List<Guid>();

        public static void Reset()
        {
            list = new List<Guid>();
        }

        public static void AddErrors(IUser client)
        {
            if (!client.Link.IsLinked)
                if (list.FindIndex(x => x.Equals(client.Guid)) == -1)
                    list.Add(client.Guid);
        }

        public static void RemoveErrors(IUser client)
        {
            if (!client.Link.IsLinked)
                list.RemoveAll(x => x.Equals(client.Guid));
        }

        public static void SendError(String script, String msg, int line)
        {
            String str = script + ": " + msg + " at line " + line;
            String sender = Server.Chatroom.BotName;

            Server.Users.All(x =>
            {
                if (!x.Link.IsLinked)
                    if (list.FindIndex(z => z.Equals(x.Guid)) > -1)
                        x.PM(sender, str);
            });
        }
    }
}
