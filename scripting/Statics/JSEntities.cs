using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Jurassic;
using Jurassic.Library;

namespace scripting.Statics
{
    [JSEmbed(Name = "Entities")]
    class JSEntities : ObjectInstance
    {
        public JSEntities(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Entities"; }
        }

        [JSFunction(Name = "encode", IsWritable = false, IsEnumerable = true)]
        public static String Encode(object a)
        {
            if (a is Null || a is Undefined)
                return null;

            return WebUtility.HtmlEncode(a.ToString());
        }

        [JSFunction(Name = "decode", IsWritable = false, IsEnumerable = true)]
        public static String Decode(object a)
        {
            if (a is Null || a is Undefined)
                return null;

            return WebUtility.HtmlDecode(a.ToString());
        }
    }
}
