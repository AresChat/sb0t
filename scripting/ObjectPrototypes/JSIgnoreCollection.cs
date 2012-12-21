using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.ObjectPrototypes
{
    [JSEmbed(Name = "IgnoreCollection")]
    class JSIgnoreCollection : ClrFunction
    {
        public JSIgnoreCollection(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "IgnoreCollection", new Objects.JSIgnoreCollection(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSIgnoreCollection Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSIgnoreCollection Construct(object a)
        {
            return null;
        }
    }
}
