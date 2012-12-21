using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Jurassic;
using Jurassic.Library;

namespace scripting.Objects
{
    class JSNodeCollection : ObjectInstance
    {
        private XmlNodeList Items { get; set; }
        private int count { get; set; }

        protected override string InternalClassName
        {
            get { return "NodeCollection"; }
        }

        internal JSNodeCollection(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSNodeCollection(ObjectInstance prototype, XmlNodeList list)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["NodeCollection"]).InstancePrototype)
        {
            this.Items = list;
            this.count = 0;

            this.PopulateFunctions();

            foreach (XmlNode n in list)
                if (!n.Name.StartsWith("#"))
                    this.SetPropertyValue((uint)this.count++,
                        new JSNode(this.Engine.Object.InstancePrototype, n), throwOnError: true);
        }

        [JSProperty(Name = "length")]
        public int Length
        {
            get { return this.count; }
            set { }
        }
    }
}
