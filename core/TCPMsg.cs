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
    public enum TCPMsg : byte
    {
        MSG_CHAT_SERVER_ERROR = 0,
        MSG_CHAT_CLIENT_RELOGIN = 1,
        MSG_CHAT_CLIENT_LOGIN = 2,
        MSG_CHAT_SERVER_LOGIN_ACK = 3,
        MSG_CHAT_CLIENT_UPDATE_STATUS = 4,
        MSG_CHAT_SERVER_UPDATE_USER_STATUS = 5,
        MSG_CHAT_SERVER_REDIRECT = 6,
        MSG_CHAT_CLIENT_AUTOLOGIN = 7,
        MSG_SERVER_ECHO = 8,
        MSG_CHAT_CLIENT_AVATAR = 9,
        MSG_CHAT_SERVER_AVATAR = 9,
        MSG_CHAT_CLIENT_PUBLIC = 10,
        MSG_CHAT_SERVER_PUBLIC = 10,
        MSG_CHAT_CLIENT_EMOTE = 11,
        MSG_CHAT_SERVER_EMOTE = 11,
        MSG_CHAT_CLIENT_PERSONAL_MESSAGE = 13,
        MSG_CHAT_SERVER_PERSONAL_MESSAGE = 13,
        MSG_CHAT_CLIENT_FASTPING = 14,
        MSG_CHAT_SERVER_FASTPING = 14,
        MSG_CHAT_SERVER_JOIN = 20,
        MSG_CHAT_CLIENT_PVT = 25,
        MSG_CHAT_SERVER_PVT = 25,
        MSG_CHAT_SERVER_ISIGNORINGYOU = 26,
        MSG_CHAT_SERVER_OFFLINEUSER = 27,
        MSG_CHAT_SERVER_PART = 22,
        MSG_CHAT_SERVER_CHANNEL_USER_LIST = 30,
        MSG_CHAT_SERVER_TOPIC = 31,
        MSG_CHAT_SERVER_TOPIC_FIRST = 32,
        MSG_CHAT_SERVER_CHANNEL_USER_LIST_END = 35,
        MSG_CHAT_SERVER_HTML = 43,
        MSG_CHAT_SERVER_NOSUCH = 44,
        MSG_CHAT_CLIENT_IGNORELIST = 45,
        MSG_CHAT_CLIENT_ADDSHARE = 50,
        MSG_CHAT_CLIENT_REMSHARE = 51,
        MSG_CHAT_CLIENT_BROWSE = 52,
        MSG_CHAT_SERVER_ENDOFBROWSE = 53,
        MSG_CHAT_SERVER_BROWSEERROR = 54,
        MSG_CHAT_SERVER_BROWSEITEM = 55,
        MSG_CHAT_SERVER_STARTOFBROWSE = 56,
        MSG_CHAT_CLIENT_SEARCH = 60,
        MSG_CHAT_SERVER_SEARCHHIT = 61,
        MSG_CHAT_SERVER_ENDOFSEARCH = 62,
        MSG_CHAT_CLIENT_DUMMY = 64,
        MSG_CHAT_CLIENT_SEND_SUPERNODES = 70,
        MSG_CHAT_SERVER_HERE_SUPERNODES = 70,
        MSG_CHAT_CLIENT_DIRCHATPUSH = 72,
        MSG_CHAT_SERVER_URL = 73,
        MSG_CHAT_CLIENT_COMMAND = 74,
        MSG_CHAT_SERVER_OPCHANGE = 75,
        MSG_CHAT_CLIENTCOMPRESSED = 80,
        MSG_CHAT_CLIENT_AUTHLOGIN = 82,
        MSG_CHAT_CLIENT_AUTHREGISTER = 83,
        MSG_CHAT_SERVER_MYFEATURES = 92,

        MSG_CHAT_CLIENT_CUSTOM_DATA = 200,
        MSG_CHAT_SERVER_CUSTOM_DATA = 200,
        MSG_CHAT_CLIENT_CUSTOM_DATA_ALL = 201,
        MSG_CHAT_CLIENT_CUSTOM_ADD_TAGS = 202,
        MSG_CHAT_CLIENT_CUSTOM_REM_TAGS = 203,
        MSG_CHAT_SERVER_CUSTOM_FONT = 204,
        MSG_CHAT_CLIENT_CUSTOM_FONT = 204, 
        MSG_CHAT_SERVER_VC_SUPPORTED = 205,
        MSG_CHAT_CLIENT_VC_SUPPORTED = 205,
        MSG_CHAT_SERVER_VC_FIRST = 206,
        MSG_CHAT_CLIENT_VC_FIRST = 206,
        MSG_CHAT_SERVER_VC_FIRST_FROM = 207,
        MSG_CHAT_CLIENT_VC_FIRST_TO = 207,
        MSG_CHAT_SERVER_VC_CHUNK = 208,
        MSG_CHAT_CLIENT_VC_CHUNK = 208,
        MSG_CHAT_SERVER_VC_CHUNK_FROM = 209,
        MSG_CHAT_CLIENT_VC_CHUNK_TO = 209,
        MSG_CHAT_SERVER_VC_IGNORE = 210,
        MSG_CHAT_CLIENT_VC_IGNORE = 210,
        MSG_CHAT_SERVER_VC_NOPVT = 211,
        MSG_CHAT_SERVER_VC_USER_SUPPORTED = 212,
        MSG_CHAT_ADVANCED_FEATURES_PROTOCOL = 250,
        MSG_CHAT_SERVER_ROOM_SCRIBBLE = 225,
        MSG_CHAT_SERVER_CRYPTO_KEY = 230,
        MSG_CHAT_SERVER_FAVICON = 231,

        // new scribble-to-room (so that images can be trickled to avoid flooding)

        // 4 bytes = size
        // 2 bytes = number of chunks
        // x bytes = data
        MSG_CHAT_CLIENT_SCRIBBLEROOM_FIRST = 240,

        // x bytes = data
        MSG_CHAT_CLIENT_SCRIBBLEROOM_CHUNK = 241,

        // Cometseeker - Maybe use this in Ares Server? :-)
        // --- client will send after MSG_CHAT_SERVER_MYFEATURES is received.. if blocking is required!
        // --- client will send when requirement changes (for example if you have a check-box to toggle blocking)
        MSG_CHAT_CLIENT_BLOCK_CUSTOMNAMES = 242, // 1 byte -> 0=no 1=yes
        
        MSG_LINK_PROTO = 251
    }
}
