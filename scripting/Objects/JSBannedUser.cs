using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;
using iconnect;

namespace scripting.Objects
{
    class JSBannedUser : ObjectInstance
    {
        private String ScriptName { get; set; }

        public JSBannedUser(ObjectInstance prototype, IBan user, String script)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["BannedUser"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.parent = user;
            this.ScriptName = script;
        }

        internal JSBannedUser(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "BannedUser"; }
        }

        internal IBan parent;

        [JSProperty(Name = "name")]
        public String Name
        {
            get { return this.parent.Name; }
            set { }
        }

        [JSProperty(Name = "version")]
        public String Version
        {
            get { return this.parent.Version; }
            set { }
        }

        [JSProperty(Name = "guid")]
        public String Guid
        {
            get { return this.parent.Guid.ToString(); }
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

        [JSProperty(Name = "port")]
        public int Port
        {
            get { return this.parent.Port; }
            set { }
        }

        [JSFunction(Name = "unban", IsWritable = false, IsEnumerable = true)]
        public void Unban()
        {
            this.parent.Unban();
        }
    }
}
