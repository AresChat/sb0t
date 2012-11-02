using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Statics
{
    [JSEmbed(Name = "Spelling")]
    class JSSpelling : ObjectInstance
    {
        public JSSpelling(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "check", IsWritable = false, IsEnumerable = true)]
        public static String Check(object a)
        {
            if (!(a is Undefined))
            {
                String text = a.ToString();
                return Server.Spelling.Check(text);
            }

            return null;
        }

        [JSFunction(Name = "suggest", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static Objects.JSSpellingSuggestionCollection Suggest(ScriptEngine eng, object a)
        {
            List<String> results = new List<String>();

            if (!(a is Undefined))
            {
                String word = a.ToString();
                results.AddRange(Server.Spelling.Suggest(word));
            }

            return new Objects.JSSpellingSuggestionCollection(eng.Object.InstancePrototype, results.ToArray(), eng.ScriptName);
        }
    }
}
