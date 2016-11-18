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
    /// <summary>Server Callback</summary>
    public interface IHostApp
    {
        /// <summary>Channel search helper</summary>
        IChannels Channels { get; }
        /// <summary>Scripting credentials</summary>
        IScripting Scripting { get; }
        /// <summary>User pool</summary>
        IPool Users { get; }
        /// <summary>Folder to save items for this extension</summary>
        String DataPath { get; }
        /// <summary>Chatroom statistics</summary>
        IStats Stats { get; }
        /// <summary>Chatroom properties</summary>
        IRoom Room { get; }
        /// <summary>Hashlink encoder / decoder</summary>
        IHashlink Hashlinks { get; }
        /// <summary>Compression utility</summary>
        ICompression Compression { get; }
        /// <summary>Write to the debug log</summary>
        void WriteLog(String text);
        /// <summary>Write to the chat log</summary>
        void WriteChatLog(String text);
        /// <summary>Get current timestamp</summary>
        uint Timestamp { get; }
        /// <summary>Get a user defined minimum admin level for a default command</summary>
        ILevel GetLevel(String command);
        /// <summary>Clear the ban list</summary>
        void ClearBans();
        /// <summary>Link hub</summary>
        IHub Hub { get; }
        /// <summary>Get current tickcount</summary>
        ulong Ticks { get; }
        /// <summary>Spell Checker</summary>
        ISpell Spelling { get; }
        /// <summary>Password Accounts</summary>
        IAccounts Accounts { get; }
        /// <summary>Send a public message to one user</summary>
        void PublicToTarget(IUser client, String sender, String text);
        /// <summary>Send an emote message to one user</summary>
        void EmoteToTarget(IUser client, String sender, String text);
    }
}
