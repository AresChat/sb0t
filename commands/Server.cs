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
using iconnect;

namespace commands
{
    class Server
    {
        private static IHostApp Callback { get; set; }

        /// <summary>
        /// Discover if this user is allowed to load or unload scripts
        /// </summary>
        public static bool CanScript(IUser user)
        {
            if (user.Link.IsLinked)
                return false;

            switch (Scripting.ScriptLevel)
            {
                case 4:
                    return user.Owner;

                case 3:
                    return user.Level >= ILevel.Host;

                case 2:
                    return user.Level >= ILevel.Administrator;

                case 1:
                    return user.Level >= ILevel.Moderator;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Send a public message to one user
        /// </summary>
        public static void PublicToTarget(IUser client, String name, String text)
        {
            Callback.PublicToTarget(client, name, text);
        }

        /// <summary>
        /// Send an emote message to one user
        /// </summary>
        public static void EmoteToTarget(IUser client, String name, String text)
        {
            Callback.EmoteToTarget(client, name, text);
        }

        /// <summary>
        /// Path to data folder
        /// </summary>
        public static String DataPath
        {
            get { return Callback.DataPath; }
        }

        /// <summary>
        /// Spell Checker
        /// </summary>
        public static ISpell Spelling
        {
            get { return Callback.Spelling; }
        }

        /// <summary>
        /// Password Accounts
        /// </summary>
        public static IAccounts Accounts
        {
            get { return Callback.Accounts; }
        }

        /// <summary>
        /// Channel search helper
        /// </summary>
        public static IChannels Channels
        {
            get { return Callback.Channels; }
        }

        public static void SetCallback(IHostApp cb)
        {
            Callback = cb;
        }

        /// <summary>
        /// Access to the link hub
        /// </summary>
        public static IHub Link
        {
            get { return Callback.Hub; }
        }

        /// <summary>
        /// Access to the link hub
        /// </summary>
        public static IScripting Scripting
        {
            get { return Callback.Scripting; }
        }

        /// <summary>
        /// Access to the user pools
        /// </summary>
        public static IPool Users
        {
            get { return Callback.Users; }
        }

        /// <summary>
        /// Hashlink encode/decoder
        /// </summary>
        public static IHashlink Hashlinks
        {
            get { return Callback.Hashlinks; }
        }

        /// <summary>
        /// Compression utility
        /// </summary>
        public static ICompression Compression
        {
            get { return Callback.Compression; }
        }

        /// <summary>
        /// Server statistics
        /// </summary>
        public static IStats Stats
        {
            get { return Callback.Stats; }
        }

        /// <summary>
        /// Chatroom properties
        /// </summary>
        public static IRoom Chatroom
        {
            get { return Callback.Room; }
        }

        /// <summary>
        /// Send an announcement to all users in all user pools
        /// </summary>
        /// <param name="text"></param>
        public static void Print(String text, bool send_link = false)
        {
            Callback.Users.Ares(x => x.Print(text));
            Callback.Users.Web(x => x.Print(text));

            if (Callback.Hub.IsLinked && send_link)
                Callback.Hub.ForEachLeaf(x => x.Print(text));
        }

        /// <summary>
        /// Send an announcement to all users in all user pools if they are in a vroom
        /// </summary>
        /// <param name="text"></param>
        public static void Print(ushort vroom, String text, bool send_link = false)
        {
            Callback.Users.Ares(x => { if (x.Vroom == vroom) x.Print(text); });
            Callback.Users.Web(x => { if (x.Vroom == vroom) x.Print(text); });

            if (Callback.Hub.IsLinked && send_link)
                Callback.Hub.ForEachLeaf(x => x.Print(vroom, text));
        }

        /// <summary>
        /// Send an announcement to all users in all user pools if their admin level is high enough
        /// </summary>
        /// <param name="text"></param>
        public static void Print(ILevel level, String text, bool send_link = false)
        {
            Callback.Users.Ares(x => { if (x.Level >= level) x.Print(text); });
            Callback.Users.Web(x => { if (x.Level >= level) x.Print(text); });

            if (Callback.Hub.IsLinked && send_link)
                Callback.Hub.ForEachLeaf(x => x.Print(level, text));
        }

        /// <summary>
        /// Send an announcement to specific users
        /// </summary>
        /// <param name="text"></param>
        /// <param name="predicate"></param>
        public static void Print(String text, Predicate<IUser> predicate)
        {
            Callback.Users.All(x =>
            {
                if (predicate(x))
                    x.Print(text);
            });
        }

        /// <summary>
        /// Write to the debug log
        /// </summary>
        /// <param name="text"></param>
        public static void WriteLog(String text)
        {
            Callback.WriteLog(text);
        }

        /// <summary>
        /// Get current timestamp
        /// </summary>
        public static uint Time
        {
            get { return Callback.Timestamp; }
        }

        /// <summary>
        /// Get a user defined minimum admin level for a default command
        /// </summary>
        /// <param name="command">command name</param>
        /// <returns></returns>
        public static ILevel GetLevel(String command)
        {
            ILevel result = Callback.GetLevel(command); // get preferred level for command

            if (result == ILevel.Regular) // preferred level is Regular
                if (!Settings.General) // but General is off?
                    result = ILevel.Moderator; // we change the preferred level to Moderator

            return result;
        }

        /// <summary>Clear the ban list</summary>
        public static void ClearBans()
        {
            Callback.ClearBans();
        }
    }
}
