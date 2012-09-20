using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;
using core.Extensions;

namespace core
{
    class Events
    {
        private static commands.ServerEvents commands { get; set; }
        private static bool DefaultCommands { get; set; }

        public static void ServerStarted()
        {
            DefaultCommands = Settings.Get<bool>("commands");

            if (commands == null)
                commands = new commands.ServerEvents(new ExHost(String.Empty));

            if (DefaultCommands)
                commands.ServerStarted();

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.ServerStarted();
                }
                catch { }
            });
        }

        public static void CycleTick()
        {
            if (DefaultCommands)
                commands.CycleTick();

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.CycleTick();
                }
                catch { }
            });
        }

        public static void UnhandledProtocol(IClient client, bool custom, TCPMsg msg, TCPPacketReader packet, ulong tick)
        {
            UserPool.AUsers.ForEachWhere(x =>
                x.SendPacket(TCPOutbound.NoSuch(x, "Unhandled : " + client.Name + " : " + msg)), x => x.LoggedIn);

            if (DefaultCommands)
                commands.UnhandledProtocol(client.IUser, custom, (byte)msg, packet.ToArray());

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.UnhandledProtocol(client.IUser, custom, (byte)msg, packet.ToArray());
                }
                catch { }
            });
        }

        public static bool Joining(IClient client)
        {
            bool result = true;
            
            if (DefaultCommands)
                result = commands.Joining(client.IUser);

            if (result)
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.Joining(client.IUser);

                        if (!result)
                            return;
                    }
                    catch { }
                });

            return result;
        }

        public static void Joined(IClient client)
        {
            Stats.JoinCount++;

            if (Stats.CurrentUserCount > Stats.PeakUserCount)
                Stats.PeakUserCount = Stats.CurrentUserCount;

            if (DefaultCommands)
                commands.Joined(client.IUser);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.Joined(client.IUser);
                }
                catch { }
            });
        }

        public static void Rejected(IClient client, RejectedMsg msg)
        {
            Stats.RejectionCount++;

            if (DefaultCommands)
                commands.Rejected(client.IUser, msg);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.Rejected(client.IUser, msg);
                }
                catch { }
            });
        }

        public static void Parting(IClient client)
        {
            if (DefaultCommands)
                commands.Parting(client.IUser);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.Parting(client.IUser);
                }
                catch { }
            });
        }

        public static void Parted(IClient client)
        {
            Stats.PartCount++;

            if (DefaultCommands)
                commands.Parted(client.IUser);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.Parted(client.IUser);
                }
                catch { }
            });
        }

        public static bool AvatarReceived(IClient client)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.AvatarReceived(client.IUser);

            if (result)
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.AvatarReceived(client.IUser);

                        if (!result)
                            return;
                    }
                    catch { }
                });

            return result;
        }

        public static bool PersonalMessageReceived(IClient client, String text)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.PersonalMessageReceived(client.IUser, text);

            if (result)
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.PersonalMessageReceived(client.IUser, text);

                        if (!result)
                            return;
                    }
                    catch { }
                });

            return result;
        }

        public static void TextReceived(IClient client, String text)
        {
            if (DefaultCommands)
                commands.TextReceived(client.IUser, text);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.TextReceived(client.IUser, text);
                }
                catch { }
            });
        }

        public static String TextSending(IClient client, String text)
        {
            String result = text;

            if (DefaultCommands)
                result = commands.TextSending(client.IUser, result);

            if (!String.IsNullOrEmpty(result))
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.TextSending(client.IUser, result);

                        if (String.IsNullOrEmpty(result))
                            return;
                    }
                    catch { }
                });

            return result;
        }

        public static void TextSent(IClient client, String text)
        {
            Stats.PublicMessages++;

            if (DefaultCommands)
                commands.TextSent(client.IUser, text);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.TextSent(client.IUser, text);
                }
                catch { }
            });
        }

        public static void EmoteReceived(IClient client, String text)
        {
            if (DefaultCommands)
                commands.EmoteReceived(client.IUser, text);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.EmoteReceived(client.IUser, text);
                }
                catch { }
            });
        }

        public static String EmoteSending(IClient client, String text)
        {
            String result = text;
            
            if (DefaultCommands)
                result = commands.EmoteSending(client.IUser, result);

            if (!String.IsNullOrEmpty(result))
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.EmoteSending(client.IUser, result);

                        if (String.IsNullOrEmpty(result))
                            return;
                    }
                    catch { }
                });

            return result;
        }

        public static void EmoteSent(IClient client, String text)
        {
            Stats.PublicMessages++;

            if (DefaultCommands)
                commands.EmoteSent(client.IUser, text);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.EmoteSent(client.IUser, text);
                }
                catch { }
            });
        }

        public static void PrivateSending(IClient client, IClient target, PMEventArgs e)
        {
            PrivateMsg pm = new PrivateMsg(e.Text);

            if (DefaultCommands)
                commands.PrivateSending(client.IUser, target.IUser, pm);

            if (!String.IsNullOrEmpty(pm.Text))
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        x.Plugin.PrivateSending(client.IUser, target.IUser, pm);

                        if (String.IsNullOrEmpty(pm.Text))
                            return;
                    }
                    catch { }
                });

            String result = pm.Text;

            if (String.IsNullOrEmpty(result))
                e.Cancel = true;
            else if (!client.Connected || !target.Connected)
                e.Cancel = true;
            else
                e.Text = result;
        }

        public static void PrivateSent(IClient client, IClient target, String text)
        {
            Stats.PrivateMessages++;

            if (DefaultCommands)
                commands.PrivateSent(client.IUser, target.IUser);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.PrivateSent(client.IUser, target.IUser);
                }
                catch { }
            });
        }

        public static void BotPrivateSent(IClient client, String text)
        {
            Stats.PrivateMessages++;

            if (DefaultCommands)
                commands.BotPrivateSent(client.IUser, text);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.BotPrivateSent(client.IUser, text);
                }
                catch { }
            });
        }

        public static void Command(IClient client, String command, IClient target, String args)
        {
            if (command == "help")
            {
                Help(client);
                return;
            }

            if (!client.Captcha)
                if (!command.StartsWith("login "))
                    return;

            if (!client.Registered)
            {
                if (command.StartsWith("register "))
                {
                    AccountManager.Register(client, command.Substring(9));
                    return;
                }
                else if (command.StartsWith("login "))
                {
                    AccountManager.Login(client, command.Substring(6));
                    return;
                }
            }
            else
            {
                if (command == "unregister" && !client.Owner)
                {
                    AccountManager.Unregister(client);
                    return;
                }

                if (command.StartsWith("nick "))
                {
                    if (!Helpers.NameAvailable(client, command.Substring(5)) || command.Substring(5).Length < 2)
                        return;

                    if (Nick(client, command.Substring(5)))
                        client.Name = command.Substring(5);

                    return;
                }

                if (command.StartsWith("setlevel "))
                {
                    if (target != null && client.Owner)
                        if (target.Registered)
                        {
                            byte level;

                            if (byte.TryParse(args, out level))
                                if (level <= 3)
                                {
                                    target.Level = (ILevel)level;
                                    AccountManager.UpdateAccount(target);
                                }
                        }

                    return;
                }
            }

            if (DefaultCommands)
                commands.Command(client.IUser, command, target.IUser, args);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.Command(client.IUser, command, target.IUser, args);
                }
                catch { }
            });
        }

        private static bool Nick(IClient client, String name)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.Nick(client.IUser, name);

            if (result)
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.Nick(client.IUser, name);

                        if (!result)
                            return;
                    }
                    catch { }
                });

            return result;
        }

        private static void Help(IClient client)
        {
            if (!client.Registered)
            {
                if (client.Captcha)
                    client.Print("/register <password>");

                client.Print("/login <password>");
            }
            else
            {
                if (!client.Owner)
                    client.Print("/unregister");

                client.Print("/nick <name>");

                if (client.Owner)
                    client.Print("/setlevel <user> <level>");
            }

            if (DefaultCommands)
                commands.Help(client.IUser);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.Help(client.IUser);
                }
                catch { }
            });
        }

        public static void FileReceived(IClient client, SharedFile file)
        {
            if (DefaultCommands)
                commands.FileReceived(client.IUser, file.FileName, file.Title, file.Mime);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.FileReceived(client.IUser, file.FileName, file.Title, file.Mime);
                }
                catch { }
            });
        }

        public static bool Ignoring(IClient client, IClient target)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.Ignoring(client.IUser, target.IUser);

            if (result)
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.Ignoring(client.IUser, target.IUser);

                        if (!result)
                            return;
                    }
                    catch { }
                });

            return result;
        }

        public static void IgnoredStateChanged(IClient client, IClient target, bool ignored)
        {
            if (DefaultCommands)
                commands.IgnoredStateChanged(client.IUser, target.IUser, ignored);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.IgnoredStateChanged(client.IUser, target.IUser, ignored);
                }
                catch { }
            });
        }

        public static void InvalidLoginAttempt(IClient client)
        {
            Stats.InvalidLoginAttempts++;

            if (DefaultCommands)
                commands.InvalidLoginAttempt(client.IUser);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.InvalidLoginAttempt(client.IUser);
                }
                catch { }
            });
        }

        public static void LoginGranted(IClient client)
        {
            if (DefaultCommands)
                commands.LoginGranted(client.IUser);

            ExtensionManager.Plugins.ForEach(x =>
            {
                x.Plugin.LoginGranted(client.IUser);
            });
        }

        public static void AdminLevelChanged(IClient client)
        {
            if (DefaultCommands)
                commands.AdminLevelChanged(client.IUser);

            ExtensionManager.Plugins.ForEach(x =>
            {
                x.Plugin.AdminLevelChanged(client.IUser);
            });
        }

        public static bool Registering(IClient client)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.Registering(client.IUser);

            if (result)
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.Registering(client.IUser);

                        if (!result)
                            return;
                    }
                    catch { }
                });

            return result;
        }

        public static void Registered(IClient client)
        {
            if (DefaultCommands)
                commands.Registered(client.IUser);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.Registered(client.IUser);
                }
                catch { }
            });
        }

        public static void Unregistered(IClient client)
        {
            if (DefaultCommands)
                commands.Unregistered(client.IUser);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.Unregistered(client.IUser);
                }
                catch { }
            });
        }

        public static void CaptchaSending(IClient client)
        {
            if (DefaultCommands)
                commands.CaptchaSending(client.IUser);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.CaptchaSending(client.IUser);
                }
                catch { }
            });
        }

        public static void CaptchaReply(IClient client, String reply)
        {
            if (DefaultCommands)
                commands.CaptchaReply(client.IUser, reply);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.CaptchaReply(client.IUser, reply);
                }
                catch { }
            });
        }

        public static bool VroomChanging(IClient client, ushort vroom)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.VroomChanging(client.IUser, vroom);

            if (result)
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.VroomChanging(client.IUser, vroom);

                        if (!result)
                            return;
                    }
                    catch { }
                });

            return result;
        }

        public static void VroomChanged(IClient client)
        {
            if (DefaultCommands)
                commands.VroomChanged(client.IUser);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    x.Plugin.VroomChanged(client.IUser);
                }
                catch { }
            });
        }
    }
}
