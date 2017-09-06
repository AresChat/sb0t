using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jurassic;
using Jurassic.Library;

namespace scripting.Instances
{
    [JSObject(Name = "ProxyCheck")]
    class JSProxyCheck : ClrFunction
    {
        public JSProxyCheck(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "ProxyCheck", new JSProxyCheckInstance(engine.Object.InstancePrototype))
        {

        }

        [JSConstructorFunction]
        public JSProxyCheckInstance Construct(string apiKey)
        {
            return new JSProxyCheckInstance(this.InstancePrototype, apiKey);
        }
    }
}
