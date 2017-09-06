using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jurassic;
using Jurassic.Library;
using Newtonsoft.Json.Linq;

namespace scripting.Objects
{
    class JSProxyCheckResult : ObjectInstance, ICallback
    {
        public string ScriptName { get; set; }
        public UserDefinedFunction Callback { get; set; }
        
        public Objects.JSUser User { get; set; }

        [JSProperty(Name = "error", IsEnumerable = true, IsConfigurable = false)]
        public string Error { get; internal set; }

        [JSProperty(Name = "proxy", IsEnumerable = true, IsConfigurable = false)]
        public bool Proxy { get; internal set; }

        [JSProperty(Name = "type", IsEnumerable = true, IsConfigurable = false)]
        public string Type { get; internal set; }

        [JSProperty(Name = "provider", IsEnumerable = true, IsConfigurable = false)]
        public string Provider { get; internal set; }

        public JSProxyCheckResult(ObjectInstance prototype, string json)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["ProxyCheckResult"]).InstancePrototype)
        {
            this.PopulateFunctions();

            dynamic proxy = JObject.Parse(json);

            Error = proxy.error;

            Proxy = proxy.proxy == "yes";

            Type = proxy.type;

            Provider = proxy.provider;
        }

        internal JSProxyCheckResult(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();

            DefineProperty(Engine.Symbol.ToStringTag, new PropertyDescriptor("ProxyCheckResult", PropertyAttributes.Sealed), true);
        }
    }
}
