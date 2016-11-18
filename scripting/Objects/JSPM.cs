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
    class JSPM : ObjectInstance
    {
        public IPrivateMsg _PM { get; set; }

        public JSPM(ObjectInstance prototype, IPrivateMsg pm)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["PM"]).InstancePrototype)
        {
            this._PM = pm;
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "PM"; }
        }

        internal JSPM(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "contains", IsWritable = false, IsEnumerable = true)]
        public bool Contains(object a)
        {
            if (a != null)
                if (!(a is Undefined) && !(a is Null))
                    return this._PM.Contains(a.ToString());

            return false;
        }

        [JSFunction(Name = "remove", IsWritable = false, IsEnumerable = true)]
        public void Remove(object a)
        {
            if (a != null)
                if (!(a is Undefined) && !(a is Null))
                    this._PM.Remove(a.ToString());
        }

        [JSFunction(Name = "replace", IsWritable = false, IsEnumerable = true)]
        public void Replace(object a, object b)
        {
            if (a != null)
                if (!(a is Undefined) && !(a is Null))
                    if (b != null)
                        if (!(b is Undefined) && !(b is Null))
                            this._PM.Replace(a.ToString(), b.ToString());
        }
    }
}
