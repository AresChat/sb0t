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
    class FloodControl
    {
        public static bool IsFlooding(IClient client, String ident, byte[] data, ulong time)
        {
            return IsFlooding(client, WebMsgToTCPMsg(ident), data, time);
        }

        public static bool IsFlooding(IClient client, TCPMsg msg, byte[] data, ulong time)
        {
            if (msg == TCPMsg.MSG_CHAT_CLIENT_PUBLIC || msg == TCPMsg.MSG_CHAT_CLIENT_EMOTE)
            {
                if (IsTextFlood(client))
                    return true;

                if (client.Level > ILevel.Regular)
                    return false;

                client.FloodRecord.recent_posts.Insert(0, data);

                if (client.FloodRecord.recent_posts.Count == 5)
                {
                    client.FloodRecord.recent_posts.RemoveAt(4);

                    if (client.FloodRecord.recent_posts.TrueForAll(x => x.SequenceEqual(client.FloodRecord.recent_posts[0])))
                        return true;
                }
            }

            if (client.Level > ILevel.Regular)
                return false;

            if (msg == TCPMsg.MSG_CHAT_CLIENT_ADDSHARE || msg == TCPMsg.MSG_CHAT_CLIENT_FASTPING)
                return false;

            if (time > (client.FloodRecord.last_packet_time + 1000))
            {
                client.FloodRecord.last_packet_time = time;
                client.FloodRecord.packet_counter_main = 0;
                client.FloodRecord.packet_counter_misc = 0;
                client.FloodRecord.packet_counter_pm = 0;
                return false;
            }
            else
            {
                if (msg == TCPMsg.MSG_CHAT_CLIENT_PUBLIC || msg == TCPMsg.MSG_CHAT_CLIENT_EMOTE)
                    return ++client.FloodRecord.packet_counter_main > 3;

                if (msg == TCPMsg.MSG_CHAT_CLIENT_PVT)
                    return ++client.FloodRecord.packet_counter_pm > 5;

                return ++client.FloodRecord.packet_counter_misc > 8;
            }
        }

        public static void Reset()
        {
            if (last_typer == null)
                last_typer = new List<IPAddress>();

            last_typer.Clear();
        }

        public static void Remove(IClient client)
        {
            last_typer.RemoveAll(x => x.Equals(client.ExternalIP));
        }

        private static List<IPAddress> last_typer { get; set; }

        public static bool IsTextFlood(IClient userobj)
        {
            last_typer.Add(userobj.ExternalIP);

            if (last_typer.Count > 8)
                last_typer.RemoveAt(0);

            if (last_typer.FindAll(x => x.Equals(userobj.ExternalIP)).Count == 8) // flooding
            {
                last_typer.Clear();

                if (userobj.Level == ILevel.Regular)
                    return true;
            }

            return false;
        }

        public static void LinkPost()
        {
            last_typer.Add(IPAddress.Any);

            if (last_typer.Count > 8)
                last_typer.RemoveAt(0);
        }

        public static TCPMsg WebMsgToTCPMsg(String ident)
        {
            switch (ident)
            {
                case "LOGIN":
                    return TCPMsg.MSG_CHAT_CLIENT_LOGIN;

                case "PUBLIC":
                    return TCPMsg.MSG_CHAT_CLIENT_PUBLIC;

                case "EMOTE":
                    return TCPMsg.MSG_CHAT_CLIENT_EMOTE;

                case "COMMAND":
                    return TCPMsg.MSG_CHAT_CLIENT_COMMAND;

                case "PING":
                    return TCPMsg.MSG_CHAT_CLIENT_UPDATE_STATUS;

                case "PM":
                    return TCPMsg.MSG_CHAT_CLIENT_PVT;

                default:
                    return (TCPMsg)255;
            }
        }
    }
}
