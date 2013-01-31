using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace core
{
    class AresFont : IFont
    {
        public bool Enabled { get; set; }
        public String NameColor { get; set; }
        public String TextColor { get; set; }
        public String FontName { get; set; }
        public int Size { get; set; }
        public bool IsEmote { get; set; }

        public AresFont()
        {
            this.Size = 12;
        }
    }
}
