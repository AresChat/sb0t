using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.ObjectPrototypes
{
    [JSEmbed(Name = "BannedUser")]
    class JSBannedUser : ClrFunction
    {
        public JSBannedUser(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "BannedUser", new Objects.JSBannedUser(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSBannedUser Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSBannedUser Construct(object a)
        {
            return null;
        }
    }
}
