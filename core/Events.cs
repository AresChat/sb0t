using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    class Events
    {
        public static void UnhandledProtocol(IClient client, TCPMsg msg, TCPPacketReader packet, ulong tick)
        {
            UserPool.AUsers.ForEachWhere(x =>
                x.SendPacket(TCPOutbound.NoSuch(x, "Unhandled : " + client.Name + " : " + msg)), x => x.LoggedIn);
        }

        public static bool Joining(IClient client) { return true; }

        public static void Joined(IClient client) { }

        public static void Rejected(IClient client, RejectedMsg msg) { }

        public static void Parting(IClient client) { }

        public static void Parted(IClient client) { }

        public static bool AvatarReceived(IClient client) { return true; }

        public static bool PersonalMessageReceived(IClient client, String text) { return true; }

        public static void TextReceived(IClient client, String text) { }

        public static String TextSending(IClient client, String text) { return text; }

        public static void TextSent(IClient client, String text) { }

        public static void EmoteReceived(IClient client, String text) { }

        public static String EmoteSending(IClient client, String text) { return text; }

        public static void EmoteSent(IClient client, String text) { }

        public static void PrivateSending(IClient client, IClient target, PMEventArgs e) { }

        public static void PrivateSent(IClient client, IClient target, String text) { }

        public static void BotPrivateSent(IClient client, String text) { }

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
                    if (NickChanging(client, command.Substring(5)))
                    {

                    }

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
                                    target.Level = (Level)level;
                                    AccountManager.UpdateAccount(target);
                                }
                        }

                    return;
                }
            }
        }

        private static bool NickChanging(IClient client, String name) { return true; }

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
        }

        public static void FileReceived(IClient client, SharedFile file) { }

        public static bool Ignoring(IClient client, IClient target) { return true; }

        public static void IgnoredStateChanged(IClient client, IClient target, bool ignored) { }

        public static void InvalidLoginAttempt(IClient client) { }

        public static void LoginGranted(IClient client) { }

        public static void AdminLevelChanged(IClient client) { }

        public static bool Registering(IClient client) { return true; }

        public static void Registered(IClient client) { }

        public static void Unregistered(IClient client) { }

        public static void CaptchaSending(IClient client) { }

        public static void CaptchaReply(IClient client, String reply) { }

        public static void VroomChanging(IClient client, ushort vroom) { }

        public static void VroomChanged(IClient client) { }
    }
}
