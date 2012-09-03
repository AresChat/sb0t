using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    class Events
    {
        public static void PacketReceived(AresClient client, TCPMsg msg, byte[] packet)
        {

        }

        public static bool Joining(AresClient client)
        {
            return true;
        }

        public static void Joined(AresClient client)
        {

        }

    }
}
