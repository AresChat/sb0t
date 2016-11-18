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
using iconnect;

namespace scripting.Objects
{
    class JSLeaf : ObjectInstance
    {
        private String ScriptName { get; set; }
        internal List<Objects.JSUser> users = new List<Objects.JSUser>();

        public JSLeaf(ObjectInstance prototype, ILeaf leaf, String script)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["Leaf"]).InstancePrototype)
        {
            this.PopulateFunctions();
            this.parent = leaf;
            this.ScriptName = script;
        }

        protected override string InternalClassName
        {
            get { return "Leaf"; }
        }

        internal JSLeaf(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
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
            set { }
        }

        [JSProperty(Name = "port")]
        public int Port
        {
            get { return this.parent.Port; }
            set { }
        }

        [JSProperty(Name = "name")]
        public String Name
        {
            get { return this.parent.Name; }
            set { }
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
            set { }
        }

        [JSFunction(Name = "print", IsWritable = false, IsEnumerable = true)]
        public void Print(object a, object b)
        {
            if (!(a is Undefined) && b is Undefined)
            {
                String str = a.ToString();
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
                {
                    if (z < 1 || z > 3)
                        z = 1;

                    this.parent.Print((ILevel)z, b.ToString());
                }
            }
            else if (!(a is Undefined))
                this.parent.Print(ILevel.Moderator, a.ToString());
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

        [JSFunction(Name = "sendText", IsWritable = false, IsEnumerable = true)]
        public void SendText(object a, object b)
        {
            if (!(a is Undefined))
                if (!(b is Undefined))
                {
                    String sender = a.ToString();
                    String text = b.ToString();
                    this.parent.SendText(sender, text);
                }
        }

        [JSFunction(Name = "sendEmote", IsWritable = false, IsEnumerable = true)]
        public void SendEmote(object a, object b)
        {
            if (!(a is Undefined))
                if (!(b is Undefined))
                {
                    String sender = a.ToString();
                    String text = b.ToString();
                    this.parent.SendEmote(sender, text);
                }
        }

        [JSFunction(Name = "scribble", IsWritable = false, IsEnumerable = true)]
        public void Scribble(object a, object b)
        {
            if (a is JSScribbleImage)
            {
                JSScribbleImage scr = (JSScribbleImage)a;
                scr.SendScribble(Server.Chatroom.BotName, this.parent);
            }
            else if (!(a is Undefined) && b is JSScribbleImage)
            {
                JSScribbleImage scr = (JSScribbleImage)b;
                scr.SendScribble(a.ToString(), this.parent);
            }
        }
    }
}
