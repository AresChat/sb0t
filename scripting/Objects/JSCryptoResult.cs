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
    class JSCryptoResult : ObjectInstance
    {
        internal byte[] data;

        public JSCryptoResult(ObjectInstance prototype, byte[] data)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["CryptoResult"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.data = data;
        }

        internal JSCryptoResult(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "CryptoResult"; }
        }

        [JSFunction(Name = "toHex", IsWritable = false, IsEnumerable = true)]
        public String ToHex()
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in this.data)
                sb.AppendFormat("{0:X2}", b);

            return sb.ToString();
        }

        [JSFunction(Name = "toBase64", IsWritable = false, IsEnumerable = true)]
        public String ToBase64()
        {
            return Convert.ToBase64String(this.data);
        }

        [JSFunction(Name = "toArray", IsWritable = false, IsEnumerable = true)]
        public ArrayInstance ToArray()
        {
            object[] results = new object[this.data.Length];

            for (int i = 0; i < results.Length; i++)
                results[i] = (int)this.data[i];

            return this.Engine.Array.New(results);
        }
    }
}
