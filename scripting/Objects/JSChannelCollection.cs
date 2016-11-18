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

namespace scripting.Objects
{
    class JSChannelCollection : ObjectInstance
    {
        private int count { get; set; }

        protected override string InternalClassName
        {
            get { return "ChannelCollection"; }
        }

        internal JSChannelCollection(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSChannelCollection(ObjectInstance prototype, JSChannel[] items, String scriptName)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["ChannelCollection"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.count = 0;

            foreach (JSChannel i in items)
                this.SetPropertyValue((uint)this.count++, i, throwOnError: true);
        }

        [JSProperty(Name = "length")]
        public int Length
        {
            get { return this.count; }
            set { }
        }
    }
}
