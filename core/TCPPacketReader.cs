using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

namespace core
{
    class TCPPacketReader
    {
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

        public String ReadString(AresClient client)
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

            String[] bad_chars = new String[] // skiddy
            {
                "\r\n",
                "\r",
                "\n",
                "",
                "",
                "\x00cc\x00b8",
                "͋"
            };

            foreach (String c in bad_chars)
                str = Regex.Replace(str, Regex.Escape(c), "", RegexOptions.IgnoreCase);

            return str;
        }

        public byte[] ToArray()
        {
            return this.Data.ToArray();
        }
    }
}
