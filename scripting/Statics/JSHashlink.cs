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
using System.Net;
using Jurassic;
using Jurassic.Library;
using iconnect;

namespace scripting.Statics
{
    [JSEmbed(Name = "Hashlink")]
    class JSHashlink : ObjectInstance
    {
        public JSHashlink(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Hashlink"; }
        }

        [JSFunction(Name = "encode", IsWritable = false, IsEnumerable = true)]
        public static String Encode(object a)
        {
            if (a is ObjectInstance)
            {
                try
                {
                    ObjectInstance obj = (ObjectInstance)a;
                    Hashlink hashlink = new Hashlink
                    {
                        IP = IPAddress.Parse(obj.GetPropertyValue("ip").ToString()),
                        Port = ushort.Parse(obj.GetPropertyValue("port").ToString()),
                        Name = obj.GetPropertyValue("name").ToString()
                    };

                    String str = "arlnk://" + Server.Hashlinks.Encrypt(hashlink);

                    if (str != null)
                        return str;
                }
                catch { }
            }

            return null;
        }

        [JSFunction(Name = "decode", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static Objects.JSHashlinkResult Decode(ScriptEngine eng, object a)
        {
            if (a is String || a is ConcatenatedString)
            {
                String str = a.ToString();

                try
                {
                    IHashlinkRoom hashlink = Server.Hashlinks.Decrypt(a.ToString());

                    if (hashlink != null)
                        return new Objects.JSHashlinkResult(eng.Object.InstancePrototype, hashlink);
                }
                catch { }
            }

            return null;
        }
    }
}
