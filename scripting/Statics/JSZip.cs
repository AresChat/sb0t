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
    [JSEmbed(Name = "Zip")]
    class JSZip : ObjectInstance
    {
        public JSZip(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Zip"; }
        }

        [JSFunction(Name = "compress", IsWritable = false, IsEnumerable = true)]
        public static String Compress(object a)
        {
            String result = null;

            if (!(a is Undefined))
            {
                try
                {
                    String str = a.ToString();
                    byte[] buf = Encoding.UTF8.GetBytes(str);
                    buf = Server.Compression.Compress(buf);
                    result = Encoding.Default.GetString(buf);
                }
                catch { }
            }

            return result;
        }

        [JSFunction(Name = "uncompress", IsWritable = false, IsEnumerable = true)]
        public static String Uncompress(object a)
        {
            String result = null;

            if (!(a is Undefined))
            {
                try
                {
                    String str = a.ToString();
                    byte[] buf = Encoding.Default.GetBytes(str);
                    buf = Server.Compression.Decompress(buf);
                    result = Encoding.UTF8.GetString(buf);
                }
                catch { }
            }

            return result;
        }
    }
}
