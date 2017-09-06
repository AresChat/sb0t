using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jurassic;
using Jurassic.Library;

namespace scripting.ObjectPrototypes
{
    [JSObject(Name = "ProxyCheckResult")]
    class JSProxyCheckResult : ClrFunction
    {
        public JSProxyCheckResult(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "ProxyCheckResult", new Objects.JSProxyCheckResult(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSProxyCheckResult Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSProxyCheckResult Construct(object a)
        {
            return null;
        }
    }
}
