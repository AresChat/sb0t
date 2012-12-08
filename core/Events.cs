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
        private static scripting.ServerEvents js { get; set; }

        public static void InitializeCommandsExtension()
        {
            js = new scripting.ServerEvents(new ExHost(String.Empty));

            if (commands == null)
                commands = new commands.ServerEvents(new ExHost(String.Empty));
        }

        public static ICommandDefault[] DefaultCommandLevels
        {
            get { return commands.DefaultCommandLevels; }
        }

        public static void ServerStarted()
        {
            DefaultCommands = Settings.Get<bool>("commands");

            if (DefaultCommands)
                commands.ServerStarted();

            js.ServerStarted();

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.ServerStarted(); }
                catch { }
            });
        }

        public static void CycleTick()
        {
            if (DefaultCommands)
                commands.CycleTick();

            js.CycleTick();

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.CycleTick(); }
                catch { }
            });
        }

        public static void UnhandledProtocol(IClient client, bool custom, TCPMsg msg, TCPPacketReader packet, ulong tick)
        {
            if (DefaultCommands)
                commands.UnhandledProtocol(client != null ? client.IUser : null, custom, (byte)msg, packet.ToArray());

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.UnhandledProtocol(client != null ? client.IUser : null, custom, (byte)msg, packet.ToArray()); }
                catch { }
            });
        }

        public static bool Joining(IClient client)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.Joining(client != null ? client.IUser : null);

            if (result)
                result = js.Joining(client != null ? client.IUser : null);

            if (result)
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.Joining(client != null ? client.IUser : null);

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
                commands.Joined(client != null ? client.IUser : null);

            js.Joined(client != null ? client.IUser : null);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.Joined(client != null ? client.IUser : null); }
                catch { }
            });

            ChatLog.WriteLine("join: " + client.Name + " has joined");
        }

        public static void Rejected(IClient client, RejectedMsg msg)
        {
            Stats.RejectionCount++;

            if (DefaultCommands)
                commands.Rejected(client != null ? client.IUser : null, msg);

            js.Rejected(client != null ? client.IUser : null, msg);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.Rejected(client != null ? client.IUser : null, msg); }
                catch { }
            });

            ChatLog.WriteLine("rejected: " + client.Name + " was rejected");
        }

        public static void Parting(IClient client)
        {
            if (DefaultCommands)
                commands.Parting(client != null ? client.IUser : null);

            js.Parting(client != null ? client.IUser : null);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.Parting(client != null ? client.IUser : null); }
                catch { }
            });
        }

        public static void Parted(IClient client)
        {
            Stats.PartCount++;

            if (DefaultCommands)
                commands.Parted(client != null ? client.IUser : null);

            js.Parted(client != null ? client.IUser : null);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.Parted(client != null ? client.IUser : null); }
                catch { }
            });

            ChatLog.WriteLine("part: " + client.Name + " has parted");
        }

        public static bool AvatarReceived(IClient client)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.AvatarReceived(client != null ? client.IUser : null);

            if (result)
                result = js.AvatarReceived(client != null ? client.IUser : null);

            if (result)
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.AvatarReceived(client != null ? client.IUser : null);

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
                result = commands.PersonalMessageReceived(client != null ? client.IUser : null, text);

            if (result)
                result = js.PersonalMessageReceived(client != null ? client.IUser : null, text);

            if (result)
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.PersonalMessageReceived(client != null ? client.IUser : null, text);

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
                commands.TextReceived(client != null ? client.IUser : null, text);

            js.TextReceived(client != null ? client.IUser : null, text);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.TextReceived(client != null ? client.IUser : null, text); }
                catch { }
            });
        }

        public static String TextSending(IClient client, String text)
        {
            String result = text;

            if (DefaultCommands)
                result = commands.TextSending(client != null ? client.IUser : null, result);

            if (!String.IsNullOrEmpty(result))
                result = js.TextSending(client != null ? client.IUser : null, result);

            if (!String.IsNullOrEmpty(result))
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.TextSending(client != null ? client.IUser : null, result);

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
                commands.TextSent(client != null ? client.IUser : null, text);

            js.TextSent(client != null ? client.IUser : null, text);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.TextSent(client != null ? client.IUser : null, text); }
                catch { }
            });

            ChatLog.WriteLine("text: " + client.Name + "> " + text);
        }

        public static void EmoteReceived(IClient client, String text)
        {
            if (DefaultCommands)
                commands.EmoteReceived(client != null ? client.IUser : null, text);

            js.EmoteReceived(client != null ? client.IUser : null, text);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.EmoteReceived(client != null ? client.IUser : null, text); }
                catch { }
            });
        }

        public static String EmoteSending(IClient client, String text)
        {
            String result = text;

            if (DefaultCommands)
                result = commands.EmoteSending(client != null ? client.IUser : null, result);

            if (!String.IsNullOrEmpty(result))
                result = js.EmoteSending(client != null ? client.IUser : null, result);

            if (!String.IsNullOrEmpty(result))
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.EmoteSending(client != null ? client.IUser : null, result);

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
                commands.EmoteSent(client != null ? client.IUser : null, text);

            js.EmoteSent(client != null ? client.IUser : null, text);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.EmoteSent(client != null ? client.IUser : null, text); }
                catch { }
            });

            ChatLog.WriteLine("emote: * " + client.Name + " " + text);
        }

        public static void PrivateSending(IClient client, IClient target, PMEventArgs e)
        {
            PrivateMsg pm = new PrivateMsg(e.Text);

            if (DefaultCommands)
                commands.PrivateSending(client != null ? client.IUser : null, target != null ? target.IUser : null, pm);

            if (!String.IsNullOrEmpty(pm.Text))
                js.PrivateSending(client != null ? client.IUser : null, target != null ? target.IUser : null, pm);

            if (!String.IsNullOrEmpty(pm.Text))
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        x.Plugin.PrivateSending(client != null ? client.IUser : null, target != null ? target.IUser : null, pm);

                        if (String.IsNullOrEmpty(pm.Text) || pm.Cancel)
                            return;
                    }
                    catch { }
                });

            String result = pm.Text;

            if (String.IsNullOrEmpty(result))
                e.Cancel = true;
            else if (!client.Connected || !target.Connected)
                e.Cancel = true;
            else if (pm.Cancel)
                e.Cancel = true;
            else
                e.Text = result;
        }

        public static void PrivateSent(IClient client, IClient target, String text)
        {
            Stats.PrivateMessages++;

            if (DefaultCommands)
                commands.PrivateSent(client != null ? client.IUser : null, target != null ? target.IUser : null);

            js.PrivateSent(client != null ? client.IUser : null, target != null ? target.IUser : null);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.PrivateSent(client != null ? client.IUser : null, target != null ? target.IUser : null); }
                catch { }
            });
        }

        public static void BotPrivateSent(IClient client, String text)
        {
            Stats.PrivateMessages++;

            if (DefaultCommands)
                commands.BotPrivateSent(client != null ? client.IUser : null, text);

            js.BotPrivateSent(client != null ? client.IUser : null, text);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.BotPrivateSent(client != null ? client.IUser : null, text); }
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

                if (command == "logout")
                {
                    AccountManager.Logout(client);
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

                if (command == "idle" || command == "idles")
                {
                    IdleManager.Add(client);
                    Events.Idled(client);
                    return;
                }
            }

            if (target != null)
                if (target.IUser.Link.IsLinked)
                {
                    if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready && client.Level > ILevel.Regular)
                        ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafAdmin(ServerCore.Linker, client, command, target, args));
                    return;
                }

            if (!client.Registered)
                if (!Settings.Get<bool>("allow_unreg"))
                    return;

            if (DefaultCommands)
                commands.Command(client != null ? client.IUser : null, command, target != null ? target.IUser : null, args);

            js.Command(client != null ? client.IUser : null, command, target != null ? target.IUser : null, args);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.Command(client != null ? client.IUser : null, command, target != null ? target.IUser : null, args); }
                catch { }
            });
        }

        private static bool Nick(IClient client, String name)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.Nick(client != null ? client.IUser : null, name);

            if (result)
                result = js.Nick(client != null ? client.IUser : null, name);

            if (result)
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.Nick(client != null ? client.IUser : null, name);

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

                client.Print("/logout");
                client.Print("/nick <name>");

                if (client.Owner)
                    client.Print("/setlevel <user> <level>");
            }

            if (!client.Registered)
                if (!Settings.Get<bool>("allow_unreg"))
                    return;

            if (DefaultCommands)
                commands.Help(client != null ? client.IUser : null);

            js.Help(client != null ? client.IUser : null);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.Help(client != null ? client.IUser : null); }
                catch { }
            });
        }

        public static void FileReceived(IClient client, SharedFile file)
        {
            if (DefaultCommands)
                commands.FileReceived(client != null ? client.IUser : null, file.FileName, file.Title, file.Mime);

            js.FileReceived(client != null ? client.IUser : null, file.FileName, file.Title, file.Mime);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.FileReceived(client != null ? client.IUser : null, file.FileName, file.Title, file.Mime); }
                catch { }
            });
        }

        public static bool Ignoring(IClient client, IClient target)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.Ignoring(client != null ? client.IUser : null, target != null ? target.IUser : null);

            if (result)
                result = js.Ignoring(client != null ? client.IUser : null, target != null ? target.IUser : null);

            if (result)
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.Ignoring(client != null ? client.IUser : null, target != null ? target.IUser : null);

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
                commands.IgnoredStateChanged(client != null ? client.IUser : null, target != null ? target.IUser : null, ignored);

            js.IgnoredStateChanged(client != null ? client.IUser : null, target != null ? target.IUser : null, ignored);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.IgnoredStateChanged(client != null ? client.IUser : null, target != null ? target.IUser : null, ignored); }
                catch { }
            });
        }

        public static void InvalidLoginAttempt(IClient client)
        {
            Stats.InvalidLoginAttempts++;

            if (DefaultCommands)
                commands.InvalidLoginAttempt(client != null ? client.IUser : null);

            js.InvalidLoginAttempt(client != null ? client.IUser : null);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.InvalidLoginAttempt(client != null ? client.IUser : null); }
                catch { }
            });
        }

        public static void LoginGranted(IClient client)
        {
            if (DefaultCommands)
                commands.LoginGranted(client != null ? client.IUser : null);

            js.LoginGranted(client != null ? client.IUser : null);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.LoginGranted(client != null ? client.IUser : null); }
                catch { }
            });
        }

        public static void AdminLevelChanged(IClient client)
        {
            if (DefaultCommands)
                commands.AdminLevelChanged(client != null ? client.IUser : null);

            js.AdminLevelChanged(client != null ? client.IUser : null);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.AdminLevelChanged(client != null ? client.IUser : null); }
                catch { }
            });

            ChatLog.WriteLine("level: " + client.Name + " level changed to " + client.Level);
        }

        public static void InvalidRegistration(IClient client)
        {
            if (DefaultCommands)
                commands.InvalidRegistration(client != null ? client.IUser : null);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try
                {
                    if (client != null)
                        x.Plugin.InvalidRegistration(client.IUser);
                }
                catch { }
            });
        }

        public static bool Registering(IClient client)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.Registering(client != null ? client.IUser : null);

            if (result)
                result = js.Registering(client != null ? client.IUser : null);

            if (result)
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.Registering(client != null ? client.IUser : null);

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
                commands.Registered(client != null ? client.IUser : null);

            js.Registered(client != null ? client.IUser : null);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.Registered(client != null ? client.IUser : null); }
                catch { }
            });
        }

        public static void Unregistered(IClient client)
        {
            if (DefaultCommands)
                commands.Unregistered(client != null ? client.IUser : null);

            js.Unregistered(client != null ? client.IUser : null);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.Unregistered(client != null ? client.IUser : null); }
                catch { }
            });
        }

        public static void CaptchaSending(IClient client)
        {
            if (DefaultCommands)
                commands.CaptchaSending(client != null ? client.IUser : null);

            js.CaptchaSending(client != null ? client.IUser : null);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.CaptchaSending(client != null ? client.IUser : null); }
                catch { }
            });
        }

        public static void CaptchaReply(IClient client, String reply)
        {
            if (DefaultCommands)
                commands.CaptchaReply(client != null ? client.IUser : null, reply);

            js.CaptchaReply(client != null ? client.IUser : null, reply);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.CaptchaReply(client != null ? client.IUser : null, reply); }
                catch { }
            });
        }

        public static bool VroomChanging(IClient client, ushort vroom)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.VroomChanging(client != null ? client.IUser : null, vroom);

            if (result)
                result = js.VroomChanging(client != null ? client.IUser : null, vroom);

            if (result)
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.VroomChanging(client != null ? client.IUser : null, vroom);

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
                commands.VroomChanged(client != null ? client.IUser : null);

            js.VroomChanged(client != null ? client.IUser : null);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.VroomChanged(client != null ? client.IUser : null); }
                catch { }
            });
        }

        public static bool Flooding(IClient client, byte msg)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.Flooding(client != null ? client.IUser : null, msg);

            if (result)
                result = js.Flooding(client != null ? client.IUser : null, msg);

            if (result)
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.Flooding(client != null ? client.IUser : null, msg);

                        if (!result)
                            return;
                    }
                    catch { }
                });

            return result;
        }

        public static void Flooded(IClient client)
        {
            Stats.FloodCount++;

            if (DefaultCommands)
                commands.Flooded(client != null ? client.IUser : null);

            js.Flooded(client != null ? client.IUser : null);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.Flooded(client != null ? client.IUser : null); }
                catch { }
            });

            ChatLog.WriteLine("flood: " + client.Name + " flooded out");
        }

        public static void Logout(IClient client)
        {
            if (DefaultCommands)
                commands.Logout(client != null ? client.IUser : null);

            js.Logout(client != null ? client.IUser : null);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.Logout(client != null ? client.IUser : null); }
                catch { }
            });
        }

        public static void Idled(IClient client)
        {
            if (DefaultCommands)
                commands.Idled(client != null ? client.IUser : null);

            js.Idled(client != null ? client.IUser : null);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.Idled(client != null ? client.IUser : null); }
                catch { }
            });
        }

        public static void Unidled(IClient client, uint seconds_away)
        {
            if (DefaultCommands)
                commands.Unidled(client != null ? client.IUser : null, seconds_away);

            js.Unidled(client != null ? client.IUser : null, seconds_away);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.Unidled(client != null ? client.IUser : null, seconds_away); }
                catch { }
            });
        }

        public static void BansAutoCleared()
        {
            if (DefaultCommands)
                commands.BansAutoCleared();

            js.BansAutoCleared();

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.BansAutoCleared(); }
                catch { }
            });
        }

        public static bool ProxyDetected(IClient client)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.ProxyDetected(client != null ? client.IUser : null);

            js.ProxyDetected(client != null ? client.IUser : null);

            if (result)
                ExtensionManager.Plugins.ForEach(x =>
                {
                    try
                    {
                        result = x.Plugin.ProxyDetected(client != null ? client.IUser : null);

                        if (!result)
                            return;
                    }
                    catch { }
                });

            return result;
        }

        public static void LinkError(core.LinkLeaf.LinkError e)
        {
            if (DefaultCommands)
                commands.LinkError((ILinkError)e);

            js.LinkError((ILinkError)e);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.LinkError((ILinkError)e); }
                catch { }
            });
        }

        public static void Linked()
        {
            if (DefaultCommands)
                commands.Linked();

            js.Linked();

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.Linked(); }
                catch { }
            });
        }

        public static void Unlinked()
        {
            if (DefaultCommands)
                commands.Unlinked();

            js.Unlinked();

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.Unlinked(); }
                catch { }
            });
        }

        public static void LeafJoined(LinkLeaf.Leaf leaf)
        {
            if (DefaultCommands)
                commands.LeafJoined(leaf);

            js.LeafJoined(leaf);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.LeafJoined(leaf); }
                catch { }
            });
        }

        public static void LeafParted(LinkLeaf.Leaf leaf)
        {
            if (DefaultCommands)
                commands.LeafParted(leaf);

            js.LeafParted(leaf);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.LeafParted(leaf); }
                catch { }
            });
        }

        public static void LinkedAdminDisabled(LinkLeaf.Leaf leaf, IClient client)
        {
            if (DefaultCommands)
                commands.LinkedAdminDisabled(leaf, client.IUser);

            js.LinkedAdminDisabled(leaf, client.IUser);

            ExtensionManager.Plugins.ForEach(x =>
            {
                try { x.Plugin.LinkedAdminDisabled(leaf, client != null ? client.IUser : null); }
                catch { }
            });
        }
    }
}
