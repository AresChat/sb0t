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
