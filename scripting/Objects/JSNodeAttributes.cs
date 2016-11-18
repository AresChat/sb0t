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
    class JSNodeAttributes : ObjectInstance
    {
        private XmlAttributeCollection Attribs { get; set; }
        private XmlNode Owner { get; set; }
        private int count { get; set; }

        public JSNodeAttributes(ObjectInstance prototype, XmlAttributeCollection attribs, XmlNode owner)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["NodeAttributes"]).InstancePrototype)
        {
            this.Attribs = attribs;
            this.Owner = owner;
            this.count = 0;

            this.PopulateFunctions();

            foreach (XmlAttribute a in attribs)
                this.SetPropertyValue((uint)this.count++, a.Name, throwOnError: true);
        }

        internal JSNodeAttributes(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "NodeAttributes"; }
        }

        [JSProperty(Name = "length")]
        public int Length
        {
            get { return this.count; }
            set { }
        }

        [JSFunction(Name = "getValue", IsWritable = false, IsEnumerable = true)]
        public String GetValue(object a)
        {
            if (a == null)
                return null;

            try
            {
                XmlAttribute att = this.Attribs[a.ToString()];

                if (att != null)
                    return att.InnerText;
            }
            catch { }

            return null;
        }

        [JSFunction(Name = "setValue", IsWritable = false, IsEnumerable = true)]
        public bool SetValue(object a, object b)
        {
            if (a == null || b == null)
                return false;

            try
            {
                String name = a.ToString();
                String text = b.ToString();

                if (this.Attribs[name] != null)
                    this.Attribs[name].InnerText = text;
                else
                {
                    XmlAttribute xa = this.Owner.OwnerDocument.CreateAttribute(name);
                    xa.InnerText = text;
                    this.Owner.Attributes.Append(xa);
                }

                for (int i = 0; i < this.count; i++)
                    this.SetPropertyValue((uint)i, null, throwOnError: true);

                this.count = 0;

                foreach (XmlAttribute att in this.Attribs)
                    this.SetPropertyValue((uint)this.count++, att.Name, throwOnError: true);

                return true;
            }
            catch { }

            return false;
        }

        [JSFunction(Name = "removeValue", IsWritable = false, IsEnumerable = true)]
        public bool RemoveValue(object a)
        {
            if (a == null)
                return false;

            try
            {
                if (this.Attribs[a.ToString()] != null)
                {
                    this.Attribs.Remove(this.Attribs[a.ToString()]);

                    for (int i = 0; i < this.count; i++)
                        this.SetPropertyValue((uint)i, null, throwOnError: true);

                    this.count = 0;

                    foreach (XmlAttribute att in this.Attribs)
                        this.SetPropertyValue((uint)this.count++, att.Name, throwOnError: true);

                    return true;
                }
            }
            catch { }

            return false;
        }
    }
}
