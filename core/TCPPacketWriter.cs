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

namespace core
{
    class TCPPacketWriter
    {
        private List<byte> Data = new List<byte>();

        public int GetByteCount()
        {
            return this.Data.Count;
        }

        public void WriteByte(byte b)
        {
            this.Data.Add(b);
        }

        public void WriteBytes(byte[] b)
        {
            this.Data.AddRange(b);
        }

        public void WriteGuid(Guid g)
        {
            this.Data.AddRange(g.ToByteArray());
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

        public void WriteIP(String ip_string)
        {
            this.Data.AddRange(IPAddress.Parse(ip_string).GetAddressBytes());
        }

        public void WriteIP(long ip_numeric)
        {
            byte[] temp = BitConverter.GetBytes((uint)ip_numeric);
            Array.Reverse(temp);
            this.Data.AddRange(new IPAddress(temp).GetAddressBytes());
        }

        public void WriteIP(byte[] ip_bytes)
        {
            if (ip_bytes.Length != 4) return;
            this.Data.AddRange(new IPAddress(ip_bytes).GetAddressBytes());
        }

        public void WriteIP(IPAddress ip_object)
        {
            this.Data.AddRange(ip_object.GetAddressBytes());
        }

        public void WriteString(String text, bool nulled, bool unenc)
        {
            this.Data.AddRange(Encoding.UTF8.GetBytes(text));

            if (nulled)
                this.Data.Add(0);
        }

        public void WriteString(IClient client, String text)
        {
            this.WriteString(client, text, true);
        }

        public void WriteString(IClient client, String text, bool null_terminated)
        {
            if (client.Encryption.Mode == EncryptionMode.Encrypted)
            {
                byte[] data = Encoding.UTF8.GetBytes(text);
                data = Crypto.Encrypt(data, client.Encryption.Key, client.Encryption.IV);
                this.WriteUInt16((ushort)data.Length);
                this.WriteBytes(data);
            }
            else this.Data.AddRange(Encoding.UTF8.GetBytes(text));

            if (null_terminated)
                this.Data.Add(0);
        }

        public void WriteString(core.LinkHub.Leaf leaf, String text)
        {
            this.WriteString(leaf, text, true);
        }

        public void WriteString(core.LinkHub.Leaf leaf, String text, bool null_terminated)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            data = Crypto.Encrypt(data, leaf.Key, leaf.IV);
            this.WriteUInt16((ushort)data.Length);
            this.WriteBytes(data);

            if (null_terminated)
                this.Data.Add(0);
        }

        public void WriteString(core.LinkLeaf.LinkClient client, String text)
        {
            this.WriteString(client, text, true);
        }

        public void WriteString(core.LinkLeaf.LinkClient client, String text, bool null_terminated)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            data = Crypto.Encrypt(data, client.Key, client.IV);
            this.WriteUInt16((ushort)data.Length);
            this.WriteBytes(data);

            if (null_terminated)
                this.Data.Add(0);
        }

        public void ReplaceByte(byte b, int i)
        {
            this.Data[i] = b;
        }

        public byte[] ToByteArray()
        {
            return this.Data.ToArray();
        }

        public byte[] ToAresPacket(TCPMsg packet_id)
        {
            List<byte> tmp = new List<byte>(this.Data.ToArray());
            tmp.Insert(0, (byte)packet_id);
            tmp.InsertRange(0, BitConverter.GetBytes((ushort)(tmp.Count - 1)));
            return tmp.ToArray();
        }

        public byte[] ToLinkPacket(core.LinkHub.LinkMsg msg)
        {
            List<byte> tmp = new List<byte>(this.Data.ToArray());
            tmp.Insert(0, (byte)msg);
            tmp.InsertRange(0, BitConverter.GetBytes((ushort)(tmp.Count - 1)));
            return tmp.ToArray();
        }
    }
}
