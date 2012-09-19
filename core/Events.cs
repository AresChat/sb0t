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
                commands = new commands.ServerEvents(new ExHost());

            if (DefaultCommands)
                commands.ServerStarted();
        }

        public static void CycleTick()
        {
            if (DefaultCommands)
                commands.CycleTick();
        }

        public static void UnhandledProtocol(IClient client, bool custom, TCPMsg msg, TCPPacketReader packet, ulong tick)
        {
            UserPool.AUsers.ForEachWhere(x =>
                x.SendPacket(TCPOutbound.NoSuch(x, "Unhandled : " + client.Name + " : " + msg)), x => x.LoggedIn);

            if (DefaultCommands)
                commands.UnhandledProtocol(client.IUser, custom, (byte)msg, packet.ToArray());
        }

        public static bool Joining(IClient client)
        {
            bool result = true;
            
            if (DefaultCommands)
                result = commands.Joining(client.IUser);

            return result;
        }

        public static void Joined(IClient client)
        {
            if (DefaultCommands)
                commands.Joined(client.IUser);

            Stats.JoinCount++;

            if (Stats.CurrentUserCount > Stats.PeakUserCount)
                Stats.PeakUserCount = Stats.CurrentUserCount;
        }

        public static void Rejected(IClient client, RejectedMsg msg)
        {
            if (DefaultCommands)
                commands.Rejected(client.IUser, msg);

            Stats.RejectionCount++;
        }

        public static void Parting(IClient client)
        {
            if (DefaultCommands)
                commands.Parting(client.IUser);
        }

        public static void Parted(IClient client)
        {
            if (DefaultCommands)
                commands.Parted(client.IUser);

            Stats.PartCount++;
        }

        public static bool AvatarReceived(IClient client)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.AvatarReceived(client.IUser);

            return result;
        }

        public static bool PersonalMessageReceived(IClient client, String text)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.PersonalMessageReceived(client.IUser, text);

            return result;
        }

        public static void TextReceived(IClient client, String text)
        {
            if (DefaultCommands)
                commands.TextReceived(client.IUser, text);
        }

        public static String TextSending(IClient client, String text)
        {
            String result = text;

            if (DefaultCommands)
                result = commands.TextSending(client.IUser, result);

            return result;
        }

        public static void TextSent(IClient client, String text)
        {
            if (DefaultCommands)
                commands.TextSent(client.IUser, text);

            Stats.PublicMessages++;
        }

        public static void EmoteReceived(IClient client, String text)
        {
            if (DefaultCommands)
                commands.EmoteReceived(client.IUser, text);
        }

        public static String EmoteSending(IClient client, String text)
        {
            String result = text;
            
            if (DefaultCommands)
                result = commands.EmoteSending(client.IUser, result);

            return result;
        }

        public static void EmoteSent(IClient client, String text)
        {
            if (DefaultCommands)
                commands.EmoteSent(client.IUser, text);

            Stats.PublicMessages++;
        }

        public static void PrivateSending(IClient client, IClient target, PMEventArgs e)
        {
            PrivateMsg pm = new PrivateMsg(e.Text);

            if (DefaultCommands)
                commands.PrivateSending(client.IUser, target.IUser, pm);

            String result = pm.Text;

            if (String.IsNullOrEmpty(result))
                e.Cancel = true;
            else if (!client.Connected || !target.Connected)
                e.Cancel = true;
            else
            {
                e.Text = result;
                Stats.PrivateMessages++;
            }
        }

        public static void PrivateSent(IClient client, IClient target, String text)
        {
            if (DefaultCommands)
                commands.PrivateSent(client.IUser, target.IUser);
        }

        public static void BotPrivateSent(IClient client, String text)
        {
            if (DefaultCommands)
                commands.BotPrivateSent(client.IUser, text);
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
        }

        private static bool Nick(IClient client, String name)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.Nick(client.IUser, name);

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
        }

        public static void FileReceived(IClient client, SharedFile file)
        {
            if (DefaultCommands)
                commands.FileReceived(client.IUser, file.FileName, file.Title, file.Mime);
        }

        public static bool Ignoring(IClient client, IClient target)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.Ignoring(client.IUser, target.IUser);

            return result;
        }

        public static void IgnoredStateChanged(IClient client, IClient target, bool ignored)
        {
            if (DefaultCommands)
                commands.IgnoredStateChanged(client.IUser, target.IUser, ignored);
        }

        public static void InvalidLoginAttempt(IClient client)
        {
            if (DefaultCommands)
                commands.InvalidLoginAttempt(client.IUser);

            Stats.InvalidLoginAttempts++;
        }

        public static void LoginGranted(IClient client)
        {
            if (DefaultCommands)
                commands.LoginGranted(client.IUser);
        }

        public static void AdminLevelChanged(IClient client)
        {
            if (DefaultCommands)
                commands.AdminLevelChanged(client.IUser);
        }

        public static bool Registering(IClient client)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.Registering(client.IUser);

            return result;
        }

        public static void Registered(IClient client)
        {
            if (DefaultCommands)
                commands.Registered(client.IUser);
        }

        public static void Unregistered(IClient client)
        {
            if (DefaultCommands)
                commands.Unregistered(client.IUser);
        }

        public static void CaptchaSending(IClient client)
        {
            if (DefaultCommands)
                commands.CaptchaSending(client.IUser);
        }

        public static void CaptchaReply(IClient client, String reply)
        {
            if (DefaultCommands)
                commands.CaptchaReply(client.IUser, reply);
        }

        public static bool VroomChanging(IClient client, ushort vroom)
        {
            bool result = true;

            if (DefaultCommands)
                result = commands.VroomChanging(client.IUser, vroom);

            return result;
        }

        public static void VroomChanged(IClient client)
        {
            if (DefaultCommands)
                commands.VroomChanged(client.IUser);
        }
    }
}
