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

namespace scripting
{
    class ErrorDispatcher
    {
        private static List<Guid> list = new List<Guid>();

        public static void Reset()
        {
            list = new List<Guid>();
        }

        public static void AddErrors(IUser client)
        {
            if (!client.Link.IsLinked)
                if (list.FindIndex(x => x.Equals(client.Guid)) == -1)
                    list.Add(client.Guid);
        }

        public static void RemoveErrors(IUser client)
        {
            if (!client.Link.IsLinked)
                list.RemoveAll(x => x.Equals(client.Guid));
        }

        public static void SendError(String script, String msg, int line)
        {
            String str = script + ": " + msg + " at line " + line;
            String sender = Server.Chatroom.BotName;

            Server.Users.All(x =>
            {
                if (!x.Link.IsLinked)
                    if (list.FindIndex(z => z.Equals(x.Guid)) > -1)
                        x.PM(sender, str);
            });
        }
    }
}
