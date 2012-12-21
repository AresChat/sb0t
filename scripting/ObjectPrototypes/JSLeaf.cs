using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.ObjectPrototypes
{
    [JSEmbed(Name = "Leaf")]
    class JSLeaf : ClrFunction
    {
        public JSLeaf(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "Leaf", new Objects.JSLeaf(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSLeaf Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSLeaf Construct(object a)
        {
            return null;
        }
    }
}
