/*
    sb0t ares chat server
    Copyright (C) 2016  AresChat

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System.IO;
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace core
{
    class Zip
    {
        public static byte[] GCompress(byte[] data)
        {
            byte[] result = null;

            using (MemoryStream ms = new MemoryStream())
            using (Stream s = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(ms))
            {
                s.Write(data, 0, data.Length);
                s.Close();
                result = ms.ToArray();
            }

            return result;
        }

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
