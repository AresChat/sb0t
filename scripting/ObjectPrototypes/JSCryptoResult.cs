using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.ObjectPrototypes
{
    [JSEmbed(Name = "CryptoResult")]
    class JSCryptoResult : ClrFunction
    {
        public JSCryptoResult(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "CryptoResult", new Objects.JSCryptoResult(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSCryptoResult Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSCryptoResult Construct(object a)
        {
            return null;
        }
    }
}
