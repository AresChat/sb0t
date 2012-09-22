using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    class FloodRecord
    {
        public List<byte[]> recent_posts = new List<byte[]>();
        public ulong last_packet_time = 0;
        public uint packet_counter_main = 0;
        public uint packet_counter_pm = 0;
        public uint packet_counter_misc = 0;
    }
}
