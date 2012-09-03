using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    class TCPOutbound
    {
        public static byte[] Ack(AresClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client.Name);
            packet.WriteString(Settings.Get<String>("name"));
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_LOGIN_ACK);
        }

        public static byte[] NoSuch(String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(text, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_NOSUCH);
        }
    }
}
