using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Objects
{
    class JSChannelCollection : ObjectInstance
    {
        private int count { get; set; }

        protected override string InternalClassName
        {
            get { return "ChannelCollection"; }
        }

        internal JSChannelCollection(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSChannelCollection(ObjectInstance prototype, JSChannel[] items, String scriptName)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["ChannelCollection"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.count = 0;

            foreach (JSChannel i in items)
                this.SetPropertyValue((uint)this.count++, i, throwOnError: true);
        }

        [JSProperty(Name = "length")]
        public int Length
        {
            get { return this.count; }
            set { }
        }
    }
}
