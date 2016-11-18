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
    class JSUserFont : ObjectInstance
    {
        internal IUser parent;

        public JSUserFont(ObjectInstance prototype, IUser user, String script)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["UserFont"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.parent = user;
        }

        internal JSUserFont(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "UserFont"; }
        }

        [JSProperty(Name = "enabled")]
        public bool Enabled
        {
            get { return this.parent.Font.Enabled; }
            set { }
        }

        [JSProperty(Name = "nameColor")]
        public String NameColor
        {
            get { return this.parent.Font.NameColor; }
            set { }
        }

        [JSProperty(Name = "textColor")]
        public String TextColor 
        {
            get { return this.parent.Font.TextColor; }
            set { }
        }

        [JSProperty(Name = "family")]
        public String Name
        {
            get { return this.parent.Font.FontName; }
            set { }
        }
    }
}
