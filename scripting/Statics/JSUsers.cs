using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Statics
{
    [JSEmbed(Name = "Users")]
    class JSUsers : ObjectInstance
    {
        public JSUsers(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Users"; }
        }

        [JSFunction(Name = "local", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static void Local(ScriptEngine eng, object f)
        {
            if (f is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    UserDefinedFunction function = (UserDefinedFunction)f;

                    try
                    {
                        script.local_users.ForEach(u => function.Call(eng.Global, u));
                    }
                    catch { }
                }
            }
        }

        [JSFunction(Name = "linked", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static void Linked(ScriptEngine eng, object f)
        {
            if (f is UserDefinedFunction)
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                    if (Server.Link.IsLinked)
                    {
                        UserDefinedFunction function = (UserDefinedFunction)f;

                        try
                        {
                            script.leaves.ForEach(l => l.users.ForEach(u => function.Call(eng.Global, u)));
                        }
                        catch { }
                    }
            }
        }

        [JSFunction(Name = "banned", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static void Banned(ScriptEngine eng, object f)
        {
            if (f is UserDefinedFunction)
            {
                UserDefinedFunction function = (UserDefinedFunction)f;

                try
                {
                    Server.Users.Banned(x => function.Call(eng.Global,
                        new Objects.JSBannedUser(eng.Object.InstancePrototype, x, eng.ScriptName)));
                }
                catch { }
            }
        }

        [JSFunction(Name = "records", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static void Records(ScriptEngine eng, object f)
        {
            if (f is UserDefinedFunction)
            {
                UserDefinedFunction function = (UserDefinedFunction)f;

                try
                {
                    Server.Users.Records(x => function.Call(eng.Global,
                        new Objects.JSRecord(eng.Object.InstancePrototype, x, eng.ScriptName)));
                }
                catch { }
            }
        }

    }
}
