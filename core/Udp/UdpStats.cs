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
        public static uint SENDNODES { get; set; }
        public static uint ACKNODES { get; set; }
        public static uint WANTCHECKFIREWALL { get; set; }
        public static uint READYTOCHECKFIREWALL { get; set; }
        public static uint PROCEEDCHECKFIREWALL { get; set; }
        public static uint CHECKFIREWALLBUSY { get; set; }

        public static void Reset()
        {
            SENDINFO = 0;
            ACKINFO = 0;
            ADDIPS = 0;
            ACKIPS = 0;
            SENDNODES = 0;
            ACKNODES = 0;
            WANTCHECKFIREWALL = 0;
            READYTOCHECKFIREWALL = 0;
            PROCEEDCHECKFIREWALL = 0;
            CHECKFIREWALLBUSY = 0;
        }

        public static void Record(UdpMsg msg)
        {
            switch (msg)
            {
                case UdpMsg.OP_SERVERLIST_SENDINFO:
                    SENDINFO++;
                    break;

                case UdpMsg.OP_SERVERLIST_ACKINFO:
                    ACKINFO++;
                    break;

                case UdpMsg.OP_SERVERLIST_ADDIPS:
                    ADDIPS++;
                    break;

                case UdpMsg.OP_SERVERLIST_ACKIPS:
                    ACKIPS++;
                    break;

                case UdpMsg.OP_SERVERLIST_SENDNODES:
                    SENDNODES++;
                    break;

                case UdpMsg.OP_SERVERLIST_ACKNODES:
                    ACKNODES++;
                    break;

                case UdpMsg.OP_SERVERLIST_WANTCHECKFIREWALL:
                    WANTCHECKFIREWALL++;
                    break;

                case UdpMsg.OP_SERVERLIST_READYTOCHECKFIREWALL:
                    READYTOCHECKFIREWALL++;
                    break;

                case UdpMsg.OP_SERVERLIST_PROCEEDCHECKFIREWALL:
                    PROCEEDCHECKFIREWALL++;
                    break;

                case UdpMsg.OP_SERVERLIST_CHECKFIREWALLBUSY:
                    CHECKFIREWALLBUSY++;
                    break;
            }
        }
    }
}
