using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    class AdminSystem
    {
        private static Random rnd;

        public static uint NextCookie
        {
            get
            {
                if (rnd == null)
                    rnd = new Random();

                return (uint)Math.Floor(rnd.NextDouble() * (uint.MaxValue - 1));
            }
        }


    }
}
