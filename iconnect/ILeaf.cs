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
    /// <summary>Link leaf</summary>
    public interface ILeaf
    {
        /// <summary>Leaf identification</summary>
        uint Ident { get; }
        /// <summary>Leaf name</summary>
        String Name { get; }
        /// <summary>Leaf ip address</summary>
        IPAddress ExternalIP { get; }
        /// <summary>Leaf port</summary>
        ushort Port { get; }
        /// <summary>Perform action on the user collection for this leaf</summary>
        void ForEachUser(Action<IUser> action);
        /// <summary>Print to all users in this leaf</summary>
        void Print(String text);
        /// <summary>Print to all users in this leaf if they are in a vroom</summary>
        void Print(ushort vroom, String text);
        /// <summary>Print to all users in this leaf if their admin level is high enough</summary>
        void Print(ILevel level, String text);
        /// <summary>Send a text to the users of this leaf</summary>
        void SendText(String sender, String text);
        /// <summary>Send an emote to the users of this leaf</summary>
        void SendEmote(String sender, String text);
        /// <summary>Send a scribble to this leaf</summary>
        void Scribble(String sender, byte[] img, int height);
    }
}
