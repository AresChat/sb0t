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
    /// <summary>Link hub</summary>
    public interface IHub
    {
        /// <summary>Connect to a hub</summary>
        void Connect(String hashlink);
        /// <summary>Disconnect from a hub</summary>
        bool Disconnect();
        /// <summary>Get status of connection to hub</summary>
        bool IsLinked { get; }
        /// <summary>Get hub name</summary>
        String Name { get; }
        /// <summary>Get hub ip address</summary>
        IPAddress ExternalIP { get; }
        /// <summary>Get hub port</summary>
        ushort Port { get; }
        /// <summary>Perform action on the leaf collection</summary>
        void ForEachLeaf(Action<ILeaf> action);
        /// <summary>Find a leaf based on a condition</summary>
        ILeaf FindLeaf(Predicate<ILeaf> predicate);
    }
}
