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
using Jurassic;
using Jurassic.Library;

namespace scripting.Statics
{
    [JSEmbed(Name = "Stats")]
    class JSStats : ObjectInstance
    {
        public JSStats(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Stats"; }
        }

        [JSProperty(Name = "userCount")]
        public static double UserCount
        {
            get { return Server.Stats.CurrentUserCount; }
            set { }
        }

        [JSProperty(Name = "dataReceived")]
        public static double DataReceived
        {
            get { return Server.Stats.DataReceived; }
            set { }
        }

        [JSProperty(Name = "dataSent")]
        public static double DataSent
        {
            get { return Server.Stats.DataSent; }
            set { }
        }

        [JSProperty(Name = "floodCount")]
        public static double FloodCount
        {
            get { return Server.Stats.FloodCount; }
            set { }
        }

        [JSProperty(Name = "invalidLoginCount")]
        public static double InvalidLoginCount
        {
            get { return Server.Stats.InvalidLoginAttempts; }
            set { }
        }

        [JSProperty(Name = "joinCount")]
        public static double JoinCount
        {
            get { return Server.Stats.JoinCount; }
            set { }
        }

        [JSProperty(Name = "partCount")]
        public static double PartCount
        {
            get { return Server.Stats.PartCount; }
            set { }
        }

        [JSProperty(Name = "peakUserCount")]
        public static double PeakUserCount
        {
            get { return Server.Stats.PeakUserCount; }
            set { }
        }

        [JSProperty(Name = "rejectionCount")]
        public static double RejectionCount
        {
            get { return Server.Stats.RejectionCount; }
            set { }
        }

        [JSProperty(Name = "messageCount")]
        public static double MessageCount
        {
            get { return Server.Stats.PublicMessages; }
            set { }
        }

        [JSProperty(Name = "pmCount")]
        public static double PMCount
        {
            get { return Server.Stats.PrivateMessages; }
            set { }
        }
    }
}
