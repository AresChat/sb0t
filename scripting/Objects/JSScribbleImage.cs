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
using System.IO;
using Jurassic;
using Jurassic.Library;
using iconnect;

namespace scripting.Objects
{
    class JSScribbleImage : ObjectInstance, ICallback
    {
        public JSScribbleImage(ObjectInstance prototype)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["ScribbleImage"]).InstancePrototype)
        {
            this.PopulateFunctions();
        }

        internal JSScribbleImage(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public byte[] Data { get; set; }
        public UserDefinedFunction Callback { get; set; }
        public String ScriptName { get; set; }
        public String Arg { get; set; }
        public int Height { get; set; }
        public String URL { get; set; }

        protected override string InternalClassName
        {
            get { return "ScribbleImage"; }
        }

        [JSProperty(Name = "arg")]
        public String GetArgument
        {
            get { return this.Arg; }
            set { }
        }

        [JSProperty(Name = "exists")]
        public bool DoesExist
        {
            get { return this.Data != null; }
            set { }
        }

        [JSFunction(Name = "toAvatar", IsWritable = false, IsEnumerable = true)]
        public JSAvatarImage ToAvatar()
        {
            if (this.Data != null)
            {
                JSAvatarImage a = new JSAvatarImage(this.Engine.Object.InstancePrototype);
                a.FromScribble(Server.Compression.Decompress(this.Data));
                return a;
            }

            return null;
        }

        private String[] bad_chars = new String[]
        {
            ".",
            "/",
            "\\",
            " ",
        };

        [JSFunction(Name = "save", IsWritable = false, IsEnumerable = true)]
        public bool Save(object a)
        {
            if (this.Data == null)
                return false;

            if (a is String || a is ConcatenatedString)
            {
                String filename = a.ToString();

                if (filename.Length > 1)
                    if (bad_chars.Count<String>(x => filename.Contains(x)) == 0)
                    {
                        String path = Path.Combine(Server.DataPath, this.Engine.ScriptName, "data");

                        try
                        {
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);

                            path = Path.Combine(path, filename);
                            File.WriteAllBytes(path, Server.Compression.Decompress(this.Data));
                            return true;
                        }
                        catch { }
                    }
            }

            return false;
        }

        public void FromAvatar(byte[] data)
        {
            this.Data = Server.Compression.Compress(data);
        }

        public void SendScribble(String sender, IUser target)
        {
            if (this.Data == null)
                return;
            
            target.Scribble(sender == null ? Server.Chatroom.BotName : sender, this.Data, this.Height);
        }

        public void SendScribble(String sender, ILeaf leaf)
        {
            if (this.Data == null)
                return;

            leaf.Scribble(sender == null ? Server.Chatroom.BotName : sender, this.Data, this.Height);
        }
    }
}
