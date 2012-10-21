using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Instances
{
    [JSEmbed(Name = "Timer")]
    class JSTimer : ClrFunction
    {
        public JSTimer(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "Timer", new JSTimerInstance(engine.Object.InstancePrototype))
        {

        }

        [JSConstructorFunction]
        public JSTimerInstance Construct()
        {
            return new JSTimerInstance(this.InstancePrototype);
        } 
    }
}
