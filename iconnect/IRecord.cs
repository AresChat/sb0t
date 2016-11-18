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
using System.Net;

namespace iconnect
{
    /// <summary>Historic user record</summary>
    public interface IRecord
    {
        /// <summary>Get joining timestamp</summary>
        uint JoinTime { get; }
        /// <summary>Get joining user name</summary>
        String Name { get; }
        /// <summary>Get external IP address</summary>
        IPAddress ExternalIP { get; }
        /// <summary>Get local IP address</summary>
        IPAddress LocalIP { get; }
        /// <summary>Get client version</summary>
        String Version { get; }
        /// <summary>Get data port</summary>
        ushort DataPort { get; }
        /// <summary>Get 16 byte GUID</summary>
        Guid Guid { get; }
        /// <summary>Get DNS host name</summary>
        String DNS { get; }
        /// <summary>Add this user to the ban list</summary>
        void Ban();
    }
}
