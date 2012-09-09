using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    class TCPAdvancedProcessor
    {
        public static void Eval(AresClient client, TCPPacket packet, ulong time)
        {
            packet.Packet.SkipBytes(2);
            TCPMsg msg = (TCPMsg)(byte)packet.Packet;

            switch (msg)
            {
                case TCPMsg.MSG_CHAT_CLIENT_CUSTOM_FONT:
                    CustomFont(client, packet.Packet);
                    break;

                default:
                    Events.UnhandledProtocol(client, packet.Msg, packet.Packet, time);
                    break;
            }
        }

        private static void CustomFont(AresClient client, TCPPacketReader packet)
        {
            if (packet.Remaining <= 2)
            {
                client.Font.HasFont = false;
                return;
            }

            client.Font.HasFont = true;
            client.Font.Size = packet;
            client.Font.Family = packet.ReadString(client);
            client.Font.NameColor = packet;
            client.Font.TextColor = packet;
            client.Font.NameColorNew = packet;
            client.Font.TextColorNew = packet;

            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.CustomFont(x, client)),
                x => x.LoggedIn && x.Vroom == client.Vroom && x.CustomClient);
        }

    }
}
