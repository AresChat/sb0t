using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.ObjectPrototypes
{
    [JSEmbed(Name = "AvatarImage")]
    class JSAvatarImage : ClrFunction
    {
        public JSAvatarImage(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "AvatarImage", new Objects.JSAvatarImage(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSAvatarImage Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSAvatarImage Construct(object a)
        {
            return null;
        }
    }
}
