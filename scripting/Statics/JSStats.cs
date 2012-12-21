using System;
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
