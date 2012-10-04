using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.Udp
{
    class UdpProcessor
    {
        public static void Eval(UdpItem item, UdpListener udp, ulong time)
        {
            switch (item.Msg)
            {
                case UdpMsg.OP_SERVERLIST_SENDINFO:
                    break;

                case UdpMsg.OP_SERVERLIST_ADDIPS:
                    break;

                case UdpMsg.OP_SERVERLIST_ACKIPS:
                    break;

                case UdpMsg.OP_SERVERLIST_SENDNODES:
                    break;

                case UdpMsg.OP_SERVERLIST_WANTCHECKFIREWALL:
                    break;

                case UdpMsg.OP_SERVERLIST_READYTOCHECKFIREWALL:
                    break;

                case UdpMsg.OP_SERVERLIST_PROCEEDCHECKFIREWALL:
                    break;

                case UdpMsg.OP_SERVERLIST_CHECKFIREWALLBUSY:
                    break;
            }
        }

    }
}
