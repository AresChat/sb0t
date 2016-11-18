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
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Jurassic;
using Jurassic.Library;

namespace scripting.Objects
{
    class JSAvatarImage : ObjectInstance, ICallback
    {
        internal JSAvatarImage(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSAvatarImage(ObjectInstance prototype)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["AvatarImage"]).InstancePrototype)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "AvatarImage"; }
        }

        public byte[] Data { get; set; }
        public UserDefinedFunction Callback { get; set; }
        public String ScriptName { get; set; }
        public String Arg { get; set; }

        [JSProperty(Name = "arg")]
        public String GetArgument
        {
            get { return this.Arg; }
            set { }
        }

        [JSProperty(Name = "exists")]
        public bool DoesExist
        {
            get
            {
                if (this.Data == null)
                    return false;

                if (this.Data.Length == 0)
                    return false;

                return true;
            }
            set { }
        }

        [JSFunction(Name = "toScribble", IsWritable = false, IsEnumerable = true)]
        public JSScribbleImage ToScribble()
        {
            if (this.Data != null)
            {
                JSScribbleImage s = new JSScribbleImage(this.Engine.Object.InstancePrototype);
                s.FromAvatar(this.Data);
                return s;
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

                            path = Path.Combine(path, filename + ".jpg");
                            
                            File.WriteAllBytes(path, this.Data);
                            return true;
                        }
                        catch { }
                    }
            }

            return false;
        }

        public void FromScribble(byte[] data)
        {
            try
            {
                using (Bitmap avatar_raw = new Bitmap(new MemoryStream(data)))
                using (Bitmap avatar_sized = new Bitmap(48, 48))
                using (Graphics g = Graphics.FromImage(avatar_sized))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(avatar_raw, new RectangleF(0, 0, 48, 48));

                    using (MemoryStream ms = new MemoryStream())
                    {
                        avatar_sized.Save(ms, ImageFormat.Jpeg);
                        this.Data = ms.ToArray();
                    }
                }
            }
            catch { }
        }
    }
}
