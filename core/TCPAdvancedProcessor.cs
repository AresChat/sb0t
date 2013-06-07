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
            if (client.Quarantined)
                return;

            packet.Packet.SkipBytes(2);
            TCPMsg msg = (TCPMsg)(byte)packet.Packet;

            switch (msg)
            {
                case TCPMsg.MSG_CHAT_CLIENT_CUSTOM_ADD_TAGS:
                    AddClientTag(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_CUSTOM_REM_TAGS:
                    RemClientTag(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_VC_SUPPORTED:
                    VCSupported(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_VC_FIRST:
                    VCFirst(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_VC_FIRST_TO:
                    VCFirstTo(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_VC_CHUNK:
                    VCChunk(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_VC_CHUNK_TO:
                    VCChunkTo(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_VC_IGNORE:
                    VCIgnore(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_CUSTOM_FONT:
                    Font(client, packet.Packet);
                    break;

                default:
                    Events.UnhandledProtocol(client, true, packet.Msg, packet.Packet, time);
                    break;
            }
        }

        private static void Font(AresClient client, TCPPacketReader packet)
        {
            if (packet.Remaining < 2)
            {
                client.Font = new AresFont();
                return;
            }

            AresFont font = new AresFont();
            font.Enabled = true;
            font.size = packet;
            font.FontName = packet.ReadString(client);
            font.oldN = packet;
            font.oldT = packet;

            if (packet.Remaining > 0)
                font.NameColor = packet.ReadString(client);

            if (packet.Remaining > 0)
                font.TextColor = packet.ReadString(client);

            if (String.IsNullOrEmpty(font.NameColor))
                font.NameColor = Helpers.AresColorToHTMLColor(font.oldN);

            if (String.IsNullOrEmpty(font.TextColor))
                font.TextColor = Helpers.AresColorToHTMLColor(font.oldT);

            client.Font = font;

            if (!client.Quarantined)
            {
                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.CustomFont(x, client)),
                    x => x.IsCbot && !x.Quarantined && x.LoggedIn && x.Vroom == client.Vroom);
                UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.FontTo(x, client.Name, font.oldN, font.oldT)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);
            }
        }

        private static void VCFirst(AresClient client, TCPPacketReader packet)
        {
            if (Settings.Get<bool>("voice"))
            {
                byte[] data = packet;

                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.VoiceChatFirst(x, client.Name, data)),
                    x => x.ID != client.ID && x.VoiceChatPublic && !x.Quarantined);
            }
        }

        private static void VCFirstTo(AresClient client, TCPPacketReader packet)
        {
            if (Settings.Get<bool>("voice"))
            {
                String name = packet.ReadString(client);
                AresClient target = UserPool.AUsers.Find(x => x.Name == name);
                byte[] data = packet;

                if (target != null)
                    if (!target.VoiceChatIgnoreList.Contains(client.Name))
                        if (target.VoiceChatPrivate)
                            target.SendPacket(TCPOutbound.VoiceChatFirstTo(target, client.Name, data));
                        else client.SendPacket(TCPOutbound.VoiceChatNoPrivate(client, name));
                    else client.SendPacket(TCPOutbound.VoiceChatIgnored(client, name));
                else client.SendPacket(TCPOutbound.OfflineUser(client, name));
            }
        }

        private static void VCChunk(AresClient client, TCPPacketReader packet)
        {
            if (Settings.Get<bool>("voice"))
            {
                byte[] data = packet;

                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.VoiceChatChunk(x, client.Name, data)),
                    x => x.ID != client.ID && x.VoiceChatPublic && !x.Quarantined);
            }
        }

        private static void VCChunkTo(AresClient client, TCPPacketReader packet)
        {
            if (Settings.Get<bool>("voice"))
            {
                String name = packet.ReadString(client);
                AresClient target = UserPool.AUsers.Find(x => x.Name == name);
                byte[] data = packet;

                if (target != null)
                    if (target.VoiceChatPrivate)
                        if (!target.VoiceChatIgnoreList.Contains(client.Name))
                            target.SendPacket(TCPOutbound.VoiceChatChunkTo(target, client.Name, data));
            }
        }

        private static void VCIgnore(AresClient client, TCPPacketReader packet)
        {
            String name = packet.ReadString(client);

            if (client.VoiceChatIgnoreList.Contains(name))
            {
                client.SendPacket(TCPOutbound.NoSuch(client, name + " Voice Chat Unignored"));
                client.VoiceChatIgnoreList.RemoveAll(x => x == name);
            }
            else
            {
                client.SendPacket(TCPOutbound.NoSuch(client, name + " Voice Chat Ignored"));
                client.VoiceChatIgnoreList.Add(name);
            }
        }

        private static void VCSupported(AresClient client, TCPPacketReader packet)
        {
            client.VoiceChatPublic = (byte)packet == 1;
            client.VoiceChatPrivate = (byte)packet == 1;

            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.VoiceChatUserSupport(x, client)),
                x => x.VoiceChatPrivate || x.VoiceChatPublic);
        }

        private static void AddClientTag(AresClient client, TCPPacketReader packet)
        {
            while (packet.Remaining > 0)
                client.CustomClientTags.Add(packet.ReadString(client));
        }

        private static void RemClientTag(AresClient client, TCPPacketReader packet)
        {
            while (packet.Remaining > 0)
            {
                String tag = packet.ReadString(client);
                client.CustomClientTags.RemoveAll(x => x == tag);
            }
        }
    }
}
