using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.ObjectPrototypes
{
    [JSEmbed(Name = "UserFont")]
    class JSUserFont : ClrFunction
    {
        public JSUserFont(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "UserFont", new Objects.JSUserFont(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUserFont Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSUserFont Construct(object a)
        {
            return null;
        }
    }
}
