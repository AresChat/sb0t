using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.LinkHub
{
    enum LinkError : byte
    {
        Unavailable = 0,
        ExpiredProtocol = 1,
        Untrusted = 2,
        HandshakeTimeout = 3,
        PingTimeout = 4,
        BadProtocol = 6
    }
}
