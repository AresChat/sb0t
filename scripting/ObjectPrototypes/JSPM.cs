using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.ObjectPrototypes
{
    [JSEmbed(Name = "PM")]
    class JSPM : ClrFunction
    {
        public JSPM(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "PM", new Objects.JSPM(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSPM Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSPM Construct(object a)
        {
            return null;
        }
    }
}
