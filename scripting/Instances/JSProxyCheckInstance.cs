/*
    sb0t ares chat server
    Copyright (C) 2017  AresChat

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jurassic.Library;
using Jurassic;
using System.Threading;
using System.Net.Http;

namespace scripting.Instances
{
    class JSProxyCheckInstance : ObjectInstance
    {
        private string apiKey = "";

        private static readonly string proxyCheckUrl = "proxycheck.io/v1/";

        public JSProxyCheckInstance(ObjectInstance prototype)
            : base(prototype)
        {
            this.PopulateFunctions();

            DefineProperty(Engine.Symbol.ToStringTag, new PropertyDescriptor("ProxyCheck", PropertyAttributes.Sealed), true);
        }

        public JSProxyCheckInstance(ObjectInstance prototype, string apiKey) 
            : base(prototype)
        {
            this.PopulateFunctions();

            this.apiKey = apiKey;

            DefineProperty(Engine.Symbol.ToStringTag, new PropertyDescriptor("ProxyCheck", PropertyAttributes.Sealed), true);
        }

        [JSProperty(Name = "includeVPN", IsEnumerable = true)]
        public bool IncludeVPN { get; set; } = true;

        [JSProperty(Name = "useTLS", IsEnumerable = true)]
        public bool UseTLS { get; set; }

        [JSFunction(Name ="query", Flags = JSFunctionFlags.None, IsEnumerable = true)]
        public bool Query(Objects.JSUser user, UserDefinedFunction jsDelegate)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                var client = new HttpClient();

                var pairs = new Dictionary<string, string>
                {
                    { "tag",  $"({Server.Chatroom.Name}) - ({user.Name}) - ({user.Guid}):"}
                };

                var content = new FormUrlEncodedContent(pairs);

                StringBuilder url = new StringBuilder();
                url.Append(UseTLS ? "https://" : "http://");
                url.Append(proxyCheckUrl);
                url.Append(user.parent.OriginalIP); // this is accessing an internal member
                url.Append(string.IsNullOrWhiteSpace(apiKey) ? "" : $"&key={apiKey}");
                url.Append(IncludeVPN ? "&vpn=1" : "");

                try
                {


                    var response = client.PostAsync(url.ToString(), content).Result;

                    string jsonResponse = response.Content.ReadAsStringAsync().Result;

                    Objects.JSProxyCheckResult result = new Objects.JSProxyCheckResult(this.Engine.Object.InstancePrototype,
                        jsonResponse)
                    {
                        Callback = jsDelegate,
                        ScriptName = this.Engine.UserData as string,
                        User = user
                    };

                    ScriptManager.Callbacks.Enqueue(result);
                }
                catch (Exception e)
                {

                }
            }));

            thread.Start();

            return true;
        }
    }
}
