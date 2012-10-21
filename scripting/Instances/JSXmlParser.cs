using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Instances
{
    [JSEmbed(Name = "XmlParser")]
    class JSXmlParser : ClrFunction
    {
        public JSXmlParser(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "XmlParser", new JSXmlParserInstance(engine.Object.InstancePrototype))
        {

        }

        [JSConstructorFunction]
        public JSXmlParserInstance Construct()
        {
            return new JSXmlParserInstance(this.InstancePrototype);
        }
    }
}
