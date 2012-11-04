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

        public JSChannelCollection(ObjectInstance prototype, JSChannel[] items, String scriptName)
            : base(prototype)
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

        public override string ToString()
        {
            return "[object ChannelCollection]";
        }
    }
}
