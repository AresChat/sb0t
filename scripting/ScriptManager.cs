using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace scripting
{
    class ScriptManager
    {
        public static List<JSScript> Scripts { get; private set; }
        public static JSScript RoomEval { get; private set; }

        public static void AutoRun()
        {
            Scripts = new List<JSScript>();
            RoomEval = new JSScript("room");

        }

        public static bool Load(FileInfo file)
        {
            if (file.Name == "room")
                return false;

            int index = Scripts.FindIndex(x => x.ScriptName == file.Name);

            if (index > -1)
            {
                Scripts[index].KillScript();
                Scripts.RemoveAt(index);
            }

            JSScript script = new JSScript(file.Name);

            if (File.Exists(file.FullName))
            {
                Scripts.Add(script);

                if (script.LoadScript(file.FullName))
                {
                    try
                    {
                        script.JS.CallGlobalFunction("onLoad");
                    }
                    catch (Jurassic.JavaScriptException e)
                    {
                        OnError(script.ScriptName, e.Message, e.LineNumber);
                    }
                    catch { }

                    return true;
                }
            }

            return false;
        }

        public static void OnError(String script, String msg, int line)
        {
            Server.Print("error: " + msg + " at line " + line);
        }


    }
}
