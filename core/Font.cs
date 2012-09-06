using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    class Font
    {
        public String Family { get; set; }
        public byte Size { get; set; }
        public byte NameColor { get; set; }
        public byte TextColor { get; set; }
        public byte[] NameColorNew { get; set; }
        public byte[] TextColorNew { get; set; }
        public bool HasFont { get; set; }

        public Font()
        {
            this.Family = String.Empty;
        }
    }
}
