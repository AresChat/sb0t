using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace commands
{
    class Settings
    {
        public static bool AnonMonitoring { get; set; }
        public static bool ShareFileMonitoring { get; set; }
        public static bool Filtering { get; set; }
        public static bool CapsMonitoring { get; set; }
        public static bool IdleMonitoring { get; set; }
        public static bool General { get; set; }
    }
}
