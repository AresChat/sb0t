using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Instances
{
    [JSEmbed(Name = "Sql")]
    class JSSql : ClrFunction
    {
        public JSSql(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "Sql", new JSSqlInstance(engine.Object.InstancePrototype))
        {

        }

        [JSConstructorFunction]
        public JSSqlInstance Construct()
        {
            return new JSSqlInstance(this.InstancePrototype);
        }
    }
}
