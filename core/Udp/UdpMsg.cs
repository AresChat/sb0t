using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.Udp
{
    enum UdpMsg : byte
    {
        OP_SERVERLIST_SENDINFO = 2,
        OP_SERVERLIST_ACKINFO = 3,
        OP_SERVERLIST_ADDIPS = 11,
        OP_SERVERLIST_ACKIPS = 12,
        OP_SERVERLIST_SENDNODES = 21,
        OP_SERVERLIST_ACKNODES = 22,
        OP_SERVERLIST_WANTCHECKFIREWALL = 31,
        OP_SERVERLIST_READYTOCHECKFIREWALL = 32,
        OP_SERVERLIST_PROCEEDCHECKFIREWALL = 33,
        OP_SERVERLIST_CHECKFIREWALLBUSY = 34
    }
}
