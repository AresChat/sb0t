using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    /// <summary>Chatroom Statistics</summary>
    public interface IStats
    {
        /// <summary>Get total byte count of data received by this server</summary>
        ulong DataReceived { get; }
        /// <summary>Get total byte count of data sent by this server</summary>
        ulong DataSent { get; }
        /// <summary>Get the total number of public messages received by this server</summary>
        uint PublicMessages { get; }
        /// <summary>Get the total number of private messages received by this server</summary>
        uint PrivateMessages { get; }
        /// <summary>Get the total number of invalid login attempts received by this server</summary>
        uint InvalidLoginAttempts { get; }
        /// <summary>Get the total number of users who have flooded out of this server</summary>
        uint FloodCount { get; }
        /// <summary>Get the total number of users who were not allowed to join this server</summary>
        uint RejectionCount { get; }
        /// <summary>Get the total number of users who have joined this server</summary>
        uint JoinCount { get; }
        /// <summary>Get the total number of users who have parted from this server</summary>
        uint PartCount { get; }
        /// <summary>Get the peak user count so far during this session for this server</summary>
        uint PeakUserCount { get; }
        /// <summary>Get the current user count for this server</summary>
        uint CurrentUserCount { get; }
    }
}
