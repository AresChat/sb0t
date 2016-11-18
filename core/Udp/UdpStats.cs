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

namespace core.Udp
{
    class UdpStats
    {
        public static uint SENDINFO { get; set; }
        public static uint ACKINFO { get; set; }
        public static uint ADDIPS { get; set; }
        public static uint ACKIPS { get; set; }

        public static void Reset()
        {
            SENDINFO = 0;
            ACKINFO = 0;
            ADDIPS = 0;
            ACKIPS = 0;
        }

        public static void Record(UdpMsg msg)
        {
            switch (msg)
            {
                case UdpMsg.OP_SERVERLIST_ACKINFO:
                    ACKINFO++;
                    break;

                case UdpMsg.OP_SERVERLIST_ADDIPS:
                    ADDIPS++;
                    break;
            }
        }
    }
}
