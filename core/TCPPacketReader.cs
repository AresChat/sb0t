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
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Security.Cryptography;

namespace core
{
    class TCPPacketReader
    {

        private static String[] bad_chars = new String[] // skiddy
            {
                "\u00A0",
                "\u00AD",
                "\u009d",
                "\r\n",
                "\r",
                "\n",
                "",
                "",
                "\x00cc\x00b8",
                "͋"
            };

        private int Position = 0;
        private List<byte> Data = new List<byte>();

        public TCPPacketReader(byte[] bytes)
        {
            this.Data.Clear();
            this.Position = 0;
            this.Data.AddRange(bytes);
        }

        public int Remaining
        {
            get { return this.Data.Count - this.Position; }
        }

        public void SkipByte()
        {
            this.Position++;
        }

        public void SkipBytes(int count)
        {
            this.Position += count;
        }

        public static implicit operator byte(TCPPacketReader p)
        {
            byte tmp = p.Data[p.Position];
            p.Position++;
            return tmp;
        }

        public static implicit operator byte[](TCPPacketReader p)
        {
            byte[] tmp = new byte[p.Data.Count - p.Position];
            Array.Copy(p.Data.ToArray(), p.Position, tmp, 0, tmp.Length);
            p.Position += tmp.Length;
            return tmp;
        }

        public static implicit operator Guid(TCPPacketReader p)
        {
            byte[] tmp = new byte[16];
            Array.Copy(p.Data.ToArray(), p.Position, tmp, 0, tmp.Length);
            p.Position += 16;

            using (MD5 md5 = MD5.Create())
                tmp = md5.ComputeHash(tmp);

            return new Guid(tmp);
        }

        public static implicit operator ushort(TCPPacketReader p)
        {
            ushort tmp = BitConverter.ToUInt16(p.Data.ToArray(), p.Position);
            p.Position += 2;
            return tmp;
        }

        public static implicit operator uint(TCPPacketReader p)
        {
            uint tmp = BitConverter.ToUInt32(p.Data.ToArray(), p.Position);
            p.Position += 4;
            return tmp;
        }

        public static implicit operator ulong(TCPPacketReader p)
        {
            ulong tmp = BitConverter.ToUInt64(p.Data.ToArray(), p.Position);
            p.Position += 8;
            return tmp;
        }

        public static implicit operator IPAddress(TCPPacketReader p)
        {
            byte[] tmp = new byte[4];
            Array.Copy(p.Data.ToArray(), p.Position, tmp, 0, tmp.Length);
            p.Position += 4;
            return new IPAddress(tmp);
        }

        public byte[] ReadBytes(int count)
        {
            byte[] tmp = new byte[count];
            Array.Copy(this.Data.ToArray(), this.Position, tmp, 0, tmp.Length);
            this.Position += tmp.Length;
            return tmp;
        }

        public String ReadString(core.LinkHub.Leaf leaf)
        {
            String str = String.Empty;
            ushort size = this;
            byte[] data = this.ReadBytes(size);
            data = Crypto.Decrypt(data, leaf.Key, leaf.IV);
            str = Encoding.UTF8.GetString(data);

            if (this.Position < this.Data.Count)
                if (this.Data[this.Position] == 0)
                    this.Position++;

            foreach (String c in bad_chars)
                str = Regex.Replace(str, Regex.Escape(c), "", RegexOptions.IgnoreCase);

            return str.Trim();
        }

        public String ReadString(core.LinkLeaf.LinkClient client)
        {
            String str = String.Empty;
            ushort size = this;
            byte[] data = this.ReadBytes(size);
            data = Crypto.Decrypt(data, client.Key, client.IV);
            str = Encoding.UTF8.GetString(data);

            if (this.Position < this.Data.Count)
                if (this.Data[this.Position] == 0)
                    this.Position++;

            foreach (String c in bad_chars)
                str = Regex.Replace(str, Regex.Escape(c), "", RegexOptions.IgnoreCase);

            return str.Trim();
        }

        public String ReadString(IClient client)
        {
            String str = String.Empty;

            if (client.Encryption.Mode == EncryptionMode.Encrypted)
            {
                ushort size = this;
                byte[] data = this.ReadBytes(size);
                data = Crypto.Decrypt(data, client.Encryption.Key, client.Encryption.IV);
                str = Encoding.UTF8.GetString(data);

                if (this.Position < this.Data.Count)
                    if (this.Data[this.Position] == 0)
                        this.Position++;
            }
            else
            {
                int split = this.Data.IndexOf(0, this.Position);
                byte[] tmp = new byte[split > -1 ? (split - this.Position) : (this.Data.Count - this.Position)];
                Array.Copy(this.Data.ToArray(), this.Position, tmp, 0, tmp.Length);
                this.Position = split > -1 ? (split + 1) : this.Data.Count;
                str = Encoding.UTF8.GetString(tmp);
            }      

            foreach (String c in bad_chars)
                str = Regex.Replace(str, Regex.Escape(c), "", RegexOptions.IgnoreCase);

            return str.Trim();
        }

        public String ReadString()
        {
            String str = String.Empty;
            int split = this.Data.IndexOf(0, this.Position);
            byte[] tmp = new byte[split > -1 ? (split - this.Position) : (this.Data.Count - this.Position)];
            Array.Copy(this.Data.ToArray(), this.Position, tmp, 0, tmp.Length);
            this.Position = split > -1 ? (split + 1) : this.Data.Count;
            str = Encoding.UTF8.GetString(tmp);

            foreach (String c in bad_chars)
                str = Regex.Replace(str, Regex.Escape(c), "", RegexOptions.IgnoreCase);

            return str.Trim();
        }

        public byte[] ToArray()
        {
            return this.Data.ToArray();
        }
    }
}
