using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.ib0t
{
    class Html5RequestEventArgs
    {
        public String Key { get; set; }
        public String Key1 { get; set; }
        public String Key2 { get; set; }
        public byte[] KeyData { get; set; }
        public bool OldProto { get; set; }
        public String Origin { get; set; }
        public String Host { get; set; }
        public ulong Time { get; set; }
    }
}
