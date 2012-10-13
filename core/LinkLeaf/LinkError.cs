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
        BadProtocol = 4,

        HubException_NotAcceptingLeaves = 10,
        HubException_WantsHigherProtocol = 11,
        HubException_DoesNotTrustYou = 12,
        HubException_HandshakeTimeout = 14,
        HubException_PingTimeout = 15,
        HubException_BadProtocol = 16
    }
}
