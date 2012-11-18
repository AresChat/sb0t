using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace commands
{
    class VSpy
    {
        private static List<Guid> list { get; set; }

        public static void Reset()
        {
            list = new List<Guid>();
        }

        public static void Add(IUser client)
        {
            list.RemoveAll(x => x.Equals(client.Guid));
            list.Add(client.Guid);
        }

        public static void Remove(IUser client)
        {
            list.RemoveAll(x => x.Equals(client.Guid));
        }

        public static void Text(IUser client, String text)
        {
            ILevel level = Server.GetLevel("vspy");

            Server.Users.Ares(x =>
            {
                if (x.Vroom != client.Vroom)
                    if (list.FindIndex(z => z.Equals(x.Guid)) > -1)
                        if (x.Level >= level)
                            Server.PublicToTarget(x, client.Name, "[vroom: " + client.Vroom + "] " + text);
            });

            Server.Users.Web(x =>
            {
                if (x.Vroom != client.Vroom)
                    if (list.FindIndex(z => z.Equals(x.Guid)) > -1)
                        if (x.Level >= level)
                            Server.PublicToTarget(x, client.Name, "[vroom: " + client.Vroom + "] " + text);
            });
        }

        public static void Emote(IUser client, String text)
        {
            ILevel level = Server.GetLevel("vspy");

            Server.Users.Ares(x =>
            {
                if (x.Vroom != client.Vroom)
                    if (list.FindIndex(z => z.Equals(x.Guid)) > -1)
                        if (x.Level >= level)
                            Server.EmoteToTarget(x, client.Name, "[vroom: " + client.Vroom + "] " + text);
            });

            Server.Users.Web(x =>
            {
                if (x.Vroom != client.Vroom)
                    if (list.FindIndex(z => z.Equals(x.Guid)) > -1)
                        if (x.Level >= level)
                            Server.EmoteToTarget(x, client.Name, "[vroom: " + client.Vroom + "] " + text);
            });
        }

        public static void Join(IUser client)
        {
            ILevel level = Server.GetLevel("vspy");
            String text = Template.Text(Category.Vspy, 0).Replace("+n", client.Name).Replace("+ip", client.ExternalIP.ToString());

            Server.Users.Ares(x =>
            {
                if (x.Vroom != client.Vroom)
                    if (list.FindIndex(z => z.Equals(x.Guid)) > -1)
                        if (x.Level >= level)
                            x.Print(text);
            });

            Server.Users.Web(x =>
            {
                if (x.Vroom != client.Vroom)
                    if (list.FindIndex(z => z.Equals(x.Guid)) > -1)
                        if (x.Level >= level)
                            x.Print(text);
            });
        }

        public static void Part(IUser client)
        {
            ILevel level = Server.GetLevel("vspy");
            String text = Template.Text(Category.Vspy, 1).Replace("+n", client.Name).Replace("+ip", client.ExternalIP.ToString());

            Server.Users.Ares(x =>
            {
                if (x.Vroom != client.Vroom)
                    if (list.FindIndex(z => z.Equals(x.Guid)) > -1)
                        if (x.Level >= level)
                            x.Print(text);
            });

            Server.Users.Web(x =>
            {
                if (x.Vroom != client.Vroom)
                    if (list.FindIndex(z => z.Equals(x.Guid)) > -1)
                        if (x.Level >= level)
                            x.Print(text);
            });
        }

        public static void VroomChanged(IUser client)
        {
            ILevel level = Server.GetLevel("vspy");
            String text = Template.Text(Category.Vspy, 2).Replace("+n", client.Name).Replace("+v", client.Vroom.ToString());

            Server.Users.Ares(x =>
            {
                if (x.Vroom != client.Vroom)
                    if (list.FindIndex(z => z.Equals(x.Guid)) > -1)
                        if (x.Level >= level)
                            x.Print(text);
            });

            Server.Users.Web(x =>
            {
                if (x.Vroom != client.Vroom)
                    if (list.FindIndex(z => z.Equals(x.Guid)) > -1)
                        if (x.Level >= level)
                            x.Print(text);
            });
        }
    }
}
