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
    [JSEmbed(Name = "Link")]
    class JSLink : ObjectInstance
    {
        public JSLink(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Link"; }
        }

        [JSFunction(Name = "leaves", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static void Leaves(ScriptEngine eng, object f)
        {
            if (Server.Link.IsLinked)
                if (f is UserDefinedFunction)
                {
                    JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                    if (script != null)
                    {
                        UserDefinedFunction function = (UserDefinedFunction)f;
                        
                        try
                        {
                            script.leaves.ForEach(l => function.Call(eng.Global, l));
                        }
                        catch { }
                    }
                }
        }

        [JSFunction(Name = "leaf", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static Objects.JSLeaf Leaf(ScriptEngine eng, object a)
        {
            Objects.JSLeaf result = null;

            if (Server.Link.IsLinked && !(a is Undefined))
            {
                JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

                if (script != null)
                {
                    String str = a.ToString();
                    result = script.leaves.Find(x => x.Name == str);

                    if (result == null)
                        result = script.leaves.Find(x => x.Name.StartsWith(str));
                }
            }

            return result;
        }

        [JSFunction(Name = "connect", IsWritable = false, IsEnumerable = true)]
        public static void Connect(object a)
        {
            if (!(a is Undefined))
                Server.Link.Connect(a.ToString());
        }

        [JSFunction(Name = "disconnect", IsWritable = false, IsEnumerable = true)]
        public static void Disconnect()
        {
            Server.Link.Disconnect();
        }

        [JSProperty(Name = "linked")]
        public bool Linked
        {
            get { return Server.Link.IsLinked; }
        }

        [JSProperty(Name = "name")]
        public String Name
        {
            get
            {
                if (!Server.Link.IsLinked)
                    return null;

                return Server.Link.Name;
            }
        }

        [JSProperty(Name = "externalIp")]
        public String ExternalIP
        {
            get
            {
                if (!Server.Link.IsLinked)
                    return null;

                return Server.Link.ExternalIP.ToString();
            }
        }

        [JSProperty(Name = "port")]
        public int Port
        {
            get
            {
                if (!Server.Link.IsLinked)
                    return -1;

                return Server.Link.Port;
            }
        }

        [JSProperty(Name = "hashlink")]
        public String Hashlink
        {
            get
            {
                if (!Server.Link.IsLinked)
                    return null;

                Hashlink obj = new Hashlink
                {
                    IP = Server.Link.ExternalIP,
                    Name = Server.Link.Name,
                    Port = Server.Link.Port
                };

                return "arlnk://" + Server.Hashlinks.Encrypt(obj);
            }
        }
    }
}
