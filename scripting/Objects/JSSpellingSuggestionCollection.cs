using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Objects
{
    class JSSpellingSuggestionCollection : ObjectInstance
    {
        private int count { get; set; }

        public JSSpellingSuggestionCollection(ObjectInstance prototype, String[] items, String scriptName)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["SpellingSuggestionCollection"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.count = 0;

            foreach (String str in items)
                this.SetPropertyValue((uint)this.count++, str, throwOnError: true);
        }

        internal JSSpellingSuggestionCollection(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "SpellingSuggestionCollection"; }
        }

        [JSProperty(Name = "length")]
        public int Length
        {
            get { return this.count; }
            set { }
        }
    }
}
