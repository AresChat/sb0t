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

namespace core
{
    class TCPOutbound
    {
        public static byte[] HTML(String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(text, false, true);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_HTML);
        }

        public static byte[] CustomFont(AresClient client, AresClient target)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            AresFont f = (AresFont)target.Font;

            packet.WriteString(client, target.Name); // user's name + null   
            packet.WriteByte(f.size);
            packet.WriteString(client, target.Font.FontName); // null terminated   
            packet.WriteByte(f.oldN);
            packet.WriteByte(f.oldT);
            packet.WriteString(client, target.Font.NameColor);
            packet.WriteString(client, target.Font.TextColor);

            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_CUSTOM_FONT); // id = 204   
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] Font(AresFont f)
        {
            StringBuilder sb = new StringBuilder("{");

            sb.Append("\"isEmote\":" + f.IsEmote.ToString().ToLower());

            if (!String.IsNullOrEmpty(f.NameColor))
                sb.Append(",\"nc\":\"" + f.NameColor + "\"");
            else
                sb.Append(",\"nc\":\"\"");

            if (!String.IsNullOrEmpty(f.TextColor))
                sb.Append(",\"tc\":\"" + f.TextColor + "\"");
            else
                sb.Append(",\"tc\":\"\"");

            if (!String.IsNullOrEmpty(f.FontName))
                sb.Append(",\"ff\":\"" + f.FontName + "\"");
            else
                sb.Append(",\"ff\":\"\"");

            sb.Append("}");

            String html = "<input type=\"hidden\" value=\"" + Uri.EscapeDataString(sb.ToString()) + "\" />";
            return HTML(html);
        }

        public static byte[] Ack(AresClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, client.Name);
            packet.WriteString(client, Settings.Get<String>("name"));
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_LOGIN_ACK);
        }

        public static byte[] NoSuch(AresClient client, String text)
        {
            String str = text;

            if (str.Length == 0)
                str = " ";

            while (Encoding.UTF8.GetByteCount(str) > 4000)
                str = str.Substring(0, str.Length - 1);

            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, str, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_NOSUCH);
        }

        public static byte[] Join(AresClient client, IClient target)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt16(target.FileCount);
            packet.WriteUInt32(0);
            packet.WriteIP(Settings.HideIps ? IPAddress.Parse("0.0.0.0") : client.Ares ? target.ExternalIP : IPAddress.Parse("0.0.0.0"));
            packet.WriteUInt16(Settings.HideIps ? (ushort)0 : target.DataPort);
            packet.WriteIP(target.NodeIP);
            packet.WriteUInt16(target.NodePort);
            packet.WriteByte(0);
            packet.WriteString(client, target.Name);
            packet.WriteIP(Settings.HideIps ? IPAddress.Parse("0.0.0.0") : client.Ares ? target.LocalIP : IPAddress.Parse("0.0.0.0"));
            packet.WriteByte((byte)(target.Browsable ? 1 : 0));
            packet.WriteByte((byte)target.Level);
            packet.WriteByte(target.Age);
            packet.WriteByte(target.Sex);
            packet.WriteByte(target.Country);
            packet.WriteString(client, target.Region);

            byte b = 0;

            if (target.SupportsHTML)
                b |= CLIENT_SUPPORTS_HTML;

            if (target is AresClient)
            {
                AresClient ac = (AresClient)target;

                if (ac != null)
                {
                    if (ac.VoiceChatPublic)
                        b |= CLIENT_SUPPORTS_VC;

                    if (ac.VoiceChatPrivate)
                        b |= CLIENT_SUPPORTS_PM_VC;

                    if (ac.VoiceOpusChatPublic)
                        b |= CLIENT_SUPPORTS_OPUS_VC;

                    if (ac.VoiceOpusChatPrivate)
                        b |= CLIENT_SUPPORTS_OPUS_PM_VC;
                }
            }

            packet.WriteByte(b);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_JOIN);
        }

        public static byte[] Part(AresClient client, IClient target)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, target.Name);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_PART);
        }

        private const int CLIENT_SUPPORTS_VC = 1;
        private const int CLIENT_SUPPORTS_PM_VC = 2;
        private const int CLIENT_SUPPORTS_OPUS_VC = 4;
        private const int CLIENT_SUPPORTS_OPUS_PM_VC = 8;
        private const int CLIENT_SUPPORTS_HTML = 16;

        public static byte[] Userlist(AresClient client, IClient target)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt16(target.FileCount);
            packet.WriteUInt32(0);
            packet.WriteIP(Settings.HideIps ? IPAddress.Parse("0.0.0.0") : client.Ares ? target.ExternalIP : IPAddress.Parse("0.0.0.0"));
            packet.WriteUInt16(Settings.HideIps ? (ushort)0 : target.DataPort);
            packet.WriteIP(target.NodeIP);
            packet.WriteUInt16(target.NodePort);
            packet.WriteByte(0);
            packet.WriteString(client, target.Name);
            packet.WriteIP(Settings.HideIps ? IPAddress.Parse("0.0.0.0") : client.Ares ? target.LocalIP : IPAddress.Parse("0.0.0.0"));
            packet.WriteByte((byte)(target.Browsable ? 1 : 0));
            packet.WriteByte((byte)target.Level);
            packet.WriteByte(target.Age);
            packet.WriteByte(target.Sex);
            packet.WriteByte(target.Country);
            packet.WriteString(client, target.Region);

            byte b = 0;

            if (target.SupportsHTML)
                b |= CLIENT_SUPPORTS_HTML;
            
            if (target is AresClient)
            {
                AresClient ac = (AresClient)target;

                if (ac != null)
                {
                    if (ac.VoiceChatPublic)
                        b |= CLIENT_SUPPORTS_VC;

                    if (ac.VoiceChatPrivate)
                        b |= CLIENT_SUPPORTS_PM_VC;

                    if (ac.VoiceOpusChatPublic)
                        b |= CLIENT_SUPPORTS_OPUS_VC;

                    if (ac.VoiceOpusChatPrivate)
                        b |= CLIENT_SUPPORTS_OPUS_PM_VC;
                }
            }

            packet.WriteByte(b);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_CHANNEL_USER_LIST);
        }

        public static byte[] UserlistBot(AresClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt16(0);
            packet.WriteUInt32(0);
            packet.WriteIP("0.0.0.0");
            packet.WriteUInt16(69);
            packet.WriteIP("0.0.0.0");
            packet.WriteUInt16(0);
            packet.WriteByte(0);
            packet.WriteString(client, Settings.Get<String>("bot"));
            packet.WriteIP("0.0.0.0");
            packet.WriteByte(1);
            packet.WriteByte(3);
            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteString(client, String.Empty);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_CHANNEL_USER_LIST);
        }

        public static byte[] UserListEnd()
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteByte(0);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_CHANNEL_USER_LIST_END);
        }

        public static byte[] TopicFirst(AresClient client, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, text, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_TOPIC_FIRST);
        }

        public static byte[] Topic(AresClient client, String text)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, text, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_TOPIC);
        }

        public static byte[] OpChange(AresClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteByte((byte)(client.Level > 0 ? 1 : 0));
            packet.WriteByte(0);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_OPCHANGE);
        }

        private enum ServerFeatures : byte
        {
            SERVER_SUPPORTS_PVT = 1,
            SERVER_SUPPORTS_SHARING = 2,
            SERVER_SUPPORTS_COMPRESSION = 4,
            SERVER_SUPPORTS_VC = 8,
            SERVER_SUPPORTS_OPUS_VC = 16,
            SERVER_SUPPORTS_ROOM_SCRIBBLES = 32,
            SERVER_SUPPORTS_PM_SCRIBBLES = 64,
            SERVER_SUPPORTS_HTML = 128
        }

        public static byte[] MyFeatures(AresClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, Settings.VERSION + " - " + Settings.RELEASE_URL);

            ServerFeatures flag = (ServerFeatures.SERVER_SUPPORTS_PVT |
                                   ServerFeatures.SERVER_SUPPORTS_SHARING |
                                   ServerFeatures.SERVER_SUPPORTS_COMPRESSION |
                                   ServerFeatures.SERVER_SUPPORTS_VC |
                                   ServerFeatures.SERVER_SUPPORTS_OPUS_VC |
                                   ServerFeatures.SERVER_SUPPORTS_PM_SCRIBBLES);

            if (client.SupportsHTML)
                flag |= ServerFeatures.SERVER_SUPPORTS_HTML;

            if (Settings.Get<bool>("can_room_scribble"))
                flag |= ServerFeatures.SERVER_SUPPORTS_ROOM_SCRIBBLES;

            packet.WriteByte((byte)flag);
            packet.WriteByte(63);
            packet.WriteByte(Settings.Get<byte>("language"));
            packet.WriteUInt32(client.Cookie);
            packet.WriteByte(1);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_MYFEATURES);
        }

        public static byte[] FavIcon()
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteBytes(Resource1.fi);
            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_FAVICON);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] Url(AresClient client, String addr, String tag)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, addr);
            packet.WriteString(client, tag);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_URL);
        }

        public static byte[] AvatarCleared(AresClient client, IClient target)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, target.Name);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_AVATAR);
        }

        public static byte[] Avatar(AresClient client, IClient target)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, target.Name);
            packet.WriteBytes(target.Avatar);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_AVATAR);
        }

        public static byte[] BotAvatarCleared(AresClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, Settings.Get<String>("bot"));
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_AVATAR);
        }

        public static byte[] BotAvatar(AresClient client, byte[] data)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, Settings.Get<String>("bot"));
            packet.WriteBytes(data);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_AVATAR);
        }

        public static byte[] PersonalMessage(AresClient client, IClient target)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, target.Name);
            packet.WriteString(client, target.PersonalMessage, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_PERSONAL_MESSAGE);
        }

        public static byte[] PersonalMessageBot(AresClient client)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, Settings.Get<String>("bot"));
            packet.WriteString(client, Settings.VERSION, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_PERSONAL_MESSAGE);
        }

        public static byte[] FastPing()
        {
            return new TCPPacketWriter().ToAresPacket(TCPMsg.MSG_CHAT_SERVER_FASTPING);
        }

        public static byte[] CryptoKey(AresClient client)
        {
            byte[] guid = client.Guid.ToByteArray();
            byte[] key = client.Encryption.IV.Concat(client.Encryption.Key).ToArray();

            for (int i = 0; i < guid.Length; i += 2)
                key = Crypto.e67(key, BitConverter.ToUInt16(guid, i));

            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteBytes(key);
            byte[] data = packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_CRYPTO_KEY);
            packet = new TCPPacketWriter();
            packet.WriteBytes(data);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] CustomData(IClient client, String sender, String ident, byte[] data)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, ident);
            packet.WriteString(client, sender);
            packet.WriteBytes(data);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_CUSTOM_DATA);
        }

        public static byte[] Public(AresClient client, String username, String text)
        {
            if (text.Length > 300)
                text = text.Substring(0, 300);

            while (Encoding.UTF8.GetByteCount(text) > 300)
                text = text.Substring(0, text.Length - 1);

            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, username);
            packet.WriteString(client, text, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_PUBLIC);
        }

        public static byte[] Emote(AresClient client, String username, String text)
        {
            if (text.Length > 300)
                text = text.Substring(0, 300);

            while (Encoding.UTF8.GetByteCount(text) > 300)
                text = text.Substring(0, text.Length - 1);

            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, username);
            packet.WriteString(client, text, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_EMOTE);
        }

        public static byte[] Private(IClient client, String username, String text)
        {
            if (text.Length > 300)
                text = text.Substring(0, 300);

            while (Encoding.UTF8.GetByteCount(text) > 300)
                text = text.Substring(0, text.Length - 1);

            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, username);
            packet.WriteString(client, text, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_PVT);
        }

        public static byte[] IsIgnoringYou(AresClient client, String name)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, name);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_ISIGNORINGYOU);
        }

        public static byte[] OfflineUser(AresClient client, String name)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, name);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_OFFLINEUSER);
        }

        public static byte[] SearchHit(AresClient client, ushort id, AresClient target, SharedFile file)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt16(id);
            packet.WriteByte((byte)file.Mime);
            packet.WriteUInt32(file.Size);
            packet.WriteBytes(file.Data);
            packet.WriteString(client, target.Name);
            packet.WriteIP(target.ExternalIP);
            packet.WriteUInt16(target.DataPort);
            packet.WriteIP(target.NodeIP);
            packet.WriteUInt16(target.NodePort);
            packet.WriteIP(target.LocalIP);
            packet.WriteByte(target.CurrentUploads);
            packet.WriteByte(target.MaxUploads);
            packet.WriteByte(target.CurrentQueued);
            packet.WriteByte(1);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_SEARCHHIT);
        }

        public static byte[] EndOfSearch(ushort id)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt16(id);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_ENDOFSEARCH);
        }

        public static byte[] ClientCompressed(byte[] data)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteBytes(Zip.Compress(data));
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENTCOMPRESSED);
        }

        public static byte[] EndOfBrowse(ushort id)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt16(id);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_ENDOFBROWSE);
        }

        public static byte[] BrowseError(ushort id)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt16(id);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_BROWSEERROR);
        }

        public static byte[] BrowseItem(ushort id, SharedFile file)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt16(id);
            packet.WriteByte((byte)file.Mime);
            packet.WriteUInt32(file.Size);
            packet.WriteBytes(file.Data);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_BROWSEITEM);
        }

        public static byte[] StartOfBrowse(ushort id, ushort count)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt16(id);
            packet.WriteUInt16(count);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_STARTOFBROWSE);
        }

        public static byte[] SuperNodes()
        {
            var linq = from x in UserPool.AUsers
                       where x.NodePort > 0 && (x.Version.StartsWith("Ares 2.") || x.Version.StartsWith("Ares_2."))
                       select new IPEndPoint(x.ExternalIP, x.DataPort);

            TCPPacketWriter packet = new TCPPacketWriter();

            if (linq.Count() > 0)
            {
                List<IPEndPoint> nodes = linq.ToList();
                nodes.Randomize();

                if (nodes.Count > 20)
                    nodes = nodes.GetRange(0, 20);

                foreach (IPEndPoint n in nodes)
                {
                    packet.WriteIP(n.Address);
                    packet.WriteUInt16((ushort)n.Port);
                }
            }

            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_HERE_SUPERNODES);
        }

        public static byte[] DirectChatPush(AresClient client, IClient target, byte[] cookie)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, target.Name);
            packet.WriteIP(target.ExternalIP);
            packet.WriteUInt16(target.DataPort);
            packet.WriteIP(target.LocalIP);
            packet.WriteBytes(cookie);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_DIRCHATPUSH);
        }

        public static byte[] SupportsVoiceClips()
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteByte((byte)(Settings.Get<bool>("voice") ? 1 : 0));
            packet.WriteByte(0);
            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_VC_SUPPORTED);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] VoiceChatUserSupport(AresClient client, AresClient target)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, target.Name);
            packet.WriteByte((byte)(target.VoiceChatPublic ? 1 : 0));
            packet.WriteByte((byte)(target.VoiceChatPrivate ? 1 : 0));
            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_VC_USER_SUPPORTED);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] VoiceChatFirst(AresClient client, String sender, byte[] buffer)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, sender);
            packet.WriteBytes(buffer);
            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_VC_FIRST);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] VoiceChatFirstTo(AresClient client, String sender, byte[] buffer)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, sender);
            packet.WriteBytes(buffer);
            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_VC_FIRST_FROM);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] VoiceChatIgnored(AresClient client, String sender)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, sender);
            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_VC_IGNORE);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] VoiceChatNoPrivate(AresClient client, String sender)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, sender);
            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_VC_NOPVT);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] VoiceChatChunk(AresClient client, String sender, byte[] buffer)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, sender);
            packet.WriteBytes(buffer);
            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_VC_CHUNK);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] VoiceChatChunkTo(AresClient client, String sender, byte[] buffer)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, sender);
            packet.WriteBytes(buffer);
            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_VC_CHUNK_FROM);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] UpdateUserStatus(AresClient client, IClient target)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(client, target.Name);
            packet.WriteUInt16(target.FileCount);
            packet.WriteByte((byte)(target.Browsable ? 1 : 0));
            packet.WriteIP(target.NodeIP);
            packet.WriteUInt16(target.NodePort);
            packet.WriteIP(Settings.HideIps ? IPAddress.Parse("0.0.0.0") : (client.Ares ? target.ExternalIP : IPAddress.Parse("0.0.0.0")));
            packet.WriteByte((byte)target.Level);
            packet.WriteByte(target.Age);
            packet.WriteByte(target.Sex);
            packet.WriteByte(target.Country);
            packet.WriteString(client, target.Region);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_UPDATE_USER_STATUS);
        }

        public static byte[] Redirect(AresClient client, Room room)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteIP(room.IP);
            packet.WriteUInt16(room.Port);
            packet.WriteIP(room.IP);
            packet.WriteString(client, room.Name);
            packet.WriteString(client, "Redirecting...");
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_SERVER_REDIRECT);
        }
    }
}
