using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.LinkHub
{
    class LeafProcessor
    {
        public static void Eval(Leaf leaf, LinkMsg msg, TCPPacketReader packet, ulong time, LinkMode mode)
        {
            switch (msg)
            {
                case LinkMsg.MSG_LINK_LEAF_AVATAR:
                    break;

                case LinkMsg.MSG_LINK_LEAF_CUSTOM_NAME:
                    break;

                case LinkMsg.MSG_LINK_LEAF_EMOTE_TEXT:
                    break;

                case LinkMsg.MSG_LINK_LEAF_LOGIN:
                    LeafLogin(leaf, packet, time, mode);
                    break;

                case LinkMsg.MSG_LINK_LEAF_PERSONAL_MESSAGE:
                    break;

                case LinkMsg.MSG_LINK_LEAF_PRIVATE_IGNORED:
                    break;

                case LinkMsg.MSG_LINK_LEAF_PRIVATE_TEXT:
                    break;

                case LinkMsg.MSG_LINK_LEAF_PUBLIC_TEXT:
                    break;

                case LinkMsg.MSG_LINK_LEAF_USERLIST_END:
                    break;

                case LinkMsg.MSG_LINK_LEAF_USERLIST_ITEM:
                    break;
            }
        }

        private static void LeafLogin(Leaf leaf, TCPPacketReader packet, ulong time, LinkMode mode)
        {
            
        }
    }
}
