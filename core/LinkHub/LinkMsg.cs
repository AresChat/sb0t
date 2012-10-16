using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.LinkHub
{
    enum LinkMsg : byte
    {
        // linking
        MSG_LINK_ERROR = 0,
        MSG_LINK_LEAF_LOGIN = 1,
        MSG_LINK_HUB_ACK = 3,
        MSG_LINK_HUB_LEAF_CONNECTED = 5,
        MSG_LINK_HUB_LEAF_DISCONNECTED = 6,
        MSG_LINK_LEAF_PING = 7,
        MSG_LINK_HUB_PONG = 8,

        // user pool
        MSG_LINK_LEAF_USERLIST_ITEM = 10,
        MSG_LINK_HUB_USERLIST_ITEM = 10,
        MSG_LINK_LEAF_AVATAR = 11,
        MSG_LINK_HUB_AVATAR = 11,
        MSG_LINK_LEAF_PERSONAL_MESSAGE = 12,
        MSG_LINK_HUB_PERSONAL_MESSAGE = 12,
        MSG_LINK_LEAF_USERLIST_END = 14,
        MSG_LINK_LEAF_JOIN = 15,
        MSG_LINK_LEAF_PART = 16,
        MSG_LINK_HUB_PART = 16,
        MSG_LINK_LEAF_USER_UPDATED = 18,
        MSG_LINK_HUB_USER_UPDATED = 18,
        MSG_LINK_LEAF_CUSTOM_NAME = 19,
        MSG_LINK_HUB_CUSTOM_NAME = 19,

        // text
        MSG_LINK_LEAF_PUBLIC_TEXT = 20,
        MSG_LINK_HUB_PUBLIC_TEXT = 20,
        MSG_LINK_LEAF_EMOTE_TEXT = 21,
        MSG_LINK_HUB_EMOTE_TEXT = 21,
        MSG_LINK_LEAF_PRIVATE_TEXT = 25,
        MSG_LINK_HUB_PRIVATE_TEXT = 25,
        MSG_LINK_LEAF_PRIVATE_IGNORED = 27,
        MSG_LINK_HUB_PRIVATE_IGNORED = 27,

        // custom data
        MSG_LINK_LEAF_CUSTOM_DATA_TO = 30,
        MSG_LINK_HUB_CUSTOM_DATA_TO = 30,

        // admin
        MSG_LINK_LEAF_NICK_CHANGED = 40,
        MSG_LINK_HUB_NICK_CHANGED = 40,
        MSG_LINK_LEAF_VROOM_CHANGED = 41,
        MSG_LINK_HUB_VROOM_CHANGED = 41,
        MSG_LINK_LEAF_IUSER = 42,
        MSG_LINK_HUB_IUSER = 42,
        MSG_LINK_LEAF_ADMIN = 43,
        MSG_LINK_HUB_ADMIN = 43,
        MSG_LINK_LEAF_IUSER_BIN = 44,
        MSG_LINK_HUB_IUSER_BIN = 44,
        MSG_LINK_LEAF_NO_ADMIN = 45,
        MSG_LINK_HUB_NO_ADMIN = 45,

        // file browse
        MSG_LINK_LEAF_BROWSE = 50,
        MSG_LINK_HUB_BROWSE = 50,
        MSG_LINK_LEAF_BROWSE_DATA = 51,
        MSG_LINK_HUB_BROWSE_DATA = 51
    }
}
