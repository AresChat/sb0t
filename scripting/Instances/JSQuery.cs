using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Instances
{
    [JSEmbed(Name = "Query")]
    class JSQuery : ClrFunction
    {
        public JSQuery(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "Query", new JSQueryInstance(engine.Object.InstancePrototype))
        {

        }

        [JSConstructorFunction]
        public JSQueryInstance Construct(String q, params object[] a)
        {
            return new JSQueryInstance(this.InstancePrototype, q, a);
        }
    }
}
