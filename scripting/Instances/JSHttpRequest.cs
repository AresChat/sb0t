using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Instances
{
    [JSEmbed(Name = "HttpRequest")]
    class JSHttpRequest : ClrFunction
    {
        public JSHttpRequest(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "HttpRequest", new JSHttpRequestInstance(engine.Object.InstancePrototype))
        {

        }

        [JSConstructorFunction]
        public JSHttpRequestInstance Construct()
        {
            return new JSHttpRequestInstance(this.InstancePrototype);
        }
    }
}
