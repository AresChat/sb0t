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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;

namespace scripting
{
    class ScriptManager
    {
        public static List<JSScript> Scripts { get; private set; }
        public static ConcurrentQueue<ICallback> Callbacks { get; private set; }

        public static void AutoRun()
        {
            Destroy();
            Callbacks = new ConcurrentQueue<ICallback>();
            Scripts = new List<JSScript>();
            Scripts.Add(new JSScript("room"));

            try
            {
                String path = Path.Combine(Server.DataPath, "autorun.dat");

                if (File.Exists(path))
                {
                    String[] lines = File.ReadAllLines(path);

                    foreach (String str in lines)
                        Load(str, false);
                }
            }
            catch { }
        }

        private static void UpdateAutorun()
        {
            try
            {
                List<String> list = new List<String>();

                for (int i = 1; i < Scripts.Count; i++)
                    list.Add(Scripts[i].ScriptName);

                String path = Path.Combine(Server.DataPath, "autorun.dat");
                File.WriteAllLines(path, list.ToArray());
            }
            catch { }
        }

        public static void RemoveCallbacks(String script)
        {
            var linq = from x in Callbacks.ToList()
                       where x.ScriptName != script
                       select x;

            Callbacks = new ConcurrentQueue<ICallback>(linq);
        }

        public static void KillScript(String name)
        {
            int index = Scripts.FindIndex(x => x.ScriptName == name);

            if (index > 0)
            {
                Scripts[index].KillScript();
                Scripts.RemoveAt(index);
                UpdateAutorun();
            }
        }

        public static void DequeueCallbacks()
        {
            while (Callbacks.Count > 0)
            {
                ICallback item;

                if (Callbacks.TryDequeue(out item))
                {
                    try
                    {
                        if (item is Objects.JSHttpRequestResult)
                        {
                            Objects.JSHttpRequestResult obj = (Objects.JSHttpRequestResult)item;
                            JSScript script = Scripts.Find(x => x.ScriptName == item.ScriptName);

                            if (script != null)
                                obj.Callback.Call(obj, !String.IsNullOrEmpty(obj.Data));
                        }
                        else if (item is Objects.JSAvatarImage)
                        {
                            Objects.JSAvatarImage obj = (Objects.JSAvatarImage)item;
                            JSScript script = Scripts.Find(x => x.ScriptName == item.ScriptName);

                            if (script != null)
                                obj.Callback.Call(obj, obj.Data != null);
                        }
                        else if (item is Objects.JSScribbleImage)
                        {
                            Objects.JSScribbleImage obj = (Objects.JSScribbleImage)item;
                            JSScript script = Scripts.Find(x => x.ScriptName == item.ScriptName);

                            if (script != null)
                                obj.Callback.Call(obj, obj.Data != null);
                        }
                    }
                    catch { }
                }
                else break;
            }
        }

        public static bool Load(String f, bool update_autorun)
        {
            FileInfo file = null;

            try
            {
                file = new FileInfo(f);
                file = new FileInfo(Path.Combine(Server.DataPath, file.Name, file.Name));
            }
            catch { return false; }

            if (file.Name == "room")
                return false;

            int index = Scripts.FindIndex(x => x.ScriptName == file.Name);

            if (index > 0)
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
                        ErrorDispatcher.SendError(script.ScriptName, e.Message, e.LineNumber);
                    }
                    catch { }

                    if (update_autorun)
                        UpdateAutorun();

                    return true;
                }
            }

            return false;
        }

        public static void Destroy()
        {
            if (Scripts != null)
            {
                Scripts.ForEach(x => x.KillScript());
                Scripts.Clear();
            }
        }
    }
}
