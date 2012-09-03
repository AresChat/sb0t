using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace core
{
    class Time
    {
        private static Stopwatch sw;

        public static void Reset()
        {
            if (sw != null)
            {
                sw.Stop();
                sw.Reset();
            }

            sw = new Stopwatch();
            sw.Start();
        }

        public static uint Now
        {
            get { return (uint)sw.ElapsedMilliseconds; }
        }
    }
}
