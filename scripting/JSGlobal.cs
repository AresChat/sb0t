/*
    sb0t ares chat server
    Copyright (C) 2016  AresChat

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jurassic;
using Jurassic.Library;
using System.Text.RegularExpressions;

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

        [JSFunction(Name = "tickCount")]
        public static double TickCount()
        {
            return Server.Ticks;
        }

        [JSFunction(Name = "escapeUtf")]
        public static String EscapeUTF(object a)
        {
            if (a is Undefined)
                return null;

            String result = String.Empty;
            char[] letters = a.ToString().ToCharArray();

            foreach (char c in letters)
                if ((c >= 97 && c <= 122) ||
                    (c >= 65 && c <= 90) ||
                    (c >= 48 && c <= 57))
                    result += c.ToString();
                else
                {
                    String s = String.Format("{0:X2}", Convert.ToUInt32(c));

                    if ((s.Length % 2) != 0)
                        s = "0" + s;

                    if (s.Length == 2)
                        result += "\\x" + s;
                    else
                        result += "\\u" + s;
                }

            return result;
        }

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
                            eng.ExecuteFile(path);
                        }
                        catch (Jurassic.JavaScriptException e)
                        {
                            ErrorDispatcher.SendError(filename, e.Message, e.LineNumber);
                        }
                        catch { }
            }
        }

        [JSFunction(Name = "includeAll", Flags = JSFunctionFlags.HasEngineParameter)]
        public static void IncludeAll(ScriptEngine eng)
        {
            DirectoryInfo directory = new DirectoryInfo(Path.Combine(Server.DataPath, eng.ScriptName));
            FileInfo[] files = directory.GetFiles("*.js");
            String main = ScriptName(eng);

            foreach (FileInfo file in files)
                if (file.Name != main)
                    Include(eng, file.Name);
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

            if (b is Undefined || c is Undefined)
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

        [JSFunction(Name = "sendText")]
        public static void SendText(object a, object b, object c)
        {
            if (!(a is Objects.JSUser))
                return;

            if (b is Undefined || c is Undefined)
                return;

            if (b != null && c != null)
            {
                String sender = b.ToString();
                String text = c.ToString();

                if (String.IsNullOrEmpty(sender) || String.IsNullOrEmpty(text))
                    return;

                Server.PublicToTarget(((Objects.JSUser)a).parent, sender, text);
            }
        }

        [JSFunction(Name = "sendEmote")]
        public static void SendEmote(object a, object b, object c)
        {
            if (!(a is Objects.JSUser))
                return;

            if (b is Undefined || c is Undefined)
                return;

            if (b != null && c != null)
            {
                String sender = b.ToString();
                String text = c.ToString();

                if (String.IsNullOrEmpty(sender) || String.IsNullOrEmpty(text))
                    return;

                Server.EmoteToTarget(((Objects.JSUser)a).parent, sender, text);
            }
        }

        [JSFunction(Name = "stripColors")]
        public static String StripColors(object a)
        {
            if (a is Undefined || a is Null)
                return null;

            String input = a.ToString();

            if (Regex.IsMatch(input, @"\x03|\x05", RegexOptions.IgnoreCase))
                input = Regex.Replace(input, @"(\x03|\x05)[0-9]{2}", "");

            input = input.Replace("\x06", "");
            input = input.Replace("\x07", "");
            input = input.Replace("\x09", "");
            input = input.Replace("\x02", "");
            input = input.Replace("­", "");

            return input;
        }
    }
}
