using System;
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
