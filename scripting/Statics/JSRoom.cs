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
using Jurassic;
using Jurassic.Library;

namespace scripting.Statics
{
    [JSEmbed(Name = "Room")]
    class JSRoom : ObjectInstance
    {
        public JSRoom(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Room"; }
        }

        [JSProperty(Name = "version")]
        public static int Version
        {
            get { return Server.SCRIPT_VERSION; }
            set { }
        }

        [JSProperty(Name = "botName")]
        public static String BotName
        {
            get { return Server.Chatroom.BotName; }
            set { }
        }

        [JSProperty(Name = "customNames")]
        public static bool CustomNames
        {
            get { return Server.Chatroom.CustomNamesEnabled; }
            set { Server.Chatroom.CustomNamesEnabled = value; }
        }

        [JSProperty(Name = "externalIp")]
        public static String ExternalIP
        {
            get { return Server.Chatroom.ExternalIP.ToString(); }
            set { }
        }

        [JSProperty(Name = "hashlink")]
        public static String Hashlink
        {
            get { return "arlnk://" + Server.Chatroom.Hashlink; }
            set { }
        }

        [JSProperty(Name = "name")]
        public static String Name
        {
            get { return Server.Chatroom.Name; }
            set { }
        }

        [JSProperty(Name = "port")]
        public static int Port
        {
            get { return Server.Chatroom.Port; }
            set { }
        }

        [JSProperty(Name = "startTime")]
        public static double StartTime
        {
            get { return Server.Chatroom.StartTime; }
            set { }
        }

        [JSProperty(Name = "topic")]
        public static String Topic
        {
            get { return Server.Chatroom.Topic; }
            set { Server.Chatroom.Topic = value; }
        }

        [JSFunction(Name = "setUrl", IsWritable = false, IsEnumerable = true)]
        public static void SetUrl(object a, object b)
        {
            if (a == null || b == null)
                return;

            String addr = String.Empty;
            String text = String.Empty;

            if (!(a is Undefined || a is Null))
                addr = a.ToString();

            if (!(b is Undefined || b is Null))
                text = b.ToString();

            if (String.IsNullOrEmpty(addr) && String.IsNullOrEmpty(text))
                Server.Chatroom.ClearURL();
            else if (!String.IsNullOrEmpty(addr))
                if (!String.IsNullOrEmpty(text))
                    Server.Chatroom.UpdateURL(addr, text);
        }

        [JSFunction(Name = "clearUrl", IsWritable = false, IsEnumerable = true)]
        public static void ClearUrl()
        {
            Server.Chatroom.ClearURL();
        }
    }
}
