/*
    sb0t ares chat server
    Copyright (C) 2016  AresChat

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System;
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
