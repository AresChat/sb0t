using System;
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

            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.BotAvatar(x, server_avatar)),
                x => x.LoggedIn);
        }

        public static void UpdateDefaultAvatar(byte[] data)
        {
            default_avatar = Scale(data);
        }

        internal static byte[] Server(AresClient client)
        {
            if (server_avatar == null)
                return TCPOutbound.BotAvatarCleared(client);
            else
                return TCPOutbound.BotAvatar(client, server_avatar);
        }

        public static void CheckAvatars(ulong time)
        {
            if (default_avatar == null)
                return;

            UserPool.AUsers.ForEachWhere(x => { x.Avatar = default_avatar; x.AvatarReceived = true; },
                x => !x.AvatarReceived && time > (x.AvatarTimeout + 10000));
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
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.DrawImage(raw_bmp, new Rectangle(0, 0, 48, 48));
                ImageCodecInfo info = new List<ImageCodecInfo>(ImageCodecInfo.GetImageEncoders()).Find(x => x.MimeType == "image/jpeg");
                EncoderParameters encoding = new EncoderParameters();
                encoding.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

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
