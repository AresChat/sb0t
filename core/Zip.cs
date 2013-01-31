using System.IO;
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace core
{
    class Zip
    {
        public static byte[] Compress(byte[] data)
        {
            byte[] result = null;

            using (MemoryStream ms = new MemoryStream())
            using (Stream s = new DeflaterOutputStream(ms))
            {
                s.Write(data, 0, data.Length);
                s.Close();
                result = ms.ToArray();
            }

            return result;
        }

        public static byte[] Decompress(byte[] data)
        {
            try
            {
                byte[] r = null;

                using (MemoryStream ms = new MemoryStream(data))
                using (Stream s = new InflaterInputStream(ms))
                {
                    List<byte> list = new List<byte>();
                    int count = 0;
                    byte[] b = new byte[8192];

                    while ((count = s.Read(b, 0, 8192)) > 0)
                        list.AddRange(b.Take(count));

                    r = list.ToArray();
                }

                return r;
            }
            catch { }

            return new byte[] { };
        }
    }
}
