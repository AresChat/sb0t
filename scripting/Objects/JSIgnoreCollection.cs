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
    class JSIgnoreCollection : ObjectInstance
    {
        private int count { get; set; }

        protected override string InternalClassName
        {
            get { return "IgnoreCollection"; }
        }

        internal JSIgnoreCollection(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSIgnoreCollection(ObjectInstance prototype, String[] ignores, String scriptName)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["IgnoreCollection"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.count = 0;

            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == scriptName);

            if (script != null)
            {
                foreach (String str in ignores)
                    if (!String.IsNullOrEmpty(str))
                    {
                        JSUser u = script.GetIgnoredUser(str);

                        if (u == null)
                            script.leaves.ForEach(x =>
                            {
                                u = x.FindUser(str);

                                if (u != null)
                                    return;
                            });

                        if (u != null)
                            this.SetPropertyValue((uint)this.count++, u, throwOnError: true);
                    }
            }
        }

        [JSProperty(Name = "length")]
        public int Length
        {
            get { return this.count; }
            set { }
        }
    }
}
