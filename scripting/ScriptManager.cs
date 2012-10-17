using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace scripting
{
    class ScriptManager
    {
        public static List<JSScript> Scripts { get; private set; }

        public static void AutoRun()
        {
            Scripts = new List<JSScript>();


        }

        public static void OnError(String script, String msg, int line)
        {
            JSScript[] scripts = Scripts.ToArray();

            foreach (JSScript s in scripts)
            {
                try { s.JS.CallGlobalFunction("onError", script, line, msg); }
                catch { }
            }
        }


    }
}
