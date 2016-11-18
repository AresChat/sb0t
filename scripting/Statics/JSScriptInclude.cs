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

        protected override string InternalClassName
        {
            get { return "Script"; }
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
