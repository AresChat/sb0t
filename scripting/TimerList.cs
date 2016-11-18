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
using Jurassic;
using Jurassic.Library;

namespace scripting
{
    class TimerList
    {
        private static List<Instances.JSTimerInstance> list = new List<Instances.JSTimerInstance>();

        public static ulong NextIdent { get; set; }

        public static void Reset()
        {
            list = new List<Instances.JSTimerInstance>();
            NextIdent = 0;
        }

        public static void UpdateTimers()
        {
            ulong time = Server.Ticks;

            if (list.Count > 0)
            {
                for (int i = (list.Count - 1); i > -1; i--)
                {
                    if (list[i].StartTime <= (time - (ulong)list[i].Interval))
                    {
                        Instances.JSTimerInstance timer = list[i];
                        list.RemoveAt(i);
                        JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == timer.Engine.ScriptName);

                        if (script != null)
                        {
                            if (script.timer_idents.Contains(timer.IDENT))
                            {
                                try
                                {
                                    if (timer.Callback != null)
                                        timer.Callback.Call(timer.Engine.Global);
                                }
                                catch (JavaScriptException e)
                                {
                                    ErrorDispatcher.SendError(timer.ScriptName, e.Message, e.LineNumber);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
        }

        public static bool AddTimer(Instances.JSTimerInstance t)
        {
            if (!list.Contains(t))
            {
                t.StartTime = Server.Ticks;
                list.Add(t);
                return true;
            }

            return false;
        }

        public static bool RemoveTimer(Instances.JSTimerInstance t)
        {
            if (list.Contains(t))
            {
                list.Remove(t);
                return true;
            }

            return false;
        }

        public static void RemoveScriptTimers(String scriptName)
        {
            list.RemoveAll(x => x.ScriptName == scriptName);
        }
    }
}
