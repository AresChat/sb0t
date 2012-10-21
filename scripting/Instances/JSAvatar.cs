using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Instances
{
    [JSEmbed(Name = "Avatar")]
    class JSAvatar : ClrFunction
    {
        public JSAvatar(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "Avatar", new JSAvatarInstance(engine.Object.InstancePrototype))
        {

        }

        [JSConstructorFunction]
        public JSAvatarInstance Construct()
        {
            return new JSAvatarInstance(this.InstancePrototype);
        }
    }
}
