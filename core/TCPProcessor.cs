using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    class TCPProcessor
    {
        public static void Eval(AresClient client, TCPPacket packet, uint time)
        {
            Events.PacketReceived(client, packet.Msg, packet.Packet.ToArray());

            switch (packet.Msg)
            {
                case TCPMsg.MSG_CHAT_CLIENT_LOGIN:
                    Login(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_UPDATE_STATUS:
                    client.Time = time;
                    break;
            }
        }

        private static void Login(AresClient client, TCPPacketReader packet)
        {
            client.Guid = packet;
            client.FileCount = packet;
            packet.SkipByte(); // crypto flag for cbot3
            client.DataPort = packet;
            client.NodeIP = packet;
            client.NodePort = packet;
            packet.SkipBytes(4);
            client.Name = packet;
            client.Version = packet;
            client.LocalIP = packet;
            packet.SkipBytes(4);
            byte flag = packet;
            client.Browsable = flag > 2;
            client.Compression = flag > 3;
            client.CurrentUploads = packet;
            client.MaxUploads = packet;
            client.CurrentQueued = packet;
            client.Age = packet;
            client.Sex = packet;
            client.Country = packet;
            client.Region = packet;

            if (Events.Joining(client))
            {
                client.SendPacket(TCPOutbound.Ack(client));
                client.LoggedIn = true;
                Events.Joined(client);
            }
        }

    }
}
