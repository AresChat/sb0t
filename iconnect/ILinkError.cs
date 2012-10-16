using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    /// <summary>LinkError</summary>
    public enum ILinkError : int
    {
        /// <summary>Unable to connect to hub</summary>
        UnableToConnect = 0,
        /// <summary>Connection to hub was lost</summary>
        RemoteDisconnect = 1,
        /// <summary>Didn't handshake with hub in time</summary>
        HandshakeTimeout = 2,
        /// <summary>Didn't ping the hub in time</summary>
        PingTimeout = 3,
        /// <summary>Incompatible protocol version with hub</summary>
        BadProtocol = 4,

        /// <summary>Hub is not accepting leaves</summary>
        HubException_NotAcceptingLeaves = 10,
        /// <summary>Incompatible protocol version with hub</summary>
        HubException_WantsHigherProtocol = 11,
        /// <summary>Your chatroom is not on the hub's trusted list</summary>
        HubException_DoesNotTrustYou = 12,
        /// <summary>Didn't handshake with hub in time</summary>
        HubException_HandshakeTimeout = 14,
        /// <summary>Didn't ping the hub in time</summary>
        HubException_PingTimeout = 15,
        /// <summary>Incompatible protocol version with hub</summary>
        HubException_BadProtocol = 16,

        /// <summary>You attempted to connect to a hub while already connected to another hub</summary>
        AlreadyLinking = 20,
        /// <summary>You cannot connect to a hub while you are in hub mode</summary>
        HubMode = 21,
        /// <summary>Your hashlink was not valid</summary>
        InvalidHashlink = 22,
        /// <summary>You attempted to disconnect from a hub while not connected to a hub</summary>
        WasNotLinking = 23,
        /// <summary>Your link mode is set to disabled</summary>
        EnableLeafLinking = 24
    }
}
