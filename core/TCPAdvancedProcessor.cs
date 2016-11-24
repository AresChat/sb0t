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

                case TCPMsg.MSG_CHAT_CLIENT_SCRIBBLEROOM_FIRST:
                    ScribbleRoomFirst(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_SCRIBBLEROOM_CHUNK:
                    ScribbleRoomChunk(client, packet.Packet);
                    break;

                case TCPMsg.MSG_CHAT_CLIENT_BLOCK_CUSTOMNAMES:
                    client.BlockCustomNames = ((byte)packet.Packet) == 1;
                    break;

                default:
                    Events.UnhandledProtocol(client, true, packet.Msg, packet.Packet, time);
                    break;
            }
        }

        private static void ScribbleRoomFirst(AresClient client, TCPPacketReader packet)
        {
            if (!Settings.Get<bool>("can_room_scribble"))
                return;

            client.ScribbleRoomObject.Reset();
            client.ScribbleRoomObject.Size = packet;

            if (client.ScribbleRoomObject.Size > 200000)
            {
                client.ScribbleRoomObject.Reset();
                return;
            }

            if(!Events.CanScribble(client))
            {
                return;
            }

            client.ScribbleRoomObject.IsReceiving = true;
            client.ScribbleRoomObject.Chunks = packet;
            client.ScribbleRoomObject.DataIn.AddRange(((byte[])packet));

            if (client.ScribbleRoomObject.ChunkCount == client.ScribbleRoomObject.Chunks)
            {
                if (client.ScribbleRoomObject.ReceivedCount == client.ScribbleRoomObject.Size)
                {
                    byte[] img = client.ScribbleRoomObject.DataIn.ToArray();
                    String sender = Settings.Get<String>("bot");
                    int height = 123; // not important

                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.NoSuch(x, "\x000314--- From " + client.Name)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && x.CustomClient && !x.Quarantined && x.ID != client.ID);

                    UserPool.AUsers.ForEachWhere(x => x.Scribble(sender, img, height),
                        x => x.LoggedIn && x.Vroom == client.Vroom && x.CustomClient && !x.Quarantined && x.ID != client.ID && !x.IgnoreList.Contains(client.Name));
                }

                client.ScribbleRoomObject.Reset();
            }
        }

        private static void ScribbleRoomChunk(AresClient client, TCPPacketReader packet)
        {
            if (!Settings.Get<bool>("can_room_scribble"))
                return;
            
            if (client.ScribbleRoomObject.IsReceiving)
            {
                client.ScribbleRoomObject.ChunkCount++;
                client.ScribbleRoomObject.DataIn.AddRange(((byte[])packet));

                if (client.ScribbleRoomObject.ChunkCount == client.ScribbleRoomObject.Chunks)
                {
                    if (client.ScribbleRoomObject.ReceivedCount == client.ScribbleRoomObject.Size)
                    {
                        byte[] img = client.ScribbleRoomObject.DataIn.ToArray();
                        String sender = Settings.Get<String>("bot");
                        int height = 123; // not important

                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.NoSuch(x, "\x000314--- From " + client.Name)),
                            x => x.LoggedIn && x.Vroom == client.Vroom && x.CustomClient && !x.Quarantined && x.ID != client.ID && !x.IgnoreList.Contains(client.Name));

                        UserPool.AUsers.ForEachWhere(x => x.Scribble(sender, img, height),
                            x => x.LoggedIn && x.Vroom == client.Vroom && x.CustomClient && !x.Quarantined && x.ID != client.ID && !x.IgnoreList.Contains(client.Name));
                    }

                    client.ScribbleRoomObject.Reset();
                }
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

            // fonts cannot be bigger than 18
            if(font.size > 18)
            {
                font.size = 18;
            }

            if(!Settings.Get<bool>("fonts_enabled"))
            {
                return;
            }

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
                    x => x.ID != client.ID && x.VoiceChatPublic && !x.Quarantined && !x.VoiceChatIgnoreList.Contains(client.Name) && !x.IgnoreList.Contains(client.Name));
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
                    if (!target.VoiceChatIgnoreList.Contains(client.Name) && !target.IgnoreList.Contains(client.Name))
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
                    x => x.ID != client.ID && x.VoiceChatPublic && !x.Quarantined && !x.VoiceChatIgnoreList.Contains(client.Name) && !x.IgnoreList.Contains(client.Name));
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
                        if (!target.VoiceChatIgnoreList.Contains(client.Name) && !target.VoiceChatIgnoreList.Contains(client.Name))
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
