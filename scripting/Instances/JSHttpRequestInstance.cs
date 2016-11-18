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
using System.Net;
using System.IO;
using System.Threading;
using Jurassic;
using Jurassic.Library;

namespace scripting.Instances
{
    class JSHttpRequestInstance : ObjectInstance
    {
        private bool busy = false;
        private String _method = "get";
        private WebHeaderCollection _headers = new WebHeaderCollection();

        public JSHttpRequestInstance(ObjectInstance prototype)
            : base(prototype)
        {
            this.PopulateFunctions();
            this.Agent = String.Empty;
            
        }

        protected override string InternalClassName
        {
            get { return "HttpRequest"; }
        }

        [JSProperty(Name = "method")]
        public String Method
        {
            get { return this._method; }
            set
            {
                String str = value.ToUpper();

                if (str == "POST")
                    this._method = "post";
                else if (str == "GET")
                    this._method = "get";
            }
        }

        [JSProperty(Name = "accept")]
        public String Accept { get; set; }

        [JSProperty(Name = "params")]
        public String Params { get; set; }

        [JSProperty(Name = "src")]
        public String Source { get; set; }

        [JSProperty(Name = "userAgent")]
        public String Agent { get; set; }

        [JSProperty(Name = "oncomplete")]
        public UserDefinedFunction Callback { get; set; }

        [JSProperty(Name = "utf")]
        public bool UTF { get; set; }

        [JSProperty(Name = "host")]
        public String Host { get; set; }

        [JSFunction(Name = "header", IsWritable = false, IsEnumerable = true)]
        public bool Header(object key, object value)
        {
            if (key is Undefined || value is Undefined)
                return false;

            String k = key.ToString();
            String v = value.ToString();

            this._headers[k] = v;
            return true;
        }

        [JSFunction(Name = "download", IsWritable = false, IsEnumerable = true)]
        public bool Download(object a)
        {
            if (this.busy)
                return false;

            Thread thread = new Thread(new ThreadStart(() =>
            {
                this.busy = true;
                bool use_utf = this.UTF;
                String arg = String.Empty;

                if (!(a is Undefined) && a != null)
                    arg = a.ToString();

                Objects.JSHttpRequestResult result = new Objects.JSHttpRequestResult(this.Engine.Object.InstancePrototype)
                {
                    Callback = this.Callback,
                    ScriptName = this.Engine.ScriptName,
                    Arg = arg
                };

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.Source);
                    request.UserAgent = this.Agent;
                    request.Method = this._method;

                    if (!String.IsNullOrEmpty(this.Host))
                        request.Host = this.Host;

                    if (!String.IsNullOrEmpty(this.Accept))
                        request.Accept = this.Accept;

                    for (int i = 0; i < this._headers.Count; i++)
                        request.Headers.Add(this._headers.Keys[i], this._headers[i]);

                    if (!String.IsNullOrEmpty(this.Params) && this._method == "post")
                    {
                        byte[] p = this.UTF ? Encoding.UTF8.GetBytes(this.Params) : Encoding.Default.GetBytes(this.Params);
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.ContentLength = p.Length;

                        using (Stream stream = request.GetRequestStream())
                            stream.Write(p, 0, p.Length);
                    }

                    WebResponse response = request.GetResponse();

                    List<byte> bytes_in = new List<byte>();
                    byte[] buf = new byte[1024];
                    int rec = 0;

                    using (Stream stream = response.GetResponseStream())
                        while ((rec = stream.Read(buf, 0, 1024)) > 0)
                            bytes_in.AddRange(buf.Take(rec));

                    response.Close();
                    result.Data = use_utf ? Encoding.UTF8.GetString(bytes_in.ToArray()) : Encoding.Default.GetString(bytes_in.ToArray());
                }
                catch { }

                ScriptManager.Callbacks.Enqueue(result);
                this.busy = false;
            }));

            thread.Start();

            return true;
        }
    }
}
