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
    /// <summary>Rejection Messages</summary>
    public enum RejectedMsg
    {
        /// <summary>Too many clients from this IP address are attempting to join</summary>
        TooManyClients,
        /// <summary>Wait a few seconds between join attempts</summary>
        TooSoon,
        /// <summary>Use a different name that isn't being used by someone else</summary>
        NameInUse,
        /// <summary>You are banned from this chatroom</summary>
        Banned,
        /// <summary>You are too young to use this chatroom</summary>
        UnderAge,
        /// <summary>Your gender is not accepted in this chatroom</summary>
        UnacceptableGender,
        /// <summary>An extension or script is preventing you from joining</summary>
        UserDefined,
        /// <summary>You are using a black listed proxy server</summary>
        Proxy
    }
}
