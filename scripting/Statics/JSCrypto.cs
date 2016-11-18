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
using System.Security.Cryptography;
using Jurassic;
using Jurassic.Library;

namespace scripting.Statics
{
    [JSEmbed(Name = "Crypto")]
    class JSCrypto : ObjectInstance
    {
        public JSCrypto(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Crypto"; }
        }

        [JSFunction(Name = "md5hash", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static Objects.JSCryptoResult MD5Hash(ScriptEngine eng, object a)
        {
            if (a is Null || a is Undefined)
                return null;

            String str = a.ToString();
            byte[] buf = Encoding.UTF8.GetBytes(str);

            using (MD5 md5 = MD5.Create())
                buf = md5.ComputeHash(buf);

            return new Objects.JSCryptoResult(eng.Object.InstancePrototype, buf);
        }

        [JSFunction(Name = "sha1hash", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static Objects.JSCryptoResult SHA1hash(ScriptEngine eng, object a)
        {
            if (a is Null || a is Undefined)
                return null;

            String str = a.ToString();
            byte[] buf = Encoding.UTF8.GetBytes(str);

            using (SHA1 md5 = SHA1.Create())
                buf = md5.ComputeHash(buf);

            return new Objects.JSCryptoResult(eng.Object.InstancePrototype, buf);
        }
    }
}
