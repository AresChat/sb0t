using System;
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
            if (msg == TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL)
                return false;

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

                default:
                    return (TCPMsg)255;
            }
        }
    }
}
