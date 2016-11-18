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
    [JSEmbed(Name = "Base64")]
    class JSBase64 : ObjectInstance
    {
        public JSBase64(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Base64"; }
        }

        [JSFunction(Name = "encode", IsWritable = false, IsEnumerable = true)]
        public static String Encode(object a)
        {
            if (a is Null || a is Undefined)
                return null;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(a.ToString()));
        }

        [JSFunction(Name = "decode", IsWritable = false, IsEnumerable = true)]
        public static String Decode(object a)
        {
            if (a is String || a is ConcatenatedString)
            {
                String str = a.ToString();

                try
                {
                    return Encoding.UTF8.GetString(Convert.FromBase64String(str));
                }
                catch { }
            }

            return null;
        }
    }
}
