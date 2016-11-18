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
        MSG_LINK_LEAF_PUBLIC_TO_USER = 28,
        MSG_LINK_HUB_PUBLIC_TO_USER = 28,
        MSG_LINK_LEAF_EMOTE_TO_USER = 29,
        MSG_LINK_HUB_EMOTE_TO_USER = 29,
        MSG_LINK_LEAF_PUBLIC_TO_LEAF = 90,
        MSG_LINK_HUB_PUBLIC_TO_LEAF = 90,
        MSG_LINK_LEAF_EMOTE_TO_LEAF = 91,
        MSG_LINK_HUB_EMOTE_TO_LEAF = 91,

        // custom data
        MSG_LINK_LEAF_CUSTOM_DATA_TO = 30,
        MSG_LINK_HUB_CUSTOM_DATA_TO = 30,
        MSG_LINK_LEAF_CUSTOM_DATA_ALL = 31,
        MSG_LINK_HUB_CUSTOM_DATA_ALL = 31,
        MSG_LINK_LEAF_NUDGE = 32,
        MSG_LINK_HUB_NUDGE = 32,
        MSG_LINK_LEAF_SCRIBBLE_USER = 33,
        MSG_LINK_HUB_SCRIBBLE_USER = 33,
        MSG_LINK_LEAF_SCRIBBLE_LEAF = 34,
        MSG_LINK_HUB_SCRIBBLE_LEAF = 34,

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
        MSG_LINK_HUB_BROWSE_DATA = 51,

        // print
        MSG_LINK_LEAF_PRINT_ALL = 60,
        MSG_LINK_HUB_PRINT_ALL = 60,
        MSG_LINK_LEAF_PRINT_VROOM = 61,
        MSG_LINK_HUB_PRINT_VROOM = 61,
        MSG_LINK_LEAF_PRINT_LEVEL = 62,
        MSG_LINK_HUB_PRINT_LEVEL = 62
    }
}
