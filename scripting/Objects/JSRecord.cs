using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;
using iconnect;

namespace scripting.Objects
{
    class JSRecord : ObjectInstance
    {
        private String ScriptName { get; set; }

        public JSRecord(ObjectInstance prototype, IRecord user, String script)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["Record"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.parent = user;
            this.ScriptName = script;
        }

        protected override string InternalClassName
        {
            get { return "Record"; }
        }

        internal JSRecord(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        internal IRecord parent;

        [JSProperty(Name = "joinTime")]
        public double JoinTime
        {
            get { return this.parent.JoinTime; }
            set { }
        }

        [JSProperty(Name = "name")]
        public String Name
        {
            get { return this.parent.Name; }
            set { }
        }

        [JSProperty(Name = "externalIp")]
        public String ExternalIP
        {
            get { return this.parent.ExternalIP.ToString(); }
            set { }
        }

        [JSProperty(Name = "localIp")]
        public String LocalIP
        {
            get { return this.parent.LocalIP.ToString(); }
            set { }
        }

        [JSProperty(Name = "version")]
        public String Version
        {
            get { return this.parent.Version; }
            set { }
        }

        [JSProperty(Name = "port")]
        public int Port
        {
            get { return this.parent.DataPort; }
            set { }
        }

        [JSProperty(Name = "guid")]
        public String Guid
        {
            get { return this.parent.Guid.ToString(); }
            set { }
        }

        [JSProperty(Name = "dns")]
        public String DNS
        {
            get { return this.parent.DNS; }
            set { }
        }

        [JSFunction(Name = "ban", IsWritable = false, IsEnumerable = true)]
        public void Ban()
        {
            this.parent.Ban();
        }
    }
}
