using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.ObjectPrototypes
{
    [JSEmbed(Name = "Record")]
    class JSRecord : ClrFunction
    {
        public JSRecord(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "Record", new Objects.JSRecord(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSRecord Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSRecord Construct(object a)
        {
            return null;
        }
    }
}
