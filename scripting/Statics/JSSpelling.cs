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
    [JSEmbed(Name = "Spelling")]
    class JSSpelling : ObjectInstance
    {
        public JSSpelling(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Spelling"; }
        }

        [JSFunction(Name = "check", IsWritable = false, IsEnumerable = true)]
        public static String Check(object a)
        {
            if (!(a is Undefined))
            {
                String text = a.ToString();
                return Server.Spelling.Check(text);
            }

            return null;
        }

        [JSFunction(Name = "suggest", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static Objects.JSSpellingSuggestionCollection Suggest(ScriptEngine eng, object a)
        {
            List<String> results = new List<String>();

            if (!(a is Undefined))
            {
                String word = a.ToString();
                results.AddRange(Server.Spelling.Suggest(word));
            }

            return new Objects.JSSpellingSuggestionCollection(eng.Object.InstancePrototype, results.ToArray(), eng.ScriptName);
        }

        [JSFunction(Name = "confirm", IsWritable = false, IsEnumerable = true)]
        public static bool Confirm(object a)
        {
            if (a is Undefined)
                return false;

            String text = a.ToString();
            return Server.Spelling.Confirm(text);
        }
    }
}
