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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace core
{
    public class Avatars
    {
        private static byte[] server_avatar { get; set; }
        private static byte[] default_avatar { get; set; }

        public static void UpdateServerAvatar(byte[] data)
        {
            server_avatar = Scale(data);

            if (UserPool.AUsers != null)
                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.BotAvatar(x, server_avatar)),
                    x => x.LoggedIn);
        }

        public static void UpdateDefaultAvatar(byte[] data)
        {
            default_avatar = Scale(data);

            if (UserPool.AUsers != null)
                UserPool.AUsers.ForEachWhere(x => x.Avatar = default_avatar, x => x.DefaultAvatar);
        }

        internal static byte[] Server(AresClient client)
        {
            if (server_avatar == null)
                return TCPOutbound.BotAvatarCleared(client);
            else
                return TCPOutbound.BotAvatar(client, server_avatar);
        }

        internal static bool GotServerAvatar
        {
            get { return server_avatar != null; }
        }

        internal static byte[] Server(ib0t.ib0tClient client)
        {
            if (server_avatar == null)
                return ib0t.WebOutbound.AvatarClearTo(client, Settings.Get<String>("bot"));
            else
                return ib0t.WebOutbound.AvatarTo(client, Settings.Get<String>("bot"), server_avatar);
        }

        public static void CheckAvatars(ulong time)
        {
            if (default_avatar == null)
                return;

            UserPool.AUsers.ForEachWhere(x =>
            {
                x.Avatar = default_avatar;
                x.OrgAvatar = default_avatar;
                x.AvatarReceived = true;
                x.DefaultAvatar = true;
            },
            x => !x.AvatarReceived &&
                 x.LoggedIn &&
                 time > (x.AvatarTimeout + 10000));
        }

        private static byte[] Default
        {
            get
            {
                if (default_avatar == null)
                    return new byte[] { };
                else
                    return default_avatar;
            }
        }

        private static byte[] Scale(byte[] raw)
        {
            byte[] result;

            using (MemoryStream raw_ms = new MemoryStream(raw))
            using (Bitmap raw_bmp = new Bitmap(raw_ms))
            using (Bitmap sized = new Bitmap(48, 48))
            using (Graphics g = Graphics.FromImage(sized))
            {
                using (SolidBrush sb = new SolidBrush(Color.White))
                    g.FillRectangle(sb, new Rectangle(0, 0, 48, 48));

                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.DrawImage(raw_bmp, new Rectangle(0, 0, 48, 48));
                ImageCodecInfo info = new List<ImageCodecInfo>(ImageCodecInfo.GetImageEncoders()).Find(x => x.MimeType == "image/jpeg");
                EncoderParameters encoding = new EncoderParameters();
                encoding.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 69L);

                using (MemoryStream ms = new MemoryStream())
                {
                    sized.Save(ms, info, encoding);
                    result = ms.ToArray();
                }
            }

            return result;
        }
    }
}
