using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace core
{
    class SharedFile
    {
        public MimeType Mime { get; set; }
        public uint Size { get; set; }
        public byte[] Data { get; set; }
        public String FileName { get; set; }
        public String Title { get; set; }
    }
}
