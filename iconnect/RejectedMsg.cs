using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    /// <summary>Rejection Messages</summary>
    public enum RejectedMsg
    {
        /// <summary>Too many clients from this IP address are attempting to join</summary>
        TooManyClients,
        /// <summary>Wait a few seconds between join attempts</summary>
        TooSoon,
        /// <summary>Use a different name that isn't being used by someone else</summary>
        NameInUse,
        /// <summary>You are banned from this chatroom</summary>
        Banned,
        /// <summary>You are too young to use this chatroom</summary>
        UnderAge,
        /// <summary>Your gender is not accepted in this chatroom</summary>
        UnacceptableGender,
        /// <summary>An extension or script is preventing you from joining</summary>
        UserDefined
    }
}
