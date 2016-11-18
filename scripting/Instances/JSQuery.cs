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

namespace scripting.Instances
{
    [JSEmbed(Name = "Query")]
    class JSQuery : ClrFunction
    {
        public JSQuery(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "Query", new JSQueryInstance(engine.Object.InstancePrototype))
        {

        }

        [JSConstructorFunction]
        public JSQueryInstance Construct(String q, params object[] a)
        {
            return new JSQueryInstance(this.InstancePrototype, q, a);
        }
    }
}
