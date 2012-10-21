using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Jurassic;
using Jurassic.Library;
using iconnect;

namespace scripting.Statics
{
    [JSEmbed(Name = "Hashlink")]
    class JSHashlink : ObjectInstance
    {
        public JSHashlink(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "encode", IsWritable = false, IsEnumerable = true)]
        public static String Encode(object a)
        {
            if (a is ObjectInstance)
            {
                try
                {
                    ObjectInstance obj = (ObjectInstance)a;
                    Hashlink hashlink = new Hashlink
                    {
                        IP = IPAddress.Parse(obj.GetPropertyValue("ip").ToString()),
                        Port = ushort.Parse(obj.GetPropertyValue("port").ToString()),
                        Name = obj.GetPropertyValue("name").ToString()
                    };

                    String str = "arlnk://" + Server.Hashlinks.Encrypt(hashlink);

                    if (str != null)
                        return str;
                }
                catch { }
            }

            return null;
        }

        [JSFunction(Name = "decode", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static Objects.JSHashlinkResult Decode(ScriptEngine eng, object a)
        {
            if (a is String || a is ConcatenatedString)
            {
                String str = a.ToString();

                try
                {
                    IHashlinkRoom hashlink = Server.Hashlinks.Decrypt(a.ToString());

                    if (hashlink != null)
                        return new Objects.JSHashlinkResult(eng.Object.InstancePrototype, hashlink);
                }
                catch { }
            }

            return null;
        }
    }
}
