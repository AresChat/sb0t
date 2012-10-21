using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Objects
{
    class JSHttpRequestResult : ObjectInstance, ICallback
    {
        public JSHttpRequestResult(ObjectInstance prototype)
            : base(prototype)
        {
            this.PopulateFunctions();
            this.Data = String.Empty;
        }

        public String Data { get; set; }
        public UserDefinedFunction Callback { get; set; }
        public String ScriptName { get; set; }
        public String Arg { get; set; }

        [JSProperty(Name = "arg")]
        public String GetArgument
        {
            get { return this.Arg; }
            set { }
        }

        [JSProperty(Name = "page")]
        public String Page
        {
            get { return this.Data; }
            set { }
        }

        public override string ToString()
        {
            return "[object HttpRequestResult]";
        }
    }
}
