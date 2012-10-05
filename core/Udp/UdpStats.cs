using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.Udp
{
    class UdpStats
    {
        public static uint SENDINFO { get; set; }
        public static uint ACKINFO { get; set; }
        public static uint ADDIPS { get; set; }
        public static uint ACKIPS { get; set; }

        public static void Reset()
        {
            SENDINFO = 0;
            ACKINFO = 0;
            ADDIPS = 0;
            ACKIPS = 0;
        }

        public static void Record(UdpMsg msg)
        {
            switch (msg)
            {
                case UdpMsg.OP_SERVERLIST_ACKINFO:
                    ACKINFO++;
                    break;

                case UdpMsg.OP_SERVERLIST_ADDIPS:
                    ADDIPS++;
                    break;
            }
        }
    }
}
