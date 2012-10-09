using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.LinkLeaf
{
    enum LinkError : byte
    {
        UnableToConnect = 0,
        RemoteDisconnect = 1,
        HandshakeTimeout = 2,
        PingTimeout = 3,
        BadProtocol = 4
    }
}
