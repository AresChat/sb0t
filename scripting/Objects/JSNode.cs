using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Jurassic;
using Jurassic.Library;

namespace scripting.Objects
{
    class JSNode : ObjectInstance
    {
        internal XmlNode Item { get; set; }

        public JSNode(ObjectInstance prototype, XmlNode node)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["Node"]).InstancePrototype)
        {
            this.Item = node;
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Node"; }
        }

        internal JSNode(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        [JSProperty(Name = "nodeName")]
        public String Name
        {
            get
            {
                try
                {
                    return this.Item.Name;
                }
                catch { return null; }
            }
            set { }
        }

        [JSProperty(Name = "nodeValue")]
        public String Value
        {
            get
            {
                try
                {
                    return this.Item.InnerText;
                }
                catch { return null; }
            }
            set { this.Item.InnerText = value; }
        }

        [JSProperty(Name = "parentNode")]
        public JSNode ParentNode
        {
            get
            {
                try
                {
                    if (this.Item.ParentNode != null)
                        return new JSNode(this.Engine.Object.InstancePrototype, this.Item.ParentNode);
                }
                catch { }

                return null;
            }
            set { }
        }

        [JSProperty(Name = "childNodes")]
        public JSNodeCollection ChildNodes
        {
            get
            {
                try
                {
                    if (this.Item.ChildNodes.Count > 0)
                        return new JSNodeCollection(this.Engine.Object.InstancePrototype, this.Item.ChildNodes);
                }
                catch { }

                return null;
            }
            set { }
        }

        [JSProperty(Name = "attributes")]
        public JSNodeAttributes Attributes
        {
            get
            {
                try
                {
                    return new JSNodeAttributes(this.Engine.Object.InstancePrototype, this.Item.Attributes, this.Item);
                }
                catch { }

                return null;
            }
            set { }
        }

        [JSFunction(Name = "getNodesByName", IsWritable = false, IsEnumerable = true)]
        public JSNodeCollection GetNodesByName(object a)
        {
            if (a != null)
                try
                {
                    XmlElement e = (XmlElement)this.Item;
                    XmlNodeList list = e.GetElementsByTagName(a.ToString());
                    
                    if (list.Count > 0)
                        return new JSNodeCollection(this.Engine.Object.InstancePrototype, list);
                }
                catch { }

            return null;
        }

        [JSFunction(Name = "appendChild", IsWritable = false, IsEnumerable = true)]
        public JSNode AppendChild(object a)
        {
            if (a == null)
                return null;

            try
            {
                XmlNode x = this.Item.OwnerDocument.CreateNode(XmlNodeType.Element, a.ToString(), this.Item.BaseURI);
                this.Item.AppendChild(x);
                return new JSNode(this.Engine.Object.InstancePrototype, x);
            }
            catch { }

            return null;
        }

        [JSFunction(Name = "removeChild", IsWritable = false, IsEnumerable = true)]
        public bool RemoveChild(object a)
        {
            if (a is JSNode)
            {
                try
                {
                    JSNode n = (JSNode)a;
                    this.Item.RemoveChild(n.Item);
                    return true;
                }
                catch { }
            }

            return false;
        }
    }
}
