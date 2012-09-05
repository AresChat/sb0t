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
    }
}
