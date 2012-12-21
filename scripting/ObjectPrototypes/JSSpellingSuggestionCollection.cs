using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.ObjectPrototypes
{
    [JSEmbed(Name = "SpellingSuggestionCollection")]
    class JSSpellingSuggestionCollection : ClrFunction
    {
        public JSSpellingSuggestionCollection(ScriptEngine eng)
            : base(eng.Function.InstancePrototype, "SpellingSuggestionCollection", new Objects.JSSpellingSuggestionCollection(eng))
        {

        }

        [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSSpellingSuggestionCollection Call(object a)
        {
            return null;
        }

        [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public Objects.JSSpellingSuggestionCollection Construct(object a)
        {
            return null;
        }
    }
}
