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
using iconnect;

namespace core.Extensions
{
    class ExStats : IStats
    {
        public ulong DataReceived
        {
            get { return Stats.DataReceived; }
        }

        public ulong DataSent
        {
            get { return Stats.DataSent; }
        }

        public uint PublicMessages
        {
            get { return Stats.PublicMessages; }
        }

        public uint PrivateMessages
        {
            get { return Stats.PrivateMessages; }
        }

        public uint InvalidLoginAttempts
        {
            get { return Stats.InvalidLoginAttempts; }
        }

        public uint FloodCount
        {
            get { return Stats.FloodCount; }
        }

        public uint RejectionCount
        {
            get { return Stats.RejectionCount; }
        }

        public uint JoinCount
        {
            get { return Stats.JoinCount; }
        }

        public uint PartCount
        {
            get { return Stats.PartCount; }
        }

        public uint PeakUserCount
        {
            get { return Stats.PeakUserCount; }
        }

        public uint CurrentUserCount
        {
            get { return Stats.CurrentUserCount; }
        }
    }
}
