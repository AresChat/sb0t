using System;
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
