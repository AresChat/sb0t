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
using iconnect;

namespace core
{
    public class Hashlink
    {
        public static byte[] d67(byte[] data, int b)
        {
            byte[] buffer = new byte[data.Length];
            Array.Copy(data, buffer, data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                buffer[i] = (byte)(data[i] ^ b >> 8 & 255);
                b = (b + data[i]) * 23219 + 36126 & 65535;
            }
            return buffer;
        }

        public static byte[] e67(byte[] data, int b)
        {
            byte[] buffer = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                buffer[i] = (byte)((data[i] ^ (b >> 8)) & 255);
                b = ((buffer[i] + b) * 23219 + 36126) & 65535;
            }

            return buffer;
        }

        public static String EncodeHashlink(IHashlinkRoom room)
        {
            List<byte> list = new List<byte>();
            list.AddRange(new byte[20]);
            list.AddRange(Encoding.UTF8.GetBytes("CHATCHANNEL"));
            list.Add(0);
            list.AddRange(room.IP.GetAddressBytes());
            list.AddRange(BitConverter.GetBytes(room.Port));
            list.AddRange(room.IP.GetAddressBytes());
            list.AddRange(Encoding.UTF8.GetBytes(room.Name));
            list.Add(0);
            list.Add(0);

            byte[] buf = list.ToArray();
            buf = Zip.Compress(buf);
            buf = e67(buf, 28435);

            return Convert.ToBase64String(buf);
        }

        public static Room DecodeHashlink(String hashlink)
        {
            Room room = new Room();

            hashlink = hashlink.Trim();

            if (hashlink.StartsWith("arlnk://"))
                hashlink = hashlink.Substring(8);

            try
            {
                if (hashlink.ToUpper().StartsWith("CHATROOM:")) // not encrypted
                {
                    hashlink = hashlink.Substring(9);
                    int split = hashlink.IndexOf(":");
                    room.IP = IPAddress.Parse(hashlink.Substring(0, split));
                    hashlink = hashlink.Substring(split + 1);
                    split = hashlink.IndexOf("|");
                    room.Port = ushort.Parse(hashlink.Substring(0, split));
                    room.Name = hashlink.Substring(split + 1);
                    return room;
                }
                else // encrypted
                {
                    byte[] buf = Convert.FromBase64String(hashlink);
                    buf = d67(buf, 28435);
                    buf = Zip.Decompress(buf);

                    TCPPacketReader packet = new TCPPacketReader(buf);
                    packet.SkipBytes(32);
                    room.IP = packet;
                    room.Port = packet;
                    packet.SkipBytes(4);
                    room.Name = packet.ReadString();

                    return room;
                }
            }
            catch // badly formed hashlink
            {
                return null;
            }
        }
    }
}
