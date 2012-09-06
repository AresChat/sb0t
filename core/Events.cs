using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    class Events
    {
        public static void PacketReceived(AresClient client, TCPMsg msg, byte[] packet) { }

        public static bool Joining(AresClient client) { return true; }

        public static void Joined(AresClient client) { }

        public static void Rejected(AresClient client, RejectedMsg msg) { }

        public static void Parting(AresClient client) { }

        public static void Parted(AresClient client) { }

        public static bool AvatarReceived(AresClient client) { return true; }

        public static bool PersonalMessageReceived(AresClient client, String text) { return true; }

        public static void TextReceived(AresClient client, String text) { }

        public static String TextSending(AresClient client, String text) { return text; }

        public static void TextSent(AresClient client, String text) { }

        public static void EmoteReceived(AresClient client, String text) { }

        public static String EmoteSending(AresClient client, String text) { return text; }

        public static void EmoteSent(AresClient client, String text) { }

        public static void PrivateSending(AresClient client, AresClient target, PMEventArgs e) { }

        public static void PrivateSent(AresClient client, AresClient target, String text) { }

        public static void BotPrivateSending(AresClient client, PMEventArgs e) { }

        public static void BotPrivateSent(AresClient client, String text) { }

        public static void Command(AresClient client, String command, AresClient target, String args) { }

        
    }
}
