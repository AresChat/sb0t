using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.ib0t
{
    class WebOutbound
    {
        public static byte[] AckTo(ib0tClient userobj, String name)
        {
            return WebSockets.Html5TextPacket("ACK:" + name.Length + ":" + name, userobj.WebCredentials.OldProto);
        }

        public static byte[] PublicTo(ib0tClient userobj, String name, String text)
        {
            String str = text;

            if (str.Length > 300)
                str = str.Substring(0, 300);

            return WebSockets.Html5TextPacket("PUBLIC:" + name.Length + "," + str.Length + ":" + name + str, userobj.WebCredentials.OldProto);
        }

        public static byte[] EmoteTo(ib0tClient userobj, String name, String text)
        {
            String str = text;

            if (str.Length > 300)
                str = str.Substring(0, 300);

            return WebSockets.Html5TextPacket("EMOTE:" + name.Length + "," + str.Length + ":" + name + str, userobj.WebCredentials.OldProto);
        }

        public static byte[] NoSuchTo(ib0tClient userobj, String text)
        {
            String str = text;

            if (str.Length > 300)
                str = str.Substring(0, 300);

            return WebSockets.Html5TextPacket("NOSUCH:" + str.Length + ":" + str, userobj.WebCredentials.OldProto);
        }

        public static byte[] ScribbleHead(ib0tClient userobj, int count, String height)
        {
            return WebSockets.Html5TextPacket("SCRIBBLE_HEAD:" + count + "," + height, userobj.WebCredentials.OldProto);
        }

        public static byte[] ScribbleBlock(ib0tClient userobj, String text)
        {
            return WebSockets.Html5TextPacket("SCRIBBLE_BLOCK:" + text, userobj.WebCredentials.OldProto);
        }

        public static byte[] UpdateTo(ib0tClient userobj, String name, Level level)
        {
            return WebSockets.Html5TextPacket("UPDATE:" + name.Length + ",1:" + name + ((byte)level), userobj.WebCredentials.OldProto);
        }

        public static byte[] UserlistItemTo(ib0tClient userobj, String name, Level level)
        {
            return WebSockets.Html5TextPacket("USERLIST:" + name.Length + ",1:" + name + ((byte)level), userobj.WebCredentials.OldProto);
        }

        public static byte[] UserlistEndTo(ib0tClient userobj)
        {
            return WebSockets.Html5TextPacket("USERLIST_END:", userobj.WebCredentials.OldProto);
        }

        public static byte[] JoinTo(ib0tClient userobj, String name, Level level)
        {
            return WebSockets.Html5TextPacket("JOIN:" + name.Length + ",1:" + name + ((byte)level), userobj.WebCredentials.OldProto);
        }

        public static byte[] PartTo(ib0tClient userobj, String name)
        {
            return WebSockets.Html5TextPacket("PART:" + name.Length + ":" + name, userobj.WebCredentials.OldProto);
        }

        public static byte[] FontTo(ib0tClient userobj, String name, int col1, int col2)
        {
            return WebSockets.Html5TextPacket("FONT:" + name.Length + "," + col1.ToString().Length + "," + col2.ToString().Length + ":" + name + col1.ToString() + col2.ToString(), userobj.WebCredentials.OldProto);
        }

        public static byte[] UrlTo(ib0tClient userobj, String addr, String tag)
        {
            return WebSockets.Html5TextPacket("URL:" + addr.Length + "," + tag.Length + ":" + addr + tag, userobj.WebCredentials.OldProto);
        }

        public static byte[] TopicTo(ib0tClient userobj, String text)
        {
            return WebSockets.Html5TextPacket("TOPIC:" + text.Length + ":" + text, userobj.WebCredentials.OldProto);
        }

        public static byte[] TopicFirstTo(ib0tClient userobj, String text)
        {
            return WebSockets.Html5TextPacket("TOPIC_FIRST:" + text.Length + ":" + text, userobj.WebCredentials.OldProto);
        }
    }
}
