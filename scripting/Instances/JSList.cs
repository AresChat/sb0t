using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Instances
{
    [JSEmbed(Name = "List")]
    class JSList : ClrFunction
    {
        public JSList(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "List", new JSListInstance(engine.Object.InstancePrototype))
        {

        }

        [JSConstructorFunction]
        public JSListInstance Construct(params object[] items)
        {
            return new JSListInstance(this.InstancePrototype, items);
        }
    }
}
