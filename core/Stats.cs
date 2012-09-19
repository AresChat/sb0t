using System;
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
        }
    }
}
