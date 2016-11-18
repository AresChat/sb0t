/*
    sb0t ares chat server
    Copyright (C) 2016  AresChat

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System;
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
