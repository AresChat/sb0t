using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.ObjectPrototypes
{
    [JSEmbed(Name = "Channel")]
    class JSChannel : ClrFunction
    {
        public JSChannel(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "Channel", new Objects.JSChannel(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSChannel Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSChannel Construct(object a)
        {
            return null;
        }
    }
}
