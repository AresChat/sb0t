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
using System.Security.Cryptography;

namespace core.ib0t
{
    class WebSockets
    {
        public static byte[] Html5TextPacket(String str, bool old_proto)
        {
            if (old_proto)
            {
                List<byte> list = new List<byte>();
                list.Add(0);
                list.AddRange(Encoding.UTF8.GetBytes(str));
                list.Add(255);
                return list.ToArray();
            }
            else
            {
                byte[] text = Encoding.UTF8.GetBytes(str);
                List<byte> list = new List<byte>();
                list.Add(129);

                if (text.Length <= 125)
                    list.Add((byte)text.Length);
                else if (text.Length < 65535)
                {
                    list.Add(126);
                    byte[] len = BitConverter.GetBytes((ushort)text.Length);
                    Array.Reverse(len);
                    list.AddRange(len);
                }
                else
                {
                    list.Add(127);
                    byte[] len = BitConverter.GetBytes((ulong)text.Length);
                    Array.Reverse(len);
                    list.AddRange(len);
                }

                list.AddRange(text);

                return list.ToArray();
            }
        }

        public static byte[] Html5HandshakeReplyPacket(Html5RequestEventArgs e)
        {
            if (e.OldProto)
            {
                String str = String.Empty;
                str += "HTTP/1.1 101 WebSocket Protocol Handshake\r\n";
                str += "Upgrade: WebSocket\r\n";
                str += "Connection: Upgrade\r\n";
                str += "Sec-WebSocket-Origin: " + e.Origin + "\r\n";
                str += "Sec-WebSocket-Location: ws://" + e.Host + "/\r\n";
                str += "\r\n";

                List<byte> list = new List<byte>();
                list.AddRange(Encoding.Default.GetBytes(str));
                list.AddRange(BuildOldKeyResponse(e.Key1, e.Key2, e.KeyData));

                return list.ToArray();
            }
            else
            {
                String str = String.Empty;
                str += "HTTP/1.1 101 Switching Protocols\r\n";
                str += "Upgrade: websocket\r\n";
                str += "Connection: Upgrade\r\n";

                String sha1 = e.Key;
                sha1 += "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                byte[] buffer = Encoding.Default.GetBytes(sha1);
                buffer = SHA1.Create().ComputeHash(buffer);
                sha1 = Convert.ToBase64String(buffer);

                str += "Sec-WebSocket-Accept: " + sha1 + "\r\n";
                str += "Access-Control-Allow-Origin: " + e.Origin + "\r\n";
                str += "Access-Control-Allow-Credentials: true\r\n";
                str += "Access-Control-Allow-Headers: content-type\r\n";
                str += "\r\n";

                return Encoding.Default.GetBytes(str);
            }
        }

        private static byte[] BuildOldKeyResponse(String key1, String key2, byte[] cookie)
        {
            List<byte> list = new List<byte>();
            long l;
            int i;
            byte[] buf;

            l = long.Parse(new String(key1.Where(x => Char.IsNumber(x)).ToArray()));
            i = (int)(l / key1.Count(x => x == ' '));
            buf = BitConverter.GetBytes(i);
            Array.Reverse(buf);
            list.AddRange(buf);

            l = long.Parse(new String(key2.Where(x => Char.IsNumber(x)).ToArray()));
            i = (int)(l / key2.Count(x => x == ' '));
            buf = BitConverter.GetBytes(i);
            Array.Reverse(buf);
            list.AddRange(buf);

            list.AddRange(cookie);

            return MD5.Create().ComputeHash(list.ToArray());
        }
    }
}
