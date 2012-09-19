using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    /// <summary>Compression Utility</summary>
    public interface ICompression
    {
        /// <summary>Compress a byte array</summary>
        byte[] Compress(byte[] data);
        /// <summary>Decompress a byte array</summary>
        byte[] Decompress(byte[] data);
    }
}
