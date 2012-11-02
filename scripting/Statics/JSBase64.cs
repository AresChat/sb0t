using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Statics
{
    [JSEmbed(Name = "Base64")]
    class JSBase64 : ObjectInstance
    {
        public JSBase64(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "encode", IsWritable = false, IsEnumerable = true)]
        public static String Encode(object a)
        {
            if (a is Null || a is Undefined)
                return null;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(a.ToString()));
        }

        [JSFunction(Name = "decode", IsWritable = false, IsEnumerable = true)]
        public static String Decode(object a)
        {
            if (a is String || a is ConcatenatedString)
            {
                String str = a.ToString();

                try
                {
                    return Encoding.UTF8.GetString(Convert.FromBase64String(str));
                }
                catch { }
            }

            return null;
        }
    }
}
