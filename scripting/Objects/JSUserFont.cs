using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;
using iconnect;

namespace scripting.Objects
{
    class JSUserFont : ObjectInstance
    {
        internal IUser parent;

        public JSUserFont(ObjectInstance prototype, IUser user, String script)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["UserFont"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.parent = user;
        }

        internal JSUserFont(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "UserFont"; }
        }

        [JSProperty(Name = "enabled")]
        public bool Enabled
        {
            get { return this.parent.Font.Enabled; }
            set { this.parent.Font.Enabled = value; }
        }

        [JSProperty(Name = "nameColor")]
        public String NameColor
        {
            get { return this.parent.Font.NameColor; }
            set { this.parent.Font.NameColor = value; }
        }

        [JSProperty(Name = "textColor")]
        public String TextColor 
        {
            get { return this.parent.Font.TextColor; }
            set { this.parent.Font.TextColor = value; }
        }

        [JSProperty(Name = "size")]
        public int Size
        {
            get { return this.parent.Font.Size; }
            set
            {
                if (value >= 8 && value <= 18)
                    this.parent.Font.Size = value;
            }
        }

        [JSProperty(Name = "family")]
        public String Name
        {
            get { return this.parent.Font.FontName; }
            set { this.parent.Font.FontName = value; }
        }
    }
}
