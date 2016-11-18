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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace core.Udp
{
    class UdpPacketReader
    {
        private int Position = 0;
        private List<byte> Data = new List<byte>();

        public UdpPacketReader(byte[] bytes)
        {
            this.Data.Clear();
            this.Position = 0;
            this.Data.AddRange(bytes);
        }

        public int Remaining
        {
            get { return this.Data.Count - this.Position; }
        }

        public static implicit operator byte(UdpPacketReader p)
        {
            byte tmp = p.Data[p.Position];
            p.Position++;
            return tmp;
        }

        public byte[] ReadBytes(int count)
        {
            byte[] tmp = new byte[count];
            Array.Copy(this.Data.ToArray(), this.Position, tmp, 0, tmp.Length);
            this.Position += count;
            return tmp;
        }

        public static implicit operator byte[](UdpPacketReader p)
        {
            byte[] tmp = new byte[p.Data.Count - p.Position];
            Array.Copy(p.Data.ToArray(), p.Position, tmp, 0, tmp.Length);
            p.Position += tmp.Length;
            return tmp;
        }

        public static implicit operator ushort(UdpPacketReader p)
        {
            ushort tmp = BitConverter.ToUInt16(p.Data.ToArray(), p.Position);
            p.Position += 2;
            return tmp;
        }

        public static implicit operator uint(UdpPacketReader p)
        {
            uint tmp = BitConverter.ToUInt32(p.Data.ToArray(), p.Position);
            p.Position += 4;
            return tmp;
        }

        public static implicit operator ulong(UdpPacketReader p)
        {
            ulong tmp = BitConverter.ToUInt64(p.Data.ToArray(), p.Position);
            p.Position += 8;
            return tmp;
        }

        public static implicit operator IPAddress(UdpPacketReader p)
        {
            byte[] tmp = new byte[4];
            Array.Copy(p.Data.ToArray(), p.Position, tmp, 0, tmp.Length);
            p.Position += 4;
            return new IPAddress(tmp);
        }

        public static implicit operator String(UdpPacketReader p)
        {
            ushort len = BitConverter.ToUInt16(p.Data.ToArray(), p.Position);
            p.Position += 2;
            String str = Encoding.UTF8.GetString(p.Data.ToArray(), p.Position, len);
            p.Position += len;
            return str;
        }
    }
}
