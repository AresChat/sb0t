using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jurassic;
using Jurassic.Library;

namespace scripting
{
    class JSGlobal
    {
        private static String[] bad_chars = new String[]
        {
            "..",
            "/",
            "\\",
            " ",
        };

        [JSFunction(Name = "scriptName", Flags = JSFunctionFlags.HasEngineParameter)]
        public static String ScriptName(ScriptEngine eng)
        {
            return eng.ScriptName;
        }

        [JSFunction(Name = "include", Flags = JSFunctionFlags.HasEngineParameter)]
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
                            eng.Evaluate(File.ReadAllText(path));
                        }
                        catch { }
            }
        }

        [JSFunction(Name = "byteLength")]
        public static int ByteLength(object a)
        {
            if (a is Undefined)
                return -1;

            return Encoding.UTF8.GetByteCount(a.ToString());
        }

        [JSFunction(Name = "clrName")]
        public static String ClrName(object a)
        {
            if (a is Undefined)
                return null;

            return a.GetType().ToString();
        }

        [JSFunction(Name = "user", Flags = JSFunctionFlags.HasEngineParameter)]
        public static Objects.JSUser User(ScriptEngine eng, object a)
        {
            if (a is Null)
                return null;

            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == eng.ScriptName);

            if (script == null)
                return null;

            Objects.JSUser result = null;


            if (a is String || a is ConcatenatedString)
            {
                if (a.ToString().Length < 2)
                    return null;

                result = script.local_users.Find(x => x.Name == ((String)a));

                if (result == null)
                    result = script.local_users.Find(x => x.Name.StartsWith((String)a));
            }
            else if (a is int || a is double)
            {
                int _i;

                if (int.TryParse(a.ToString(), out _i))
                    result = script.local_users.Find(x => x.Id == _i);
            }

            return result;
        }

        [JSFunction(Name = "print")]
        public static void Print(object a, object b)
        {
            if (b is Undefined) // to all local users
            {
                String result = null;

                if (!(a is Null || a is Undefined))
                    if (a is bool)
                        result = ((bool)a).ToString().ToLower();
                    else if (a != null)
                        result = a.ToString();

                if (result != null)
                    Server.Print(result);
            }
            else if (a is int || a is double) // to vroom
            {
                int _i;

                if (int.TryParse(a.ToString(), out _i))
                {
                    if (_i >= 0 && _i <= 65535)
                    {
                        String result = null;

                        if (!(b is Null || b is Undefined))
                            if (b is bool)
                                result = ((bool)b).ToString().ToLower();
                            else if (b != null)
                                result = b.ToString();

                        if (result != null)
                            Server.Print((ushort)_i, result);
                    }
                }
            }
            else if (a is Objects.JSUser) // to user
            {
                Objects.JSUser u = (Objects.JSUser)a;

                if (u != null)
                {
                    String result = null;

                    if (!(b is Null || b is Undefined))
                        if (b is bool)
                            result = ((bool)b).ToString().ToLower();
                        else if (b != null)
                            result = b.ToString();

                    if (result != null)
                        u.parent.Print(result);
                }
            }
        }

        [JSFunction(Name = "sendPM")]
        public static void SendPM(object a, object b, object c)
        {
            if (!(a is Objects.JSUser))
                return;

            if (b is Null || c is Null)
                return;

            if (b != null && c != null)
            {
                String sender = b.ToString();
                String text = c.ToString();

                if (String.IsNullOrEmpty(sender) || String.IsNullOrEmpty(text))
                    return;

                ((Objects.JSUser)a).parent.PM(sender, text);
            }
        }
    }
}
