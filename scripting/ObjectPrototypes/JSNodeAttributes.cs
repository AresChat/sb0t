using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.ObjectPrototypes
{
    [JSEmbed(Name = "NodeAttributes")]
    class JSNodeAttributes : ClrFunction
    {
        public JSNodeAttributes(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "NodeAttributes", new Objects.JSNodeAttributes(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSNodeAttributes Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSNodeAttributes Construct(object a)
        {
            return null;
        }
    }
}
