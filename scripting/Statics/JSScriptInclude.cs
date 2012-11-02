using System;
using System.Linq;
using System.IO;
using Jurassic;
using Jurassic.Library;

namespace scripting.Statics
{
    [JSEmbed(Name = "Script")]
    class JSScriptInclude : ObjectInstance
    {
        public JSScriptInclude(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        private static String[] bad_chars = new String[] { "..", "/", "\\", " ", };

        [JSFunction(Name = "include", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static void Include(ScriptEngine eng, object a)
        {
            if (!(a is Undefined))
            {
                String filename = a.ToString();

                if (filename.Length > 3 && filename.EndsWith(".js"))
                    if (bad_chars.Count<String>(x => filename.Contains(x)) == 0)
                        try
                        {
                            String path = Path.Combine(Server.DataPath, eng.ScriptName, filename);
                            eng.ExecuteFile(path);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ErrorDispatcher.SendError(filename, e.Message, e.LineNumber);
                        }
                        catch { }
            }
        }
    }
}
