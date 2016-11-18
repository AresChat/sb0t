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
    /// <summary>Channel search result</summary>    
    public interface IChannelItem
    {
        /// <summary>Name</summary>
        String Name { get; }
        /// <summary>Topic</summary>
        String Topic { get; }
        /// <summary>Version</summary>
        String Version { get; }
        /// <summary>User count</summary>
        ushort Users { get; }
        /// <summary>Port</summary>
        ushort Port { get; }
        /// <summary>IP Address</summary>
        IPAddress IP { get; }
        /// <summary>Language Code</summary>
        byte Language { get; }
    }
}
