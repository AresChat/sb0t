using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.ObjectPrototypes
{
    [JSEmbed(Name = "ChannelCollection")]
    class JSChannelCollection : ClrFunction
    {
        public JSChannelCollection(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "ChannelCollection", new Objects.JSChannelCollection(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSChannelCollection Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSChannelCollection Construct(object a)
        {
            return null;
        }
    }
}
