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
    class UdpPacketWriter
    {
        private List<byte> Data = new List<byte>();

        public void WriteByte(byte b)
        {
            this.Data.Add(b);
        }

        public void WriteBytes(byte[] b)
        {
            this.Data.AddRange(b);
        }

        public void WriteUInt16(ushort i)
        {
            this.Data.AddRange(BitConverter.GetBytes(i));
        }

        public void WriteUInt32(uint i)
        {
            this.Data.AddRange(BitConverter.GetBytes(i));
        }

        public void WriteUInt64(ulong i)
        {
            this.Data.AddRange(BitConverter.GetBytes(i));
        }

        public void WriteIP(IPAddress ip_object)
        {
            this.Data.AddRange(ip_object.GetAddressBytes());
        }

        public void WriteString(String text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            this.Data.AddRange(BitConverter.GetBytes((ushort)buffer.Length));
            this.Data.AddRange(buffer);
        }

        public byte[] ToAresPacket(UdpMsg packet_id)
        {
            List<byte> tmp = new List<byte>(this.Data.ToArray());
            tmp.Insert(0, (byte)packet_id);
            return tmp.ToArray();
        }
    }
}
