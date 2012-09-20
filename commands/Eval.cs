using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace commands
{
    public class Eval
    {
        [CommandLevel("vroom", ILevel.Regular)]
        public static void Vroom(IUser client, String args)
        {
            if (client.Level >= Server.GetLevel("vroom"))
            {
                ushort vroom;

                if (ushort.TryParse(args, out vroom))
                    client.Vroom = vroom;
            }
        }

        [CommandLevel("id", ILevel.Regular)]
        public static void ID(IUser client)
        {
            if (client.Level >= Server.GetLevel("id"))
                client.Print("ID: " + client.ID);
        }

        [CommandLevel("admins", ILevel.Moderator)]
        public static void Admins(IUser client)
        {
            if (client.Level >= Server.GetLevel("admins"))
            {
                Server.Print("ADMIN LIST", x => x.Vroom == client.Vroom);
                Server.Print(String.Empty, x => x.Vroom == client.Vroom);

                Server.Users.All(u =>
                {
                    if (u.Level > ILevel.Regular)
                        Server.Print(u.Name + " [" + u.Level + "]", x => x.Vroom == client.Vroom);
                });

                Server.Print(String.Empty, x => x.Vroom == client.Vroom);
                Server.Print("END OF LIST", x => x.Vroom == client.Vroom);
            }
        }


        // and so on...


    }
}
