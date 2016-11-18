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
