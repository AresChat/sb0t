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
        HubException_BadProtocol = 16,

        AlreadyLinking = 20,
        HubMode = 21,
        InvalidHashlink = 22,
        WasNotLinking = 23,
        EnableLeafLinking = 24
    }
}
