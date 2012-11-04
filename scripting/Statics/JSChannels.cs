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

        private static ulong last_call = 0;

        [JSFunction(Name = "search", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static Objects.JSChannelCollection Search(ScriptEngine eng, Object a)
        {
            List<Objects.JSChannel> results = new List<Objects.JSChannel>();
            ulong time = Server.Ticks;

            if ((last_call + 1000) < time)
            {
                last_call = time;

                if (Server.Channels.Enabled)
                    if (Server.Channels.Available)
                        if (a is UserDefinedFunction)
                        {
                            UserDefinedFunction f = (UserDefinedFunction)a;

                            Server.Channels.ForEach(x =>
                            {
                                Objects.JSChannel channel = new Objects.JSChannel(eng.Object.InstancePrototype, x);
                                object obj = f.Call(eng.Global, channel);

                                if (obj != null)
                                    if (obj is bool)
                                        if ((bool)obj)
                                            results.Add(channel);
                            });
                        }
            }

            return new Objects.JSChannelCollection(eng.Object.InstancePrototype, results.ToArray(), eng.ScriptName);
        }
    }
}
