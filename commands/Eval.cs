/*
    sb0t ares chat server
    Copyright (C) 2016  AresChat

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System;
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
            client.Print(client.Name + ": " + client.ID);
        }

        public static void Info(IUser client)
        {
            client.Print(Template.Text(Category.Info, 0).Replace("+r", Server.Chatroom.Name));
            client.Print(String.Empty);

            Server.Users.Ares(x =>
            {
                if (!x.Cloaked)
                    client.Print(Template.Text(Category.Info, 1).Replace("+n", x.Name).Replace("+v", x.Vroom.ToString()).Replace("+i", x.ID.ToString()));
            });

            Server.Users.Web(x =>
            {
                client.Print(Template.Text(Category.Info, 1).Replace("+n", x.Name).Replace("+v", x.Vroom.ToString()).Replace("+i", x.ID.ToString()));
            });

            if (Server.Link.IsLinked)
                Server.Link.ForEachLeaf(l =>
                {
                    client.Print(String.Empty);
                    client.Print(Template.Text(Category.Info, 0).Replace("+r", l.Name));
                    client.Print(String.Empty);
                    l.ForEachUser(x => client.Print(Template.Text(Category.Info, 1).Replace("+n", x.Name).Replace("+v", x.Vroom.ToString()).Replace("+i", "linked")));
                });
        }

        [CommandLevel("ban", ILevel.Administrator)]
        public static void Ban(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("ban"))
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        if (!target.Link.IsLinked && admin.Link.IsLinked)
                            if (target.Level != ILevel.Regular)
                                return;

                        Server.Print(Template.Text(Category.AdminAction, 0).Replace("+n",
                            target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

                        commands.BanStats.Add(admin, target);
                        target.Ban();
                    }
                    else admin.Print(Template.Text(Category.Notification, 29).Replace("+n", target.Name));
        }

        [CommandLevel("ban10", ILevel.Moderator)]
        public static void Ban10(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("ban10"))
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        if (!target.Link.IsLinked && admin.Link.IsLinked)
                            if (target.Level != ILevel.Regular)
                                return;

                        Server.Print(Template.Text(Category.AdminAction, 21).Replace("+n",
                            target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

                        commands.BanStats.Add(admin, target);
                        Bans.AddBan(target, BanDuration.Ten);
                        target.Ban();
                    }
                    else admin.Print(Template.Text(Category.Notification, 29).Replace("+n", target.Name));
        }

        [CommandLevel("ban60", ILevel.Administrator)]
        public static void Ban60(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("ban60"))
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        if (!target.Link.IsLinked && admin.Link.IsLinked)
                            if (target.Level != ILevel.Regular)
                                return;

                        Server.Print(Template.Text(Category.AdminAction, 22).Replace("+n",
                            target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

                        commands.BanStats.Add(admin, target);
                        Bans.AddBan(target, BanDuration.Sixty);
                        target.Ban();
                    }
                    else admin.Print(Template.Text(Category.Notification, 29).Replace("+n", target.Name));
        }

        [CommandLevel("unban", ILevel.Administrator)]
        public static void Unban(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("unban"))
            {
                String name = null;
                bool done = false;

                Server.Users.Banned(x =>
                {
                    if (!done)
                        if (x.Name == args.Replace("\"", String.Empty))
                        {
                            name = x.Name;
                            x.Unban();
                            done = true;
                        }
                });

                if (name == null && args.Length > 0)
                {
                    done = false;

                    Server.Users.Banned(x =>
                    {
                        if (!done)
                            if (x.Name.StartsWith(args.Replace("\"", String.Empty)))
                            {
                                name = x.Name;
                                x.Unban();
                                done = true;
                            }
                    });

                    if (name == null)
                    {
                        uint counter = 0;
                        uint target;

                        if (uint.TryParse(args, out target))
                            Server.Users.Banned(x =>
                            {
                                if (counter++ == target)
                                {
                                    name = x.Name;
                                    x.Unban();
                                }
                            });
                    }
                }

                if (name != null)
                {
                    Bans.RemoveBan(name);
                    Server.Print(Template.Text(Category.AdminAction, 1).Replace("+n", name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name), true);
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
                    admin.Print((counter++) + " - " + x.Name + " [" + x.ExternalIP + "]");
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
                    {
                        if (!target.Link.IsLinked && admin.Link.IsLinked)
                            if (target.Level != ILevel.Regular)
                                return;

                        Server.Print(Template.Text(Category.AdminAction, 2).Replace("+n",
                            target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

                        target.Disconnect();
                    }
                    else admin.Print(Template.Text(Category.Notification, 29).Replace("+n", target.Name));
        }

        [CommandLevel("muzzle", ILevel.Moderator)]
        public static void Muzzle(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("muzzle"))
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        if (!target.Link.IsLinked && admin.Link.IsLinked)
                            if (target.Level != ILevel.Regular)
                                return;

                        Server.Print(Template.Text(Category.AdminAction, 3).Replace("+n",
                            target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

                        target.Muzzled = true;
                        Muzzles.AddMuzzle(target);
                    }
                    else admin.Print(Template.Text(Category.Notification, 29).Replace("+n", target.Name));
        }

        public static void Unmuzzle(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("muzzle"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 4).Replace("+n",
                        target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

                    target.Muzzled = false;
                    Muzzles.RemoveMuzzle(target);
                }
        }

        [CommandLevel("customname", ILevel.Moderator)]
        public static void CustomName(IUser admin, IUser target, String args)
        {
            if (target == null || args.Length < 2)
                return;

            String check = args.ToUpper();

            if (check.Contains("CHATROOM") || check.Contains("HTTP") || check.Contains("WWW") || check.Contains("ARLNK"))
                return;

            if (admin.Name == target.Name)
            {
                if (admin.Level > ILevel.Regular || Settings.General)
                {
                    target.CustomName = args;
                    commands.CustomNames.UpdateCustomName(target);
                    Server.Print(Template.Text(Category.AdminAction, 5).Replace("+n", target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name), true);
                }
            }
            else if (admin.Level >= Server.GetLevel("customname"))
            {
                target.CustomName = args;
                commands.CustomNames.UpdateCustomName(target);
                Server.Print(Template.Text(Category.AdminAction, 5).Replace("+n", target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name), true);
            }
            else admin.Print(Template.Text(Category.Notification, 29).Replace("+n", target.Name));
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
                    Server.Print(Template.Text(Category.AdminAction, 6).Replace("+n", target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name), true);
                }
            }
            else if (admin.Level >= Server.GetLevel("uncustomname"))
            {
                target.CustomName = null;
                commands.CustomNames.UpdateCustomName(target);
                Server.Print(Template.Text(Category.AdminAction, 6).Replace("+n", target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name), true);
            }
            else admin.Print(Template.Text(Category.Notification, 29).Replace("+n", target.Name));
        }

        [CommandLevel("kewltext", ILevel.Moderator)]
        public static void AddKewlText(IUser admin, IUser target)
        {
            if (admin.Level >= Server.GetLevel("kewltext"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 7).Replace("+n", target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name), true);
                    KewlText.Add(target);
                }
        }

        public static void RemKewlText(IUser admin, IUser target)
        {
            if (admin.Level >= Server.GetLevel("kewltext"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 8).Replace("+n", target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name), true);
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
                            target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

                        Lowered.Add(target);
                    }
                    else admin.Print(Template.Text(Category.Notification, 29).Replace("+n", target.Name));
        }

        [CommandLevel("unlower", ILevel.Moderator)]
        public static void Unlower(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("unlower"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 10).Replace("+n",
                        target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

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
                            target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

                        Kiddied.Add(target);
                    }
                    else admin.Print(Template.Text(Category.Notification, 29).Replace("+n", target.Name));
        }

        public static void Unkiddy(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("kiddy"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 12).Replace("+n",
                        target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name) + (args.Length == 0 ? "" : (" [" + args + "]")), true);

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
                        Server.Print(Template.Text(Category.AdminAction, 13).Replace("+n", target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name), true);
                        commands.Echo.Add(target, args);
                    }
                    else admin.Print(Template.Text(Category.Notification, 29).Replace("+n", target.Name));
        }

        public static void Unecho(IUser admin, IUser target)
        {
            if (admin.Level >= Server.GetLevel("echo"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 14).Replace("+n", target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name), true);
                    commands.Echo.Remove(target);
                }
        }

        [CommandLevel("paint", ILevel.Moderator)]
        public static void Paint(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("paint"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 15).Replace("+n", target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name), true);
                    commands.Paint.Add(target, args);
                }
        }

        public static void Unpaint(IUser admin, IUser target)
        {
            if (admin.Level >= Server.GetLevel("paint"))
                if (target != null)
                {
                    Server.Print(Template.Text(Category.AdminAction, 16).Replace("+n", target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name), true);
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
                    Server.Print(Template.Text(Category.AdminAction, 17).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : client.Name).Replace("+r", str), true);
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
                    Server.Print(Template.Text(Category.AdminAction, 18).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : client.Name).Replace("+r", str), true);
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
                Server.Print(Template.Text(Category.AdminAction, 19).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : client.Name), true);
            }
        }

        public static void PMBlock(IUser client, String onoff)
        {
            if (client.Level > ILevel.Regular || Settings.General)
                if (onoff == "on" && !PMBlocking.IsBlocking(client))
                {
                    PMBlocking.Add(client);
                    Server.Print(Template.Text(Category.PmBlocking, 0).Replace("+n", client.Name), true);
                }
                else if (onoff == "off" && PMBlocking.IsBlocking(client))
                {
                    PMBlocking.Remove(client);
                    Server.Print(Template.Text(Category.PmBlocking, 1).Replace("+n", client.Name), true);
                }
        }

        public static void Locate(IUser client)
        {
            if (client.Level > ILevel.Regular || Settings.General)
            {
                bool empty = true;

                Server.Users.All(x =>
                {
                    if (!x.Cloaked)
                        if (x.Vroom > 0)
                        {
                            if (empty)
                            {
                                empty = false;
                                client.Print(Template.Text(Category.Locate, 0));
                                client.Print(String.Empty);
                            }

                            client.Print(Template.Text(Category.Locate, 1).Replace("+n", x.Name).Replace("+v", x.Vroom.ToString()));
                        }
                });

                if (!empty)
                {
                    client.Print(String.Empty);
                    client.Print(Template.Text(Category.Locate, 2));
                }
                else client.Print(Template.Text(Category.Locate, 3));
            }
        }

        public static void ViewMotd(IUser client)
        {
            if (client.Level > ILevel.Regular || Settings.General)
                Motd.ViewMOTD(client);
        }

        [CommandLevel("pmroom", ILevel.Host)]
        public static void PMRoom(IUser client, String args)
        {
            if (client.Level >= Server.GetLevel("pmroom"))
                Server.Users.Ares(x => x.PM(client.Name, args));
        }

        public static void HostKill(IUser admin, IUser target)
        {
            if (admin.Level == ILevel.Host)
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        if (!target.Link.IsLinked && admin.Link.IsLinked)
                            if (target.Level != ILevel.Regular)
                                return;

                        target.Disconnect();
                    }
                    else admin.Print(Template.Text(Category.Notification, 29).Replace("+n", target.Name));
        }

        public static void HostBan(IUser admin, IUser target)
        {
            if (admin.Level == ILevel.Host)
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        if (!target.Link.IsLinked && admin.Link.IsLinked)
                            if (target.Level != ILevel.Regular)
                                return;

                        target.Ban();
                    }
                    else admin.Print(Template.Text(Category.Notification, 29).Replace("+n", target.Name));
        }

        public static void HostUnban(IUser admin, String args)
        {
            if (admin.Level == ILevel.Host)
            {
                String name = null;
                bool done = false;

                Server.Users.Banned(x =>
                {
                    if (!done)
                        if (x.Name == args.Replace("\"", String.Empty))
                        {
                            name = x.Name;
                            x.Unban();
                            done = true;
                        }
                });

                if (name == null && args.Length > 0)
                {
                    done = false;

                    Server.Users.Banned(x =>
                    {
                        if (!done)
                            if (x.Name.StartsWith(args.Replace("\"", String.Empty)))
                            {
                                name = x.Name;
                                x.Unban();
                                done = true;
                            }
                    });

                    if (name == null)
                    {
                        uint counter = 0;
                        uint target;

                        if (uint.TryParse(args, out target))
                            Server.Users.Banned(x =>
                            {
                                if (counter++ == target)
                                {
                                    name = x.Name;
                                    x.Unban();
                                }
                            });
                    }
                }

                if (name != null)
                    Bans.RemoveBan(name);
            }
        }

        public static void HostMuzzle(IUser admin, IUser target)
        {
            if (admin.Level == ILevel.Host)
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        if (!target.Link.IsLinked && admin.Link.IsLinked)
                            if (target.Level != ILevel.Regular)
                                return;

                        target.Muzzled = true;
                        Muzzles.AddMuzzle(target);
                    }
                    else admin.Print(Template.Text(Category.Notification, 29).Replace("+n", target.Name));
        }

        public static void HostUnmuzzle(IUser admin, IUser target)
        {
            if (admin.Level == ILevel.Host)
                if (target != null)
                {
                    target.Muzzled = false;
                    Muzzles.RemoveMuzzle(target);
                }
        }

        public static void HostCBans(IUser admin)
        {
            if (admin.Level == ILevel.Host)
            {
                Server.ClearBans();
                RangeBans.Clear();
                Muzzles.Clear();
                Kiddied.Clear();
                commands.Echo.Clear();
                commands.Paint.Clear();
                Lowered.Clear();
            }
        }

        public static void HostClone(IUser admin, IUser target, String args)
        {
            if (admin.Level == ILevel.Host)
            {
                if (args.StartsWith("/me ") && args.Length > 4)
                    target.SendEmote(args.Substring(4));
                else if (args.Length > 0)
                    target.SendText(args);
            }
        }

        [CommandLevel("loadmotd", ILevel.Host)]
        public static void LoadMotd(IUser admin)
        {
            if (admin.Level >= Server.GetLevel("loadmotd"))
            {
                Motd.LoadMOTD();
                Server.Print(Template.Text(Category.Notification, 8).Replace("+n", admin.Name));
            }
        }

        [CommandLevel("disableadmins", ILevel.Host)]
        public static void EnableAdmins(IUser admin)
        {
            if (admin.Level >= Server.GetLevel("disableadmins"))
            {
                Settings.DisableAdmins = false;
                Server.Print(ILevel.Moderator, Template.Text(Category.Notification, 9).Replace("+n", admin.Name));
            }
        }

        [CommandLevel("disableadmins", ILevel.Host)]
        public static void DisableAdmins(IUser admin)
        {
            if (admin.Level >= Server.GetLevel("disableadmins"))
            {
                Settings.DisableAdmins = true;
                Server.Print(ILevel.Moderator, Template.Text(Category.Notification, 10).Replace("+n", admin.Name));
            }
        }

        [CommandLevel("stealth", ILevel.Administrator)]
        public static void Stealth(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("stealth"))
                if (args == "on" && !Settings.Stealth)
                {
                    Settings.Stealth = true;
                    Server.Print(ILevel.Moderator, Template.Text(Category.EnableDisable, 26).Replace("+n", admin.Name));
                }
                else if (args == "off" && Settings.Stealth)
                {
                    Settings.Stealth = false;
                    Server.Print(ILevel.Moderator, Template.Text(Category.EnableDisable, 27).Replace("+n", admin.Name));
                }
        }

        [CommandLevel("cloak", ILevel.Host)]
        public static void Cloak(IUser admin, String args)
        {
            if (!Server.Link.IsLinked)
                if (admin.Level >= Server.GetLevel("cloak"))
                    if (args == "on" && !admin.Cloaked)
                    {
                        admin.Cloaked = true;
                        Server.Print(ILevel.Moderator, Template.Text(Category.Notification, 11).Replace("+n", admin.Name));
                    }
                    else if (args == "off" && admin.Cloaked)
                    {
                        admin.Cloaked = false;
                        Server.Print(ILevel.Moderator, Template.Text(Category.Notification, 12).Replace("+n", admin.Name));
                    }
        }

        [CommandLevel("disableavatar", ILevel.Moderator)]
        public static void DisableAvatar(IUser admin, IUser target)
        {
            if (admin.Level >= Server.GetLevel("disableavatar"))
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        if (!target.Link.IsLinked && admin.Link.IsLinked)
                            if (target.Level != ILevel.Regular)
                                return;

                        target.Avatar = null;
                        AvatarPMManager.AddAvatar(target);
                        Server.Print(Template.Text(Category.AdminAction, 23).Replace("+n", target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name), true);
                    }
                    else admin.Print(Template.Text(Category.Notification, 29).Replace("+n", target.Name));
        }

        [CommandLevel("changemessage", ILevel.Moderator)]
        public static void ChangeMessage(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("changemessage"))
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        if (!target.Link.IsLinked && admin.Link.IsLinked)
                            if (target.Level != ILevel.Regular)
                                return;

                        target.PersonalMessage = args;
                        AvatarPMManager.AddPM(target, args);
                        Server.Print(Template.Text(Category.AdminAction, 24).Replace("+n", target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name), true);
                    }
                    else admin.Print(Template.Text(Category.Notification, 29).Replace("+n", target.Name));
        }

        [CommandLevel("clearscreen", ILevel.Moderator)]
        public static void ClearScreen(IUser admin)
        {
            if (admin.Level >= Server.GetLevel("clearscreen"))
            {
                Server.Users.Web(x =>
                {
                    for (int i = 0; i < 500; i++)
                        x.Print(String.Empty);
                });

                Server.Users.Ares(x =>
                {
                    if (!x.SupportsHTML)
                    {
                        for (int i = 0; i < 500; i++)
                            x.Print(String.Empty);
                    }
                    else
                    {
                        for (int i = 0; i < 400; i++)
                            x.Print("\x000500\x000300.");
                    }
                });

                Server.Print(Template.Text(Category.Notification, 14).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
            }
        }

        [CommandLevel("banstats", ILevel.Administrator)]
        public static void BanStats(IUser admin)
        {
            if (admin.Level >= Server.GetLevel("banstats"))
                commands.BanStats.View(admin);
        }

        [CommandLevel("colors", ILevel.Host)]
        public static void Colors(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("colors"))
                if (args == "on" && !Settings.Colors)
                {
                    Settings.Colors = true;
                    Server.Print(Template.Text(Category.EnableDisable, 28).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
                else if (args == "off" && Settings.Colors)
                {
                    Settings.Colors = false;
                    Server.Print(Template.Text(Category.EnableDisable, 29).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
        }

        [CommandLevel("vspy", ILevel.Administrator)]
        public static void Vspy(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("vspy"))
                if (args == "on" && !commands.VSpy.IsVspy(admin))
                {
                    commands.VSpy.Add(admin);
                    Server.Print(ILevel.Moderator, Template.Text(Category.Notification, 20).Replace("+n", admin.Name), true);
                }
                else if (args == "off" && commands.VSpy.IsVspy(admin))
                {
                    commands.VSpy.Remove(admin);
                    Server.Print(ILevel.Moderator, Template.Text(Category.Notification, 21).Replace("+n", admin.Name), true);
                }
        }

        [CommandLevel("customnames", ILevel.Host)]
        public static void CustomNames(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("customnames"))
                if (args == " on" && !Server.Chatroom.CustomNamesEnabled)
                {
                    Server.Chatroom.CustomNamesEnabled = true;
                    Server.Print(Template.Text(Category.EnableDisable, 14).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
                else if (args == " off" && Server.Chatroom.CustomNamesEnabled)
                {
                    Server.Chatroom.CustomNamesEnabled = false;
                    Server.Print(Template.Text(Category.EnableDisable, 15).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
                else if (String.IsNullOrEmpty(args))
                {
                    Server.Users.All(x =>
                    {
                        if (!String.IsNullOrEmpty(x.CustomName))
                            admin.Print(x.Name + ": " + x.CustomName);
                    });
                }
        }

        [CommandLevel("urban", ILevel.Moderator)]
        public static void Urban(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("urban"))
                UrbanDictionary.Lookup(args);
        }

        [CommandLevel("define", ILevel.Moderator)]
        public static void Define(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("define"))
                DefineDictionary.Lookup(args);
        }

        [CommandLevel("trace", ILevel.Administrator)]
        public static void Trace(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("trace"))
                if (target != null)
                    commands.Trace.Lookup(target);
                else
                {
                    String str = args.Replace("\"", String.Empty).Trim();
                    System.Net.IPAddress ip;

                    if (System.Net.IPAddress.TryParse(str, out ip))
                        commands.Trace.Lookup(ip.ToString());
                }
        }

        [CommandLevel("whois", ILevel.Moderator)]
        public static void Whois(IUser admin, IUser target)
        {
            if (admin.Level >= Server.GetLevel("whois"))
                if (target != null)
                {
                    admin.Print(Template.Text(Category.Whois, 0).Replace("+n", target.Name));
                    admin.Print(Template.Text(Category.Whois, 1).Replace("+n", target.OrgName));
                    admin.Print(Template.Text(Category.Whois, 2).Replace("+n", target.ExternalIP.ToString()));
                    admin.Print(Template.Text(Category.Whois, 3).Replace("+n", target.LocalIP.ToString()));
                    admin.Print(Template.Text(Category.Whois, 4).Replace("+n", target.DataPort.ToString()));
                    admin.Print(Template.Text(Category.Whois, 5).Replace("+n", target.Version));
                    admin.Print(Template.Text(Category.Whois, 6).Replace("+n", target.Vroom.ToString()));
                    admin.Print(Template.Text(Category.Whois, 7).Replace("+n", target.Link.IsLinked ? "linked" : target.ID.ToString()));
                    admin.Print(Template.Text(Category.Whois, 8).Replace("+n", target.Link.IsLinked.ToString()));
                    admin.Print(Template.Text(Category.Whois, 9).Replace("+n", target.Registered.ToString()));
                }
        }

        [CommandLevel("announce", ILevel.Moderator)]
        public static void Announce(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("announce"))
            {
                Server.Print(args, true);
                Server.Print(ILevel.Moderator, Template.Text(Category.Notification, 19).Replace("+a", admin.Name), true);
            }
        }

        [CommandLevel("clone", ILevel.Moderator)]
        public static void Clone(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("clone"))
                if (target != null)
                {
                    if (args.StartsWith("/me "))
                        target.SendEmote(args.Substring(4));
                    else
                        target.SendText(args);

                    Server.Print(ILevel.Moderator, Template.Text(Category.Notification,
                        15).Replace("+n", target.Name).Replace("+a", admin.Name), true);
                }
        }

        [CommandLevel("move", ILevel.Administrator)]
        public static void Move(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("move"))
                if (target != null)
                {
                    ushort u;

                    if (ushort.TryParse(args, out u))
                    {
                        target.Vroom = u;

                        if (target.Vroom == u)
                            Server.Print(ILevel.Moderator, Template.Text(Category.Notification,
                                16).Replace("+n", target.Name).Replace("+a", admin.Name).Replace("+v", target.Vroom.ToString()), true);
                    }
                }
        }

        [CommandLevel("changename", ILevel.Administrator)]
        public static void ChangeName(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("changename"))
                if (target != null)
                {
                    String org_name = target.Name;
                    target.Name = args;

                    if (target.Name == args)
                        Server.Print(ILevel.Moderator, Template.Text(Category.Notification,
                            17).Replace("+n", org_name).Replace("+a", admin.Name), true);
                }
        }

        [CommandLevel("oldname", ILevel.Administrator)]
        public static void OldName(IUser admin, IUser target)
        {
            if (admin.Level >= Server.GetLevel("oldname"))
                if (target != null)
                {
                    target.Name = target.OrgName;

                    Server.Print(ILevel.Moderator, Template.Text(Category.Notification,
                        18).Replace("+n", target.Name).Replace("+a", admin.Name), true);
                }
        }

        [CommandLevel("bansend", ILevel.Moderator)]
        public static void BanSend(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("bansend"))
                if (args == "on" && !commands.BanSend.Has(admin))
                {
                    commands.BanSend.Add(admin);
                    Server.Print(ILevel.Moderator, Template.Text(Category.Notification, 24).Replace("+n", admin.Name), true);
                }
                else if (args == "off" && commands.BanSend.Has(admin))
                {
                    commands.BanSend.Remove(admin);
                    Server.Print(ILevel.Moderator, Template.Text(Category.Notification, 25).Replace("+n", admin.Name), true);
                }
        }

        [CommandLevel("logsend", ILevel.Moderator)]
        public static void LogSend(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("logsend"))
                if (args == "on" && !commands.LogSend.Has(admin))
                {
                    commands.LogSend.Add(admin);
                    Server.Print(ILevel.Moderator, Template.Text(Category.Notification, 26).Replace("+n", admin.Name), true);
                }
                else if (args == "off" && commands.LogSend.Has(admin))
                {
                    commands.LogSend.Remove(admin);
                    Server.Print(ILevel.Moderator, Template.Text(Category.Notification, 27).Replace("+n", admin.Name), true);
                }
        }

        [CommandLevel("ipsend", ILevel.Moderator)]
        public static void IPSend(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("ipsend"))
                if (args == "on" && !commands.IPSend.Has(admin))
                {
                    commands.IPSend.Add(admin);
                    Server.Print(ILevel.Moderator, Template.Text(Category.Notification, 22).Replace("+n", admin.Name), true);
                }
                else if (args == "off" && commands.IPSend.Has(admin))
                {
                    commands.IPSend.Remove(admin);
                    Server.Print(ILevel.Moderator, Template.Text(Category.Notification, 23).Replace("+n", admin.Name), true);
                }
        }

        [CommandLevel("stats", ILevel.Moderator)]
        public static void Stats(IUser admin)
        {
            if (admin.Level >= Server.GetLevel("stats"))
            {
                admin.Print(Template.Text(Category.Stats, 0).Replace("+n", Server.Chatroom.Name));
                admin.Print(String.Empty);
                admin.Print(Template.Text(Category.Stats, 1).Replace("+n", Helpers.LanguageCodeToString(Server.Chatroom.Language)));
                admin.Print(Template.Text(Category.Stats, 2).Replace("+n", "arlnk://" + Server.Chatroom.Hashlink));
                admin.Print(Template.Text(Category.Stats, 3).Replace("+n", Helpers.GetUptime));
                admin.Print(Template.Text(Category.Stats, 4).Replace("+n", Server.Stats.DataReceived.ToString()));
                admin.Print(Template.Text(Category.Stats, 5).Replace("+n", Server.Stats.DataSent.ToString()));
                admin.Print(Template.Text(Category.Stats, 6).Replace("+n", Server.Stats.InvalidLoginAttempts.ToString()));
                admin.Print(Template.Text(Category.Stats, 7).Replace("+n", Server.Stats.FloodCount.ToString()));
                admin.Print(Template.Text(Category.Stats, 8).Replace("+n", Server.Stats.RejectionCount.ToString()));
                admin.Print(Template.Text(Category.Stats, 9).Replace("+n", Server.Stats.JoinCount.ToString()));
                admin.Print(Template.Text(Category.Stats, 10).Replace("+n", Server.Stats.PartCount.ToString()));
                admin.Print(Template.Text(Category.Stats, 11).Replace("+n", Server.Stats.CurrentUserCount.ToString()));
                int counter = 0;
                Server.Users.All(x =>
                {
                    if (!x.Link.IsLinked)
                        counter++;
                });
                counter = (int)(Server.Stats.CurrentUserCount - counter);
                admin.Print(Template.Text(Category.Stats, 12).Replace("+n", counter.ToString()));
                admin.Print(Template.Text(Category.Stats, 13).Replace("+n", Server.Stats.PeakUserCount.ToString()));
                admin.Print(Template.Text(Category.Stats, 14).Replace("+n", Server.Stats.PublicMessages.ToString()));
                admin.Print(Template.Text(Category.Stats, 15).Replace("+n", Server.Stats.PrivateMessages.ToString()));
                counter = 0;
                Server.Channels.ForEach(x => counter++);
                admin.Print(Template.Text(Category.Stats, 16).Replace("+n", counter.ToString()));
            }
        }

        [CommandLevel("whowas", ILevel.Moderator)]
        public static void Whowas(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("whowas"))
                commands.Whowas.Query(admin, args.Replace("\"", String.Empty));
        }

        public static void Shout(IUser client, String text)
        {
            if (client.Level > ILevel.Regular || Settings.General)
                if (!client.Muzzled)
                {
                    Server.Users.Ares(x =>
                    {
                        if (!x.IgnoreList.Contains(client.Name))
                            x.Print(Template.Text(Category.Messaging, 0).Replace("+n", client.Name).Replace("+t", text));
                    });

                    Server.Users.Web(x =>
                    {
                        if (!x.IgnoreList.Contains(client.Name))
                            x.Print(Template.Text(Category.Messaging, 0).Replace("+n", client.Name).Replace("+t", text));
                    });

                    if(Server.Link.IsLinked)
                    {
                        Server.Link.ForEachLeaf(l =>
                        {
                            l.ForEachUser(c =>
                            {
                                if (c.IgnoreList.Contains(client.Name))
                                    c.Print(Template.Text(Category.Messaging, 0).Replace("+n", client.Name).Replace("+t", text));
                            });
                        });
                    }
                }
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
                if (target != null && !target.IgnoreList.Contains(client.Name))
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
                Server.Print(Template.Text(Category.AdminList, 0).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : client.Name), true);
                Server.Print(String.Empty, true);

                Server.Users.All(x =>
                {
                    if (x.Level > ILevel.Regular)
                        if (x.Link.Visible || !x.Link.IsLinked)
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
                        Server.Print(Template.Text(Category.RoomSearch, 2).Replace("+n", args), true);
                    else
                    {
                        Server.Print(Template.Text(Category.RoomSearch, 3).Replace("+n", args), true);

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
                            Settings.Stealth ? Server.Chatroom.Name : client.Name).Replace("+i", b == 0 ? "unlimited" : (b + " minutes")), true);
                    }
            }
        }

        [CommandLevel("redirect", ILevel.Administrator)]
        public static void Redirect(IUser admin, IUser target, String args)
        {
            if (admin.Level >= Server.GetLevel("redirect"))
                if (target != null)
                    if (target.Level < admin.Level)
                    {
                        if (!target.Link.IsLinked && admin.Link.IsLinked)
                            if (target.Level != ILevel.Regular)
                                return;

                        IHashlinkRoom hr = Server.Hashlinks.Decrypt(args.Trim());

                        if (hr != null)
                        {
                            Server.Print(Template.Text(Category.AdminAction, 20).Replace("+n",
                                target.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name).Replace("+r", hr.Name), true);

                            target.Redirect(args.Trim());
                        }
                    }
                    else admin.Print(Template.Text(Category.Notification, 29).Replace("+n", target.Name));
        }

        [CommandLevel("sharefiles", ILevel.Host)]
        public static void ShareFiles(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("sharefiles"))
                if (args == "on" && !Settings.ShareFileMonitoring)
                {
                    Settings.ShareFileMonitoring = true;
                    Server.Print(Template.Text(Category.EnableDisable, 0).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
                else if (args == "off" && Settings.ShareFileMonitoring)
                {
                    Settings.ShareFileMonitoring = false;
                    Server.Print(Template.Text(Category.EnableDisable, 1).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
        }

        [CommandLevel("idle", ILevel.Host)]
        public static void IdleMonitoring(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("idle"))
                if (args == "on" && !Settings.IdleMonitoring)
                {
                    Settings.IdleMonitoring = true;
                    Server.Print(Template.Text(Category.EnableDisable, 2).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
                else if (args == "off" && Settings.IdleMonitoring)
                {
                    Settings.IdleMonitoring = false;
                    Server.Print(Template.Text(Category.EnableDisable, 3).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
        }

        [CommandLevel("clock", ILevel.Administrator)]
        public static void Clock(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("clock"))
                if (args == "on" && !Settings.Clock)
                {
                    Settings.Clock = true;
                    Topics.EnableClock();
                    Server.Print(Template.Text(Category.EnableDisable, 4).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
                else if (args == "off" && Settings.Clock)
                {
                    Settings.Clock = false;
                    Topics.DisableClock();
                    Server.Print(Template.Text(Category.EnableDisable, 5).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
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
                    Settings.Stealth ? Server.Chatroom.Name : admin.Name).Replace("+v", admin.Vroom.ToString()));
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
                    Settings.Stealth ? Server.Chatroom.Name : admin.Name).Replace("+v", admin.Vroom.ToString()));
            }
        }

        [CommandLevel("greetmsg", ILevel.Host)]
        public static void GreetMsg(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("greetmsg"))
                if (args == "on" && !Settings.GreetMsg)
                {
                    Settings.GreetMsg = true;
                    Server.Print(Template.Text(Category.EnableDisable, 6).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
                else if (args == "off" && Settings.GreetMsg)
                {
                    Settings.GreetMsg = false;
                    Server.Print(Template.Text(Category.EnableDisable, 7).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
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
                        Server.Print(Template.Text(Category.Greetings, 0).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
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
                            Server.Print(Template.Text(Category.Greetings, 1).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
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
                if (args == "on" && !Settings.PMGreetMsg)
                {
                    Settings.PMGreetMsg = true;
                    Server.Print(Template.Text(Category.EnableDisable, 8).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
                else if (args == "off" && Settings.PMGreetMsg)
                {
                    Settings.PMGreetMsg = false;
                    Server.Print(Template.Text(Category.EnableDisable, 9).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
                else if (!String.IsNullOrEmpty(args))
                {
                    Settings.PMGreetMsg = true;
                    Greets.SetPM(args);
                    Server.Print(Template.Text(Category.Greetings, 2).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
        }

        [CommandLevel("caps", ILevel.Administrator)]
        public static void Caps(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("caps"))
                if (args == "on" && !Settings.CapsMonitoring)
                {
                    Settings.CapsMonitoring = true;
                    Server.Print(Template.Text(Category.EnableDisable, 10).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
                else if (args == "off" && Settings.CapsMonitoring)
                {
                    Settings.CapsMonitoring = false;
                    Server.Print(Template.Text(Category.EnableDisable, 11).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
        }

        [CommandLevel("anon", ILevel.Administrator)]
        public static void Anon(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("anon"))
                if (args == "on" && !Settings.AnonMonitoring)
                {
                    Settings.AnonMonitoring = true;
                    Server.Print(Template.Text(Category.EnableDisable, 12).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
                else if (args == "off" && Settings.AnonMonitoring)
                {
                    Settings.AnonMonitoring = false;
                    Server.Print(Template.Text(Category.EnableDisable, 13).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
        }

        [CommandLevel("general", ILevel.Administrator)]
        public static void General(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("general"))
                if (args == "on" && !Settings.General)
                {
                    Settings.General = true;
                    Server.Print(Template.Text(Category.EnableDisable, 16).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
                else if (args == "off" && Settings.General)
                {
                    Settings.General = false;
                    Server.Print(Template.Text(Category.EnableDisable, 17).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
        }

        [CommandLevel("url", ILevel.Host)]
        public static void Url(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("url"))
                if (args == "on" && !Settings.Url)
                {
                    Settings.Url = true;
                    Server.Print(Template.Text(Category.EnableDisable, 18).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                    Urls.EnableDisable(true);
                }
                else if (args == "off" && Settings.Url)
                {
                    Settings.Url = false;
                    Server.Print(Template.Text(Category.EnableDisable, 19).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                    Urls.EnableDisable(false);
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
                    Server.Print(Template.Text(Category.Urls, 1).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
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
                        Server.Print(Template.Text(Category.Urls, 2).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
            }
        }

        [CommandLevel("listurls", ILevel.Administrator)]
        public static void ListUrls(IUser admin)
        {
            if (admin.Level >= Server.GetLevel("listurls"))
                Urls.List(admin);
        }

        [CommandLevel("roominfo", ILevel.Host)]
        public static void RoomInfo(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("roominfo"))
                if (args == "on" && !Settings.RoomInfo)
                {
                    Settings.RoomInfo = true;
                    Server.Print(Template.Text(Category.EnableDisable, 20).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                    commands.RoomInfo.ForceUpdate();
                }
                else if (args == "off" && Settings.RoomInfo)
                {
                    Settings.RoomInfo = false;
                    Server.Print(Template.Text(Category.EnableDisable, 21).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
        }

        [CommandLevel("status", ILevel.Host)]
        public static void Status(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("status"))
            {
                Settings.Status = args;
                Server.Print(Template.Text(Category.RoomInfo, 6).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));

                if (Settings.RoomInfo)
                    commands.RoomInfo.ForceUpdate();
            }
        }

        [CommandLevel("lastseen", ILevel.Host)]
        public static void LastSeen(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("lastseen"))
                if (args == "on" && !Settings.LastSeen)
                {
                    Settings.LastSeen = true;
                    Server.Print(Template.Text(Category.EnableDisable, 22).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
                else if (args == "off" && Settings.LastSeen)
                {
                    Settings.LastSeen = false;
                    Server.Print(Template.Text(Category.EnableDisable, 23).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
        }

        [CommandLevel("history", ILevel.Host)]
        public static void History(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("history"))
                if (args == "on" && !Settings.History)
                {
                    Settings.History = true;
                    Server.Print(Template.Text(Category.EnableDisable, 24).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
                else if (args == "off" && Settings.History)
                {
                    Settings.History = false;
                    Server.Print(Template.Text(Category.EnableDisable, 25).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
        }

        [CommandLevel("filter", ILevel.Administrator)]
        public static void AddJoinFilter(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("filter"))
                JoinFilter.Add(admin, args);
        }

        public static void RemJoinFilter(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("filter"))
            {
                int i;

                if (int.TryParse(args, out i))
                    JoinFilter.Remove(admin, i);
            }
        }

        public static void JoinFilters(IUser admin)
        {
            if (admin.Level >= Server.GetLevel("filter"))
                JoinFilter.View(admin);
        }

        public static void Filter(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("filter"))
                if (args == "on" && !Settings.Filtering)
                {
                    Settings.Filtering = true;
                    Server.Print(Template.Text(Category.EnableDisable, 30).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
                else if (args == "off" && Settings.Filtering)
                {
                    Settings.Filtering = false;
                    Server.Print(Template.Text(Category.EnableDisable, 31).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
        }

        public static void AddFileFilter(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("filter"))
                FileFilter.Add(admin, args);
        }

        public static void RemFileFilter(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("filter"))
            {
                int i;

                if (int.TryParse(args, out i))
                    FileFilter.Remove(admin, i);
            }
        }

        public static void FileFilters(IUser admin)
        {
            if (admin.Level >= Server.GetLevel("filter"))
                FileFilter.View(admin);
        }

        public static void AddWordFilter(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("filter"))
                WordFilter.Add(admin, args);
        }

        public static void RemWordFilter(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("filter"))
            {
                int i;

                if (int.TryParse(args, out i))
                    WordFilter.Remove(admin, i);
            }
        }

        public static void WordFilters(IUser admin)
        {
            if (admin.Level >= Server.GetLevel("filter"))
                WordFilter.View(admin);
        }

        [CommandLevel("adminannounce", ILevel.Host)]
        public static void AdminAnnounce(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("adminannounce"))
                if (args == "on" && !Settings.AdminAnnounce)
                {
                    Settings.AdminAnnounce = true;
                    Server.Print(Template.Text(Category.Filter, 18).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
                else if (args == "off" && Settings.AdminAnnounce)
                {
                    Settings.AdminAnnounce = false;
                    Server.Print(Template.Text(Category.Filter, 19).Replace("+n", Settings.Stealth ? Server.Chatroom.Name : admin.Name));
                }
        }

        public static void AddLine(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("filter"))
            {
                String[] split = args.Split(new String[] { ", " }, StringSplitOptions.None);

                if (split.Length > 1)
                {
                    int i;

                    if (int.TryParse(split[0], out i))
                    {
                        String str = String.Join(", ", split.Skip(1).ToArray());
                        WordFilter.AddLine(admin, i, str);
                    }
                }
            }
        }

        public static void RemLine(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("filter"))
            {
                String[] split = args.Split(new String[] { ", " }, StringSplitOptions.None);

                if (split.Length == 2)
                {
                    int i1, i2;

                    if (int.TryParse(split[0], out i1))
                        if (int.TryParse(split[1], out i2))
                            WordFilter.RemLine(admin, i1, i2);
                }
            }
        }

        public static void ViewFilter(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("filter"))
            {
                int i;

                if (int.TryParse(args, out i))
                    WordFilter.ViewFilter(admin, i);
            }
        }

        [CommandLevel("loadtemplate", ILevel.Host)]
        public static void LoadTemplate(IUser admin)
        {
            if (admin.Level >= Server.GetLevel("loadtemplate"))
                Template.Load(true);
        }

        public static void ListPasswords(IUser admin)
        {
            IPassword[] passwords = Server.Accounts.GetPasswords();

            for (int i = 0; i < passwords.Length; i++)
                admin.Print(i + " - " + passwords[i].Name + " [" + passwords[i].Level + "]");
        }

        public static void RemovePassword(IUser admin, String args)
        {
            int index;

            if (int.TryParse(args, out index))
            {
                IPassword[] passwords = Server.Accounts.GetPasswords();

                if (index >= 0 && index < passwords.Length)
                {
                    admin.Print(Template.Text(Category.AdminLogin, 7).Replace("+n", passwords[index].Name));
                    Server.Accounts.Remove(passwords[index]);
                }
            }
        }

        [CommandLevel("listquarantined", ILevel.Host)]
        public static void ListQuarantined(IUser admin)
        {
            if (admin.Level >= Server.GetLevel("listquarantined"))
            {
                bool empty = true;
                int counter = 0;

                Server.Users.Quarantined(x =>
                {
                    empty = false;
                    admin.Print((counter++) + " - " + x.Name);
                });

                if (empty)
                    admin.Print(Template.Text(Category.Quarantined, 1));
            }
        }

        [CommandLevel("unquarantine", ILevel.Host)]
        public static void Unquarantine(IUser admin, String args)
        {
            if (admin.Level >= Server.GetLevel("unquarantine"))
            {
                int target;

                if (int.TryParse(args, out target))
                {
                    int counter = 0;

                    Server.Users.Quarantined(x =>
                    {
                        if (counter++ == target)
                        {
                            Server.Print(Template.Text(Category.Quarantined, 0).Replace("+n",
                                x.Name).Replace("+a", Settings.Stealth ? Server.Chatroom.Name : admin.Name), true);

                            x.Release();
                        }
                    });
                }
            }
        }


    }
}
