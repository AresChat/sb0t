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
    /// <summary>Chatroom Properties</summary>
    public interface IRoom
    {
        /// <summary>Get or set the global custom names setting</summary>
        bool CustomNamesEnabled { get; set; }
        /// <summary>Get the chatroom name</summary>
        String Name { get; }
        /// <summary>Get or set the chatroom main topic</summary>
        String Topic { get; set; }
        /// <summary>Get the chatroom external IP address</summary>
        IPAddress ExternalIP { get; }
        /// <summary>Get the chatroom local IP address</summary>
        IPAddress LocalIP { get; }
        /// <summary>Get the chatroom data port</summary>
        ushort Port { get; }
        /// <summary>Get the chatroom hashlink</summary>
        String Hashlink { get; }
        /// <summary>Get the chatroom running status</summary>
        bool IsRunning { get; }
        /// <summary>Get the chatroom preferred langauge code</summary>
        byte Language { get; }
        /// <summary>Update the main URL tag</summary>
        void UpdateURL(String address, String text);
        /// <summary>Clear the main URL tag</summary>
        void ClearURL();
        /// <summary>Get the chatroom bot name</summary>
        String BotName { get; }
        /// <summary>Get the timestamp when the server started</summary>
        uint StartTime { get; }
        /// <summary>Get the minimum age to join the chatroom</summary>
        byte MinimumAge { get; }
        /// <summary>Get the server version</summary>
        String Version { get; }
    }
}
