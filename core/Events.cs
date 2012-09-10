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

        public static void BotPrivateSending(IClient client, PMEventArgs e) { }

        public static void BotPrivateSent(IClient client, String text) { }

        public static void Command(IClient client, String command, IClient target, String args) { }

        public static void FileReceived(IClient client, SharedFile file) { }

        public static bool Ignoring(IClient client, IClient target) { return true; }

        public static void IgnoredStateChanged(IClient client, IClient target, bool ignored) { }


    }
}
