using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Objects
{
    class JSIgnoreCollection : ObjectInstance
    {
        private int count { get; set; }

        public JSIgnoreCollection(ObjectInstance prototype, String[] ignores, String scriptName)
            : base(prototype)
        {
            this.PopulateFunctions();
            this.count = 0;

            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == scriptName);

            if (script != null)
            {
                foreach (String str in ignores)
                    if (!String.IsNullOrEmpty(str))
                    {
                        JSUser u = script.GetIgnoredUser(str);

                        if (u == null)
                            script.leaves.ForEach(x =>
                            {
                                u = x.FindUser(str);

                                if (u != null)
                                    return;
                            });

                        if (u != null)
                            this.SetPropertyValue((uint)this.count++, u, throwOnError: true);
                    }
            }
        }

        [JSProperty(Name = "length")]
        public int Length
        {
            get { return this.count; }
            set { }
        }

        public override string ToString()
        {
            return "[object IgnoreCollection]";
        }
    }
}
