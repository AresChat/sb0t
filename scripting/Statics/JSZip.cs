using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Statics
{
    [JSEmbed(Name = "Zip")]
    class JSZip : ObjectInstance
    {
        public JSZip(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Zip"; }
        }

        [JSFunction(Name = "compress", IsWritable = false, IsEnumerable = true)]
        public static String Compress(object a)
        {
            String result = null;

            if (!(a is Undefined))
            {
                try
                {
                    String str = a.ToString();
                    byte[] buf = Encoding.UTF8.GetBytes(str);
                    buf = Server.Compression.Compress(buf);
                    result = Encoding.Default.GetString(buf);
                }
                catch { }
            }

            return result;
        }

        [JSFunction(Name = "uncompress", IsWritable = false, IsEnumerable = true)]
        public static String Uncompress(object a)
        {
            String result = null;

            if (!(a is Undefined))
            {
                try
                {
                    String str = a.ToString();
                    byte[] buf = Encoding.Default.GetBytes(str);
                    buf = Server.Compression.Decompress(buf);
                    result = Encoding.UTF8.GetString(buf);
                }
                catch { }
            }

            return result;
        }
    }
}
