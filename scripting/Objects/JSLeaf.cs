using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;
using iconnect;

namespace scripting.Objects
{
    class JSLeaf : ObjectInstance
    {
        private String ScriptName { get; set; }

        public JSLeaf(ObjectInstance prototype, ILeaf leaf, String script)
            : base(prototype)
        {
            this.PopulateFunctions();
            this.parent = leaf;
            this.ScriptName = script;
        }

        internal ILeaf parent;

        public uint Ident
        {
            get { return this.parent.Ident; }
        }


    }
}
