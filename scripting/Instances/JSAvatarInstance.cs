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
using System.Net;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using Jurassic;
using Jurassic.Library;

namespace scripting.Instances
{
    class JSAvatarInstance : ObjectInstance
    {
        private bool busy = false;

        public JSAvatarInstance(ObjectInstance prototype)
            : base(prototype)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Avatar"; }
        }

        [JSProperty(Name = "src")]
        public String Source { get; set; }

        [JSProperty(Name = "oncomplete")]
        public UserDefinedFunction Callback { get; set; }

        [JSFunction(Name = "download", IsWritable = false, IsEnumerable = true)]
        public bool Download(object a)
        {
            if (this.busy)
                return false;

            Thread thread = new Thread(new ThreadStart(() =>
            {
                this.busy = true;
                String arg = String.Empty;

                if (!(a is Undefined) && a != null)
                    arg = a.ToString();

                Objects.JSAvatarImage result = new Objects.JSAvatarImage(this.Engine.Object.InstancePrototype)
                {
                    Callback = this.Callback,
                    Data = null,
                    ScriptName = this.Engine.ScriptName,
                    Arg = arg
                };

                try
                {
                    WebRequest request = WebRequest.Create(this.Source);
                    List<byte> bytes_in = new List<byte>();

                    using (WebResponse response = request.GetResponse())
                    {
                        int received = 0;
                        byte[] buf = new byte[1024];

                        using (Stream stream = response.GetResponseStream())
                            while ((received = stream.Read(buf, 0, 1024)) > 0)
                                bytes_in.AddRange(buf.Take(received));
                    }

                    using (Bitmap avatar_raw = new Bitmap(new MemoryStream(bytes_in.ToArray())))
                    {
                        using (Bitmap avatar_sized = new Bitmap(48, 48))
                        {
                            using (Graphics g = Graphics.FromImage(avatar_sized))
                            {
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                g.DrawImage(avatar_raw, new RectangleF(0, 0, 48, 48));

                                using (MemoryStream ms = new MemoryStream())
                                {
                                    avatar_sized.Save(ms, ImageFormat.Jpeg);
                                    result.Data = ms.ToArray();
                                    bytes_in.Clear();
                                }
                            }
                        }
                    }
                }
                catch { }

                ScriptManager.Callbacks.Enqueue(result);
                this.busy = false;
            }));

            thread.Start();

            return true;
        }

        private String[] bad_chars = new String[]
        {
            "/",
            "\\",
            " ",
        };

        [JSFunction(Name = "load", IsWritable = false, IsEnumerable = true)]
        public Objects.JSAvatarImage Load(object a)
        {
            Objects.JSAvatarImage av = new Objects.JSAvatarImage(this.Engine.Object.InstancePrototype);

            if (a is String || a is ConcatenatedString)
            {
                String filename = a.ToString();

                if (filename.Length > 1)
                    if (bad_chars.Count<String>(x => filename.Contains(x)) == 0)
                    {
                        String path = Path.Combine(Server.DataPath, this.Engine.ScriptName, "data", filename);

                        try
                        {
                            if (File.Exists(path))
                            {
                                byte[] data = File.ReadAllBytes(path);

                                using (Bitmap avatar_raw = new Bitmap(new MemoryStream(data)))
                                {
                                    using (Bitmap avatar_sized = new Bitmap(48, 48))
                                    {
                                        using (Graphics g = Graphics.FromImage(avatar_sized))
                                        {
                                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                            g.DrawImage(avatar_raw, new RectangleF(0, 0, 48, 48));

                                            using (MemoryStream ms = new MemoryStream())
                                            {
                                                avatar_sized.Save(ms, ImageFormat.Jpeg);
                                                av.Data = ms.ToArray();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
            }

            return av;
        }
    }
}
