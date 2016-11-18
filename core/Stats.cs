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
    class Stats
    {
        public static ulong DataReceived { get; set; }
        public static ulong DataSent { get; set; }
        public static uint PublicMessages { get; set; }
        public static uint PrivateMessages { get; set; }
        public static uint InvalidLoginAttempts { get; set; }
        public static uint FloodCount { get; set; }
        public static uint RejectionCount { get; set; }
        public static uint JoinCount { get; set; }
        public static uint PartCount { get; set; }
        public static uint PeakUserCount { get; set; }
        public static uint StartTime { get; private set; }

        public static uint CurrentUserCount
        {
            get
            {
                ushort result = 0;

                UserPool.AUsers.ForEachWhere(x => result++, x => x.LoggedIn);
                UserPool.WUsers.ForEachWhere(x => result++, x => x.LoggedIn);

                return result;
            }
        }

        public static void Reset()
        {
            DataReceived = 0;
            DataSent = 0;
            PublicMessages = 0;
            PrivateMessages = 0;
            InvalidLoginAttempts = 0;
            FloodCount = 0;
            RejectionCount = 0;
            JoinCount = 0;
            PartCount = 0;
            PeakUserCount = 0;
            StartTime = Helpers.UnixTime;
        }
    }
}
