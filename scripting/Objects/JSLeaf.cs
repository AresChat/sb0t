using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;
using iconnect;

namespace scripting.Objects
{
    class JSLeaf : ObjectInstance
    {
        private String ScriptName { get; set; }
        internal List<Objects.JSUser> users = new List<Objects.JSUser>();

        public JSLeaf(ObjectInstance prototype, ILeaf leaf, String script)
            : base(prototype)
        {
            this.PopulateFunctions();
            this.parent = leaf;
            this.ScriptName = script;
        }

        internal ILeaf parent;

        public uint Ident
        {
            get { return this.parent.Ident; }
        }

        [JSProperty(Name = "externalIp")]
        public String ExternalIP
        {
            get { return this.parent.ExternalIP.ToString(); }
        }

        [JSProperty(Name = "port")]
        public int Port
        {
            get { return this.parent.Port; }
        }

        [JSProperty(Name = "name")]
        public String Name
        {
            get { return this.parent.Name; }
        }

        [JSFunction(Name = "print", IsWritable = false)]
        public void Print(object a, object b)
        {
            if (a != null && b == null)
            {
                String str = a.ToString();
                this.parent.Print(str);
            }
            else if (a != null && b != null)
            {
                ushort u;

                if (ushort.TryParse(a.ToString(), out u))
                    this.parent.Print(u, b.ToString());
            }
        }

        [JSFunction(Name = "printAdmins", IsWritable = false)]
        public void PrintAdmins(object a, object b)
        {
            if (a != null && b != null)
            {
                byte z;

                if (byte.TryParse(a.ToString(), out z))
                    this.parent.Print((ILevel)z, b.ToString());
            }
        }

        [JSFunction(Name = "users", IsWritable = false)]
        public void Users(object a)
        {
            if (a is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == this.ScriptName);

                if (script != null)
                {
                    UserDefinedFunction x = (UserDefinedFunction)a;

                    try
                    {
                        this.users.ForEach(z => x.Call(script.JS.Global, z));
                    }
                    catch { }
                }
            }
        }

        [JSFunction(Name = "user", IsWritable = false)]
        public JSUser FindUser(object a)
        {
            JSUser result = null;

            if (a != null)
            {
                String str = a.ToString();

                if (str.Length > 0)
                {
                    result = this.users.Find(x => x.Name == str);

                    if (result == null)
                        result = this.users.Find(x => x.Name.StartsWith(str));
                }
            }

            return result;
        }

        public override string ToString()
        {
            return "[object Leaf]";
        }
    }
}
