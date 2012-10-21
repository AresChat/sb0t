using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Instances
{
    [JSEmbed(Name = "Scribble")]
    class JSScribble : ClrFunction
    {
        public JSScribble(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "Scribble", new JSScribbleInstance(engine.Object.InstancePrototype))
        {

        }

        [JSConstructorFunction]
        public JSScribbleInstance Construct()
        {
            return new JSScribbleInstance(this.InstancePrototype);
        }
    }
}
