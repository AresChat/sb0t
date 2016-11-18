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
using iconnect;

namespace scripting.Objects
{
    class JSChannel : ObjectInstance
    {
        public IChannelItem Item { get; set; }

        internal JSChannel(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSChannel(ObjectInstance prototype, IChannelItem item)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["Channel"]).InstancePrototype)
        {
            this.Item = item;
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Channel"; }
        }

        [JSProperty(Name = "hashlink")]
        public String Hashlink
        {
            get
            {
                Hashlink obj = new Hashlink
                {
                    IP = this.Item.IP,
                    Name = this.Item.Name,
                    Port = this.Item.Port
                };

                return "arlnk://" + Server.Hashlinks.Encrypt(obj);
            }
            set { }
        }

        [JSProperty(Name = "name")]
        public String Name
        {
            get { return this.Item.Name; }
            set { }
        }

        [JSProperty(Name = "topic")]
        public String Topic
        {
            get { return this.Item.Topic; }
            set { }
        }

        [JSProperty(Name = "version")]
        public String Version
        {
            get { return this.Item.Version; }
            set { }
        }

        [JSProperty(Name = "userCount")]
        public int Users
        {
            get { return this.Item.Users; }
            set { }
        }

        [JSProperty(Name = "port")]
        public int Port
        {
            get { return this.Item.Port; }
            set { }
        }

        [JSProperty(Name = "externalIp")]
        public String IP
        {
            get { return this.Item.IP.ToString(); }
            set { }
        }

        [JSProperty(Name = "language")]
        public int Language
        {
            get { return this.Item.Language; }
            set { }
        }
    }
}
