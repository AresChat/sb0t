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
