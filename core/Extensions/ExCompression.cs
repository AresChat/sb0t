using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace core.Extensions
{
    class ExCompression : ICompression
    {
        public byte[] Compress(byte[] data)
        {
            return Zip.Compress(data);
        }

        public byte[] Decompress(byte[] data)
        {
            return Zip.Decompress(data);
        }
    }
}
