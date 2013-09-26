using System;
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
