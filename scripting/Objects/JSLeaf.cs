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

        [JSProperty(Name = "hashlink")]
        public String Hashlink
        {
            get
            {
                Hashlink obj = new Hashlink
                {
                    IP = this.parent.ExternalIP,
                    Name = this.parent.Name,
                    Port = this.parent.Port
                };

                return "arlnk://" + Server.Hashlinks.Encrypt(obj);
            }
        }

        [JSFunction(Name = "print", IsWritable = false, IsEnumerable = true)]
        public void Print(object a, object b)
        {
            if (!(a is Undefined) && b is Undefined)
            {
                String str = a.ToString();
                Server.Print("test");
                this.parent.Print(str);
            }
            else if (!(a is Undefined) && !(b is Undefined))
            {
                ushort u;

                if (ushort.TryParse(a.ToString(), out u))
                    this.parent.Print(u, b.ToString());
            }
        }

        [JSFunction(Name = "printAdmins", IsWritable = false, IsEnumerable = true)]
        public void PrintAdmins(object a, object b)
        {
            if (!(a is Undefined) && !(b is Undefined))
            {
                byte z;

                if (byte.TryParse(a.ToString(), out z))
                    this.parent.Print((ILevel)z, b.ToString());
            }
        }

        [JSFunction(Name = "users", IsWritable = false, IsEnumerable = true)]
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

        [JSFunction(Name = "user", IsWritable = false, IsEnumerable = true)]
        public JSUser FindUser(object a)
        {
            JSUser result = null;

            if (!(a is Undefined))
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
