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
using System.IO;
using Jurassic;
using Jurassic.Library;

namespace scripting.Instances
{
    class JSXmlParserInstance : ObjectInstance
    {
        private XmlDocument DOC { get; set; }
        private bool _avail = false;

        public JSXmlParserInstance(ObjectInstance prototype)
            : base(prototype)
        {
            this.DOC = new XmlDocument();
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "XmlParser"; }
        }

        [JSProperty(Name = "available")]
        public bool Available
        {
            get { return this._avail; }
            set { }
        }

        [JSProperty(Name = "xml")]
        public String Xml
        {
            get
            {
                if (!this._avail)
                    return null;

                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(this.DOC.InnerXml);
                    String result = String.Empty;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
                        writer.Formatting = Formatting.Indented;
                        doc.Save(writer);
                        ms.Position = 0;

                        using (StreamReader reader = new StreamReader(ms, true))
                            result = reader.ReadToEnd();
                    }

                    return result;
                }
                catch { return null; }
            }
            set { }
        }

        [JSProperty(Name = "nodeName")]
        public String Name
        {
            get
            {
                if (!this._avail)
                    return null;

                try
                {
                    return this.DOC.Name;
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
                if (!this._avail)
                    return null;

                try
                {
                    return this.DOC.InnerText;
                }
                catch { return null; }
            }
            set { this.DOC.InnerText = value; }
        }

        [JSProperty(Name = "parentNode")]
        public Objects.JSNode ParentNode
        {
            get
            {
                if (!this._avail)
                    return null;

                try
                {
                    if (this.DOC.ParentNode != null)
                        return new Objects.JSNode(this.Engine.Object.InstancePrototype, this.DOC.ParentNode);
                }
                catch { }

                return null;
            }
            set { }
        }

        [JSProperty(Name = "childNodes")]
        public Objects.JSNodeCollection ChildNodes
        {
            get
            {
                if (!this._avail)
                    return null;

                try
                {
                    if (this.DOC.ChildNodes.Count > 0)
                        return new Objects.JSNodeCollection(this.Engine.Object.InstancePrototype, this.DOC.ChildNodes);
                }
                catch { }

                return null;
            }
            set { }
        }

        [JSProperty(Name = "attributes")]
        public Objects.JSNodeAttributes Attributes
        {
            get
            {
                if (!this._avail)
                    return null;

                try
                {
                    return new Objects.JSNodeAttributes(this.Engine.Object.InstancePrototype, this.DOC.Attributes, this.DOC);
                }
                catch { }

                return null;
            }
            set { }
        }

        [JSFunction(Name = "load", IsWritable = false, IsEnumerable = true)]
        public bool Load(object a)
        {
            if (a != null)
            {
                try
                {
                    this.DOC = new XmlDocument();
                    this.DOC.PreserveWhitespace = true;
                    this.DOC.LoadXml(a.ToString());
                    this._avail = true;
                    return true;
                }
                catch { this._avail = false; }
            }

            return false;
        }

        [JSFunction(Name = "create", IsWritable = false, IsEnumerable = true)]
        public bool Create(object a)
        {
            if (a == null)
                return false;

            try
            {
                this.DOC = new XmlDocument();
                this.DOC.PreserveWhitespace = true;
                this.DOC.AppendChild(this.DOC.CreateXmlDeclaration("1.0", null, null));
                this.DOC.AppendChild(this.DOC.CreateElement(a.ToString()));
                this._avail = true;
                return true;
            }
            catch { }

            this._avail = false;
            return false;
        }

        [JSFunction(Name = "getNodesByName", IsWritable = false, IsEnumerable = true)]
        public Objects.JSNodeCollection GetNodesByName(object a)
        {
            if (a != null)
                try
                {
                    XmlNodeList list = this.DOC.GetElementsByTagName(a.ToString());

                    if (list.Count > 0)
                        return new Objects.JSNodeCollection(this.Engine.Object.InstancePrototype, list);
                }
                catch { }

            return null;
        }

        [JSFunction(Name = "appendChild", IsWritable = false, IsEnumerable = true)]
        public Objects.JSNode AppendChild(object a)
        {
            if (a == null)
                return null;

            try
            {
                XmlNode x = this.DOC.CreateNode(XmlNodeType.Element, a.ToString(), this.DOC.DocumentElement.BaseURI);
                this.DOC.DocumentElement.AppendChild(x);
                return new Objects.JSNode(this.Engine.Object.InstancePrototype, x);
            }
            catch { }

            return null;
        }

        [JSFunction(Name = "removeChild", IsWritable = false, IsEnumerable = true)]
        public bool RemoveChild(object a)
        {
            if (a is Objects.JSNode)
            {
                try
                {
                    Objects.JSNode n = (Objects.JSNode)a;
                    this.DOC.DocumentElement.RemoveChild(n.Item);
                    return true;
                }
                catch { }
            }

            return false;
        }
    }
}
