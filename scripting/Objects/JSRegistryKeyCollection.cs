using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Objects
{
    class JSRegistryKeyCollection : ObjectInstance
    {
        private int count { get; set; }

        public JSRegistryKeyCollection(ObjectInstance prototype, String[] keys, String scriptName)
            : base(prototype)
        {
            this.PopulateFunctions();
            this.count = 0;

            foreach (String str in keys)
                this.SetPropertyValue((uint)this.count++, str, throwOnError: true);
        }

        [JSProperty(Name = "length")]
        public int Length
        {
            get { return this.count; }
            set { }
        }

        public override string ToString()
        {
            return "[object RegistryKeyCollection]";
        }
    }
}
