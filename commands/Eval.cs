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

        [CommandLevel("ban", ILevel.Administrator)]
        public static void Ban(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("ban"))
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        Server.Print(Template.Text(Category.AdminAction, 0).Replace("+n",
                            target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")));

                        target.Ban();
                    }
        }

        [CommandLevel("unban", ILevel.Administrator)]
        public static void Unban(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("unban"))
            {
                String name = null;
                Server.Users.Banned(x => { if (x.Name == args) { name = x.Name; x.Unban(); return; } });

                if (name == null && args.Length > 0)
                {
                    Server.Users.Banned(x => { if (x.Name.StartsWith(args)) { name = x.Name; x.Unban(); return; } });

                    if (name == null)
                    {
                        uint counter = 0;
                        uint target;

                        if (uint.TryParse(args, out target))
                            Server.Users.Banned(x => { if (counter == target) { name = x.Name; x.Unban(); return; } counter++; });
                    }
                }

                if (name != null)
                    Server.Print(Template.Text(Category.AdminAction, 1).Replace("+n", name).Replace("+a", admin.Name));
            }
        }

        [CommandLevel("kick", ILevel.Moderator)]
        public static void Kick(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("kick"))
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        Server.Print(Template.Text(Category.AdminAction, 2).Replace("+n",
                            target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")));

                        target.Disconnect();
                    }
        }

        [CommandLevel("muzzle", ILevel.Moderator)]
        public static void Muzzle(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("muzzle"))
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        Server.Print(Template.Text(Category.AdminAction, 3).Replace("+n",
                            target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")));

                        target.Muzzled = true;
                        Muzzles.AddMuzzle(target);
                    }
        }

        public static void Unmuzzle(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("muzzle"))
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        Server.Print(Template.Text(Category.AdminAction, 4).Replace("+n",
                            target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")));

                        target.Muzzled = false;
                        Muzzles.RemoveMuzzle(target);
                    }
        }

        [CommandLevel("customname", ILevel.Moderator)]
        public static void CustomName(IUser admin, IUser target, String args)
        {
            if (target == null || args.Length < 2)
                return;

            if (admin.Name == target.Name)
            {
                target.CustomName = args;
                CustomNames.UpdateCustomName(target);
                Server.Print(Template.Text(Category.AdminAction, 5).Replace("+n", target.Name).Replace("+a", admin.Name));
            }
            else if (admin.Level >= Server.GetLevel("customname"))
            {
                target.CustomName = args;
                CustomNames.UpdateCustomName(target);
                Server.Print(Template.Text(Category.AdminAction, 5).Replace("+n", target.Name).Replace("+a", admin.Name));
            }
        }

        [CommandLevel("uncustomname", ILevel.Moderator)]
        public static void UncustomName(IUser admin, IUser target, String args)
        {
            if (target == null)
                return;

            if (admin.Name == target.Name)
            {
                target.CustomName = null;
                CustomNames.UpdateCustomName(target);
                Server.Print(Template.Text(Category.AdminAction, 6).Replace("+n", target.Name).Replace("+a", admin.Name));
            }
            else if (admin.Level >= Server.GetLevel("uncustomname"))
            {
                target.CustomName = null;
                CustomNames.UpdateCustomName(target);
                Server.Print(Template.Text(Category.AdminAction, 6).Replace("+n", target.Name).Replace("+a", admin.Name));
            }
        }

        [CommandLevel("kewltext", ILevel.Moderator)]
        public static void AddKewlText(IUser admin, IUser target)
        {
            if (admin.Level >= Server.GetLevel("kewltext"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 7).Replace("+n", target.Name).Replace("+a", admin.Name));
                    KewlText.Add(target);
                }
        }

        public static void RemKewlText(IUser admin, IUser target)
        {
            if (admin.Level >= Server.GetLevel("kewltext"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 8).Replace("+n", target.Name).Replace("+a", admin.Name));
                    KewlText.Remove(target);
                }
        }

        [CommandLevel("lower", ILevel.Moderator)]
        public static void Lower(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("lower"))
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        Server.Print(Template.Text(Category.AdminAction, 9).Replace("+n",
                            target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")));

                        Lowered.Add(target);
                    }
        }

        [CommandLevel("unlower", ILevel.Moderator)]
        public static void Unlower(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("unlower"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 10).Replace("+n",
                        target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")));

                    Lowered.Remove(target);
                }
        }

        [CommandLevel("kiddy", ILevel.Moderator)]
        public static void Kiddy(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("kiddy"))
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        Server.Print(Template.Text(Category.AdminAction, 11).Replace("+n",
                            target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")));

                        Kiddied.Add(target);
                    }
        }

        public static void Unkiddy(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("kiddy"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 12).Replace("+n",
                        target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")));

                    Kiddied.Remove(target);
                }
        }

        [CommandLevel("echo", ILevel.Moderator)]
        public static void Echo(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("echo"))
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        Server.Print(Template.Text(Category.AdminAction, 13).Replace("+n", target.Name).Replace("+a", admin.Name));
                        commands.Echo.Add(target, args);
                    }
        }

        public static void Unecho(IUser admin, IUser target)
        {
            if (admin.Level >= Server.GetLevel("echo"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 14).Replace("+n", target.Name).Replace("+a", admin.Name));
                    commands.Echo.Remove(target);
                }
        }

        [CommandLevel("paint", ILevel.Moderator)]
        public static void Paint(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("paint"))
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        Server.Print(Template.Text(Category.AdminAction, 15).Replace("+n", target.Name).Replace("+a", admin.Name));
                        commands.Paint.Add(target, args);
                    }
        }

        public static void Unpaint(IUser admin, IUser target)
        {
            if (admin.Level >= Server.GetLevel("paint"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 16).Replace("+n", target.Name).Replace("+a", admin.Name));
                    commands.Paint.Remove(target);
                }
        }

        [CommandLevel("rangeban", ILevel.Administrator)]
        public static void RangeBan(IUser client, String args)
        {
            if (client.Level >= Server.GetLevel("rangeban"))
            {
                String str = args.Replace("\"", "").Replace("*", "").Trim();

                if (!String.IsNullOrEmpty(str))
                {
                    Server.Print(Template.Text(Category.AdminAction, 17).Replace("+a", client.Name).Replace("+r", str));
                    RangeBans.Add(str);
                }
            }
        }

        [CommandLevel("rangeunban", ILevel.Administrator)]
        public static void RangeUnban(IUser client, String args)
        {
            if (client.Level >= Server.GetLevel("rangeunban"))
            {
                String str = RangeBans.Remove(args);

                if (str != null)
                    Server.Print(Template.Text(Category.AdminAction, 18).Replace("+a", client.Name).Replace("+r", str));
            }
        }

        [CommandLevel("listrangebans", ILevel.Administrator)]
        public static void ListRangeBans(IUser client)
        {
            if (client.Level >= Server.GetLevel("listrangebans"))
                RangeBans.List(client);
        }

        [CommandLevel("cbans", ILevel.Host)]
        public static void Cbans(IUser client)
        {
            if (client.Level >= Server.GetLevel("cbans"))
            {
                Server.ClearBans();
                RangeBans.Clear();
                Muzzles.Clear();
                Kiddied.Clear();
                commands.Echo.Clear();
                commands.Paint.Clear();
                Lowered.Clear();
                Server.Print(Template.Text(Category.AdminAction, 19).Replace("+a", client.Name));
            }
        }
    }
}
