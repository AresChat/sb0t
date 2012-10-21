using System;
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
            : base(prototype)
        {
            this.Attribs = attribs;
            this.Owner = owner;
            this.count = 0;

            this.PopulateFunctions();

            foreach (XmlAttribute a in attribs)
                this.SetPropertyValue((uint)this.count++, a.Name, throwOnError: true);
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

        public override string ToString()
        {
            return "[object NodeAttributes]";
        }
    }
}
