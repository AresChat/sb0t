using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.LinkHub
{
    enum LinkMsg : byte
    {
        MSG_LINK_LEAF_LOGIN = 1,
        MSG_LINK_HUB_ACK = 3,
        MSG_LINK_HUB_NEW_LEAF = 5,
        MSG_LINK_LEAF_USERLIST_ITEM = 10,
        MSG_LINK_HUB_USERLIST_ITEM = 10,
        MSG_LINK_LEAF_AVATAR = 11,
        MSG_LINK_HUB_AVATAR = 11,
        MSG_LINK_LEAF_PERSONAL_MESSAGE = 12,
        MSG_LINK_HUB_PERSONAL_MESSAGE = 12,
        MSG_LINK_LEAF_USERLIST_END = 14,
        MSG_LINK_LEAF_CUSTOM_NAME = 19,
        MSG_LINK_HUB_CUSTOM_NAME = 19,
        MSG_LINK_LEAF_PUBLIC_TEXT = 20,
        MSG_LINK_HUB_PUBLIC_TEXT = 20,
        MSG_LINK_LEAF_EMOTE_TEXT = 21,
        MSG_LINK_HUB_EMOTE_TEXT = 21,
        MSG_LINK_LEAF_PRIVATE_TEXT = 25,
        MSG_LINK_HUB_PRIVATE_TEXT = 25,
        MSG_LINK_LEAF_PRIVATE_IGNORED = 27,
        MSG_LINK_HUB_PRIVATE_IGNORED = 27
    }
}
