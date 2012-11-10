using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace commands
{
    public class Eval
    {
        public static void Vroom(IUser client, String args)
        {
            if (client.Level >= ILevel.Regular || Settings.General)
            {
                ushort vroom;

                if (ushort.TryParse(args, out vroom))
                    client.Vroom = vroom;
            }
        }

        public static void ID(IUser client)
        {
            client.Print("ID: " + client.ID);
        }

        [CommandLevel("ban", ILevel.Administrator)]
        public static void Ban(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("ban"))
                if (target != null)
                    if (target.Level < admin.Level)
                        if (!(admin.Link.IsLinked && !target.Link.IsLinked))
                        {
                            Server.Print(Template.Text(Category.AdminAction, 0).Replace("+n",
                                target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

                            target.Ban();
                        }
        }

        [CommandLevel("ban10", ILevel.Moderator)]
        public static void Ban10(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("ban10"))
                if (target != null)
                    if (target.Level < admin.Level)
                        if (!(admin.Link.IsLinked && !target.Link.IsLinked))
                        {
                            Server.Print(Template.Text(Category.AdminAction, 21).Replace("+n",
                                target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

                            Bans.AddBan(target, BanDuration.Ten);
                            target.Ban();
                        }
        }

        [CommandLevel("ban60", ILevel.Administrator)]
        public static void Ban60(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("ban60"))
                if (target != null)
                    if (target.Level < admin.Level)
                        if (!(admin.Link.IsLinked && !target.Link.IsLinked))
                        {
                            Server.Print(Template.Text(Category.AdminAction, 22).Replace("+n",
                                target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

                            Bans.AddBan(target, BanDuration.Sixty);
                            target.Ban();
                        }
        }

        [CommandLevel("unban", ILevel.Administrator)]
        public static void Unban(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("unban"))
            {
                String name = null;
                Server.Users.Banned(x => { if (x.Name == args.Replace("\"", String.Empty)) { name = x.Name; x.Unban(); return; } });

                if (name == null && args.Length > 0)
                {
                    Server.Users.Banned(x => { if (x.Name.StartsWith(args.Replace("\"", String.Empty))) { name = x.Name; x.Unban(); return; } });

                    if (name == null)
                    {
                        uint counter = 0;
                        uint target;

                        if (uint.TryParse(args, out target))
                            Server.Users.Banned(x => { if (counter == target) { name = x.Name; x.Unban(); return; } counter++; });
                    }
                }

                if (name != null)
                {
                    Bans.RemoveBan(name);
                    Server.Print(Template.Text(Category.AdminAction, 1).Replace("+n", name).Replace("+a", admin.Name), true);
                }
            }
        }

        [CommandLevel("listbans", ILevel.Administrator)]
        public static void ListBans(IUser admin)
        {
            if (admin.Level >= Server.GetLevel("listbans"))
            {
                bool empty = true;
                int counter = 0;

                Server.Users.Banned(x =>
                {
                    empty = false;
                    admin.Print((counter++) + " - " + x.Name);
                });

                if (empty)
                    admin.Print(Template.Text(Category.Notification, 1));
            }
        }

        [CommandLevel("kick", ILevel.Moderator)]
        public static void Kick(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("kick"))
                if (target != null)
                    if (target.Level < admin.Level)
                        if (!(admin.Link.IsLinked && !target.Link.IsLinked))
                        {
                            Server.Print(Template.Text(Category.AdminAction, 2).Replace("+n",
                                target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

                            target.Disconnect();
                        }
        }

        [CommandLevel("muzzle", ILevel.Moderator)]
        public static void Muzzle(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("muzzle"))
                if (target != null)
                    if (target.Level < admin.Level)
                        if (!(admin.Link.IsLinked && !target.Link.IsLinked))
                        {
                            Server.Print(Template.Text(Category.AdminAction, 3).Replace("+n",
                                target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

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
                            target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

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
                if (admin.Level > ILevel.Regular || Settings.General)
                {
                    target.CustomName = args;
                    commands.CustomNames.UpdateCustomName(target);
                    Server.Print(Template.Text(Category.AdminAction, 5).Replace("+n", target.Name).Replace("+a", admin.Name), true);
                }
            }
            else if (admin.Level >= Server.GetLevel("customname"))
            {
                target.CustomName = args;
                commands.CustomNames.UpdateCustomName(target);
                Server.Print(Template.Text(Category.AdminAction, 5).Replace("+n", target.Name).Replace("+a", admin.Name), true);
            }
        }

        [CommandLevel("uncustomname", ILevel.Moderator)]
        public static void UncustomName(IUser admin, IUser target, String args)
        {
            if (target == null)
                return;

            if (admin.Name == target.Name)
            {
                if (admin.Level > ILevel.Regular || Settings.General)
                {
                    target.CustomName = null;
                    commands.CustomNames.UpdateCustomName(target);
                    Server.Print(Template.Text(Category.AdminAction, 6).Replace("+n", target.Name).Replace("+a", admin.Name), true);
                }
            }
            else if (admin.Level >= Server.GetLevel("uncustomname"))
            {
                target.CustomName = null;
                commands.CustomNames.UpdateCustomName(target);
                Server.Print(Template.Text(Category.AdminAction, 6).Replace("+n", target.Name).Replace("+a", admin.Name), true);
            }
        }

        [CommandLevel("kewltext", ILevel.Moderator)]
        public static void AddKewlText(IUser admin, IUser target)
        {
            if (admin.Level >= Server.GetLevel("kewltext"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 7).Replace("+n", target.Name).Replace("+a", admin.Name), true);
                    KewlText.Add(target);
                }
        }

        public static void RemKewlText(IUser admin, IUser target)
        {
            if (admin.Level >= Server.GetLevel("kewltext"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 8).Replace("+n", target.Name).Replace("+a", admin.Name), true);
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
                            target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

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
                        target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

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
                            target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

                        Kiddied.Add(target);
                    }
        }

        public static void Unkiddy(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("kiddy"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 12).Replace("+n",
                        target.Name).Replace("+a", admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

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
                        Server.Print(Template.Text(Category.AdminAction, 13).Replace("+n", target.Name).Replace("+a", admin.Name), true);
                        commands.Echo.Add(target, args);
                    }
        }

        public static void Unecho(IUser admin, IUser target)
        {
            if (admin.Level >= Server.GetLevel("echo"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 14).Replace("+n", target.Name).Replace("+a", admin.Name), true);
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
                        Server.Print(Template.Text(Category.AdminAction, 15).Replace("+n", target.Name).Replace("+a", admin.Name), true);
                        commands.Paint.Add(target, args);
                    }
        }

        public static void Unpaint(IUser admin, IUser target)
        {
            if (admin.Level >= Server.GetLevel("paint"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 16).Replace("+n", target.Name).Replace("+a", admin.Name), true);
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
                    Server.Print(Template.Text(Category.AdminAction, 17).Replace("+a", client.Name).Replace("+r", str), true);
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
                    Server.Print(Template.Text(Category.AdminAction, 18).Replace("+a", client.Name).Replace("+r", str), true);
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
                Server.Print(Template.Text(Category.AdminAction, 19).Replace("+a", client.Name), true);
            }
        }

        public static void PMBlock(IUser client, String onoff)
        {
            if (client.Level > ILevel.Regular || Settings.General)
            {
                if (onoff == "on")
                {
                    PMBlocking.Add(client);
                    Server.Print(Template.Text(Category.PmBlocking, 0).Replace("+n", client.Name), true);
                }
                else if (onoff == "off")
                {
                    PMBlocking.Remove(client);
                    Server.Print(Template.Text(Category.PmBlocking, 1).Replace("+n", client.Name), true);
                }
            }
        }

        public static void Shout(IUser client, String text)
        {
            if (client.Level > ILevel.Regular || Settings.General)
                Server.Print(Template.Text(Category.Messaging, 0).Replace("+n", client.Name).Replace("+t", text), true);
        }

        [CommandLevel("adminmsg", ILevel.Moderator)]
        public static void AdminMsg(IUser client, String text)
        {
            if (client.Level >= Server.GetLevel("adminmsg"))
            {
                String str = Template.Text(Category.Messaging, 1).Replace("+n", client.Name).Replace("+t", text);
                Server.Users.Ares(x => { if (x.Level > ILevel.Regular) x.Print(str); });
                Server.Users.Web(x => { if (x.Level > ILevel.Regular) x.Print(str); });

                if (Server.Link.IsLinked)
                    Server.Link.ForEachLeaf(x => x.Print(ILevel.Moderator, str));
            }
        }

        public static void Whisper(IUser client, IUser target, String text)
        {
            if (client.Level > ILevel.Regular || Settings.General)
                if (target != null)
                {
                    client.Print(Template.Text(Category.Messaging, 2).Replace("+n", target.Name).Replace("+t", text));
                    target.Print(Template.Text(Category.Messaging, 3).Replace("+n", client.Name).Replace("+t", text));
                }
        }

        [CommandLevel("link", ILevel.Host)]
        public static void Link(IUser client, String args)
        {
            if (client.Level >= Server.GetLevel("link"))
            {
                IHashlinkRoom room = Server.Hashlinks.Decrypt(args);

                if (room != null)
                {
                    Server.Print(Template.Text(Category.Linking, 5).Replace("+n", room.Name));
                    Server.Link.Connect(args);
                }
            }
        }

        [CommandLevel("unlink", ILevel.Host)]
        public static void Unlink(IUser client)
        {
            if (client.Level >= Server.GetLevel("unlink"))
                if (Server.Link.IsLinked)
                {
                    Server.Print(Template.Text(Category.Linking, 6));
                    Server.Link.Disconnect();
                }
        }

        [CommandLevel("admins", ILevel.Moderator)]
        public static void Admins(IUser client)
        {
            if (client.Level >= Server.GetLevel("admins"))
            {
                Server.Print(Template.Text(Category.AdminList, 0).Replace("+n", client.Name), true);
                Server.Print(String.Empty, true);

                Server.Users.All(x =>
                {
                    if (x.Level > ILevel.Regular)
                        Server.Print(Template.Text(Category.AdminList, 1).Replace("+n", x.Name).Replace("+l", ((byte)x.Level).ToString()), true);
                });

                Server.Print(String.Empty, true);
                Server.Print(Template.Text(Category.AdminList, 2), true);
            }
        }

        public static void AddAutologin(IUser admin, IUser target, String args)
        {
            if (!admin.Link.IsLinked)
                if (admin.Owner)
                    if (target != null)
                        if (!target.Link.IsLinked)
                        {
                            byte b;

                            if (byte.TryParse(args, out b))
                                if (b >= 1 && b <= 3)
                                {
                                    AutoLogin.Add(target, (ILevel)b);
                                    Server.Print(Template.Text(Category.AdminLogin, 4).Replace("+n", target.Name).Replace("+l", b.ToString()), true);
                                    target.SetLevel((ILevel)b);
                                }
                        }
        }

        public static void RemAutologin(IUser admin, String args)
        {
            if (!admin.Link.IsLinked)
                if (admin.Owner)
                {
                    int i;

                    if (int.TryParse(args, out i))
                    {
                        String name = AutoLogin.Remove(i);

                        if (!String.IsNullOrEmpty(name))
                            Server.Print(Template.Text(Category.AdminLogin, 5).Replace("+n", name), true);
                    }
                }
        }

        public static void Autologins(IUser admin)
        {
            if (!admin.Link.IsLinked)
                if (admin.Owner)
                    AutoLogin.ListAdmins(admin);
        }

        [CommandLevel("roomsearch", ILevel.Administrator)]
        public static void RoomSearch(IUser client, String args)
        {
            if (client.Level >= Server.GetLevel("roomsearch"))
                if (!Server.Channels.Enabled)
                    Server.Print(Template.Text(Category.RoomSearch, 0), true);
                else if (!Server.Channels.Available)
                    Server.Print(Template.Text(Category.RoomSearch, 1), true);
                else
                {
                    List<IChannelItem> items = new List<IChannelItem>();
                    String str = args.ToUpper();

                    Server.Channels.ForEach(x =>
                    {
                        if (x.Name.ToUpper().Contains(str))
                            items.Add(x);
                    });

                    items.Sort((y, x) => x.Users.CompareTo(y.Users));

                    if (items.Count > 5)
                        items = items.GetRange(0, 5);

                    if (items.Count == 0)
                        Server.Print(Template.Text(Category.RoomSearch, 2).Replace("+n", str), true);
                    else
                    {
                        Server.Print(Template.Text(Category.RoomSearch, 3).Replace("+n", str), true);

                        foreach (IChannelItem i in items)
                        {
                            Server.Print(String.Empty, true);
                            Server.Print(Template.Text(Category.RoomSearch, 4).Replace("+n", i.Name), true);
                            Server.Print(Template.Text(Category.RoomSearch, 5).Replace("+t", i.Topic), true);
                            Server.Print(Template.Text(Category.RoomSearch, 6).Replace("+l",
                                Helpers.LanguageCodeToString(i.Language)).Replace("+s", i.Version).Replace("+u", i.Users.ToString()), true);

                            IHashlinkRoom obj = new Hashlink
                            {
                                IP = i.IP,
                                Name = i.Name,
                                Port = i.Port
                            };

                            String hashlink = Server.Hashlinks.Encrypt(obj);

                            if (!String.IsNullOrEmpty(hashlink))
                                Server.Print(Template.Text(Category.RoomSearch, 7).Replace("+h", "arlnk://" + hashlink), true);
                        }
                    }
                }
        }

        [CommandLevel("mtimeout", ILevel.Host)]
        public static void MTimeout(IUser client, String args)
        {
            if (client.Level >= Server.GetLevel("mtimeout"))
            {
                byte b;

                if (byte.TryParse(args, out b))
                    if (b < 100)
                    {
                        Settings.MuzzleTimeout = b;
                        Server.Print(Template.Text(Category.Timeouts, 0).Replace("+n",
                            client.Name).Replace("+i", b == 0 ? "unlimited" : (b + " minutes")), true);
                    }
            }
        }

        [CommandLevel("redirect", ILevel.Administrator)]
        public static void Redirect(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("redirect"))
                if (target != null)
                    if (!(admin.Link.IsLinked && !target.Link.IsLinked))
                        if (target.Level < admin.Level)
                        {
                            IHashlinkRoom hr = Server.Hashlinks.Decrypt(args.Trim());

                            if (hr != null)
                            {
                                Server.Print(Template.Text(Category.AdminAction, 20).Replace("+n",
                                    target.Name).Replace("+a", admin.Name).Replace("+r", hr.Name), true);

                                target.Redirect(args.Trim());
                            }
                        }
        }

        [CommandLevel("sharefiles", ILevel.Host)]
        public static void ShareFiles(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("sharefiles"))
            {
                if (args == "on")
                {
                    Settings.ShareFileMonitoring = true;
                    Server.Print(Template.Text(Category.EnableDisable, 0).Replace("+n", admin.Name));
                }
                else if (args == "off")
                {
                    Settings.ShareFileMonitoring = false;
                    Server.Print(Template.Text(Category.EnableDisable, 1).Replace("+n", admin.Name));
                }
            }
        }

        [CommandLevel("idle", ILevel.Host)]
        public static void IdleMonitoring(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("idle"))
            {
                if (args == "on")
                {
                    Settings.IdleMonitoring = true;
                    Server.Print(Template.Text(Category.EnableDisable, 2).Replace("+n", admin.Name));
                }
                else if (args == "off")
                {
                    Settings.IdleMonitoring = false;
                    Server.Print(Template.Text(Category.EnableDisable, 3).Replace("+n", admin.Name));
                }
            }
        }

        [CommandLevel("clock", ILevel.Administrator)]
        public static void Clock(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("clock"))
            {
                if (args == "on")
                {
                    Settings.Clock = true;
                    Topics.EnableClock();
                    Server.Print(Template.Text(Category.EnableDisable, 4).Replace("+n", admin.Name));
                }
                else if (args == "off")
                {
                    Settings.Clock = false;
                    Topics.DisableClock();
                    Server.Print(Template.Text(Category.EnableDisable, 5).Replace("+n", admin.Name));
                }
            }
        }

        [CommandLevel("addtopic", ILevel.Administrator)]
        public static void AddTopic(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("addtopic"))
            {
                if (admin.Vroom == 0)
                {
                    Server.Chatroom.Topic = args;

                    if (Settings.Clock)
                    {
                        String topic = Topics.ClockTopic;

                        Server.Users.Ares(x =>
                        {
                            if (x.Vroom == admin.Vroom)
                                x.Topic(topic);
                        });

                        Server.Users.Web(x =>
                        {
                            if (x.Vroom == admin.Vroom)
                                x.Topic(topic);
                        });
                    }
                }
                else
                {
                    Topics.AddTopic(admin.Vroom, args);

                    Server.Users.Ares(x =>
                    {
                        if (x.Vroom == admin.Vroom)
                            x.Topic(args);
                    });

                    Server.Users.Web(x =>
                    {
                        if (x.Vroom == admin.Vroom)
                            x.Topic(args);
                    });
                }

                Server.Print(Template.Text(Category.Topics, 0).Replace("+n",
                    admin.Name).Replace("+v", admin.Vroom.ToString()));
            }
        }

        [CommandLevel("remtopic", ILevel.Administrator)]
        public static void RemTopic(IUser admin)
        {
            if (admin.Level >= Server.GetLevel("remtopic"))
            {
                if (admin.Vroom > 0)
                {
                    String args = Settings.Clock ? Topics.ClockTopic : Server.Chatroom.Topic;
                    Topics.RemTopic(admin.Vroom);

                    Server.Users.Ares(x =>
                    {
                        if (x.Vroom == admin.Vroom)
                            x.Topic(args);
                    });

                    Server.Users.Web(x =>
                    {
                        if (x.Vroom == admin.Vroom)
                            x.Topic(args);
                    });
                }

                Server.Print(Template.Text(Category.Topics, 1).Replace("+n",
                    admin.Name).Replace("+v", admin.Vroom.ToString()));
            }
        }

        [CommandLevel("greetmsg", ILevel.Host)]
        public static void GreetMsg(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("greetmsg"))
                if (args == "on")
                {
                    Settings.GreetMsg = true;
                    Server.Print(Template.Text(Category.EnableDisable, 6).Replace("+n", admin.Name));
                }
                else if (args == "off")
                {
                    Settings.GreetMsg = false;
                    Server.Print(Template.Text(Category.EnableDisable, 7).Replace("+n", admin.Name));
                }
        }

        [CommandLevel("addgreetmsg", ILevel.Host)]
        public static void AddGreetMsg(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("addgreetmsg"))
                if (!String.IsNullOrEmpty(args))
                    if (args.Length > 0)
                    {
                        Greets.Add(args);
                        Server.Print(Template.Text(Category.Greetings, 0).Replace("+n", admin.Name));
                    }
        }

        [CommandLevel("remgreetmsg", ILevel.Host)]
        public static void RemGreetMsg(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("remgreetmsg"))
                if (!String.IsNullOrEmpty(args))
                {
                    int i;

                    if (int.TryParse(args, out i))
                    {
                        String str = Greets.Remove(i);

                        if (!String.IsNullOrEmpty(str))
                            Server.Print(Template.Text(Category.Greetings, 1).Replace("+n", admin.Name));
                    }
                }
        }

        [CommandLevel("listgreetmsg", ILevel.Host)]
        public static void ListGreetMsg(IUser admin)
        {
            if (admin.Level >= Server.GetLevel("listgreetmsg"))
                Greets.List(admin);
        }

        [CommandLevel("pmgreetmsg", ILevel.Host)]
        public static void PMGreetMsg(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("pmgreetmsg"))
            {
                if (args == "on")
                {
                    Settings.PMGreetMsg = true;
                    Server.Print(Template.Text(Category.EnableDisable, 8).Replace("+n", admin.Name));
                }
                else if (args == "off")
                {
                    Settings.PMGreetMsg = false;
                    Server.Print(Template.Text(Category.EnableDisable, 9).Replace("+n", admin.Name));
                }
                else if (!String.IsNullOrEmpty(args))
                {
                    Settings.PMGreetMsg = true;
                    Greets.SetPM(args);
                    Server.Print(Template.Text(Category.Greetings, 2).Replace("+n", admin.Name));
                }
            }
        }

        [CommandLevel("caps", ILevel.Administrator)]
        public static void Caps(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("caps"))
            {
                if (args == "on")
                {
                    Settings.CapsMonitoring = true;
                    Server.Print(Template.Text(Category.EnableDisable, 10).Replace("+n", admin.Name));
                }
                else if (args == "off")
                {
                    Settings.CapsMonitoring = false;
                    Server.Print(Template.Text(Category.EnableDisable, 11).Replace("+n", admin.Name));
                }
            }
        }

        [CommandLevel("anon", ILevel.Administrator)]
        public static void Anon(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("anon"))
            {
                if (args == "on")
                {
                    Settings.AnonMonitoring = true;
                    Server.Print(Template.Text(Category.EnableDisable, 12).Replace("+n", admin.Name));
                }
                else if (args == "off")
                {
                    Settings.AnonMonitoring = false;
                    Server.Print(Template.Text(Category.EnableDisable, 13).Replace("+n", admin.Name));
                }
            }
        }

        [CommandLevel("customnames", ILevel.Administrator)]
        public static void CustomNames(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("customnames"))
            {
                if (args == "on")
                {
                    Server.Chatroom.CustomNamesEnabled = true;
                    Server.Print(Template.Text(Category.EnableDisable, 14).Replace("+n", admin.Name));
                }
                else if (args == "off")
                {
                    Server.Chatroom.CustomNamesEnabled = false;
                    Server.Print(Template.Text(Category.EnableDisable, 15).Replace("+n", admin.Name));
                }
            }
        }

        [CommandLevel("general", ILevel.Administrator)]
        public static void General(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("general"))
            {
                if (args == "on")
                {
                    Settings.General = true;
                    Server.Print(Template.Text(Category.EnableDisable, 16).Replace("+n", admin.Name));
                }
                else if (args == "off")
                {
                    Settings.General = false;
                    Server.Print(Template.Text(Category.EnableDisable, 17).Replace("+n", admin.Name));
                }
            }
        }

        [CommandLevel("url", ILevel.Host)]
        public static void Url(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("url"))
            {
                if (args == "on")
                {
                    Settings.Url = true;
                    Server.Print(Template.Text(Category.EnableDisable, 18).Replace("+n", admin.Name));
                    Urls.EnableDisable(true);
                }
                else if (args == "off")
                {
                    Settings.Url = false;
                    Server.Print(Template.Text(Category.EnableDisable, 19).Replace("+n", admin.Name));
                    Urls.EnableDisable(false);
                }
            }
        }

        [CommandLevel("addurl", ILevel.Administrator)]
        public static void AddUrl(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("addurl"))
            {
                int i = args.IndexOf(" ");

                if (i > 0)
                {
                    String addr = args.Substring(0, i);
                    String text = args.Substring(i + 1);
                    Urls.Add(addr, text);
                    Server.Print(Template.Text(Category.Urls, 1).Replace("+n", admin.Name));
                }
            }
        }

        [CommandLevel("remurl", ILevel.Administrator)]
        public static void RemUrl(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("remurl"))
            {
                int i;

                if (int.TryParse(args, out i))
                    if (Urls.Remove(i))
                        Server.Print(Template.Text(Category.Urls, 2).Replace("+n", admin.Name));
            }
        }

        [CommandLevel("listurls", ILevel.Administrator)]
        public static void ListUrls(IUser admin)
        {
            if (admin.Level >= Server.GetLevel("listurls"))
                Urls.List(admin);
        }


    }
}
