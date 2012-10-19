using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Statics
{
    class JSUsers : ObjectInstance
    {
        public JSUsers(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "test", IsWritable = false, IsEnumerable = true)]
        public static String Test()
        {
            return "test complete";
        }
    }
}
