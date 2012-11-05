using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;
using iconnect;

namespace scripting.Statics
{
    [JSEmbed(Name = "Channels")]
    class JSChannels : ObjectInstance
    {
        public JSChannels(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        [JSProperty(Name = "available")]
        public static bool Available
        {
            get { return Server.Channels.Available; }
            set { }
        }

        [JSProperty(Name = "enabled")]
        public static bool Enabled
        {
            get { return Server.Channels.Enabled; }
            set { }
        }

        [JSFunction(Name = "search", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static Objects.JSChannelCollection Search(ScriptEngine eng, Object a)
        {
            List<Objects.JSChannel> results = new List<Objects.JSChannel>();

            if (!(a is Undefined))
            {
                String str = a.ToString().ToUpper();
                List<IChannelItem> matches = new List<IChannelItem>();

                Server.Channels.ForEach(x =>
                {
                    if (x.Name.ToUpper().Contains(str))
                        if (x.Users < 200)
                            matches.Add(x);
                });

                matches.Sort((y, x) => x.Users.CompareTo(y.Users));

                if (matches.Count > 10)
                    matches = matches.GetRange(0, 10);

                foreach (IChannelItem m in matches)
                    results.Add(new Objects.JSChannel(eng.Object.InstancePrototype, m));
            }

            return new Objects.JSChannelCollection(eng.Object.InstancePrototype, results.ToArray(), eng.ScriptName);
        }
    }
}
