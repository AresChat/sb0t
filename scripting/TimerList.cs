using System;
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
