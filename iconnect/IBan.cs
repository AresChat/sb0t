using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace iconnect
{
    /// <summary>Banned User</summary>
    public interface IBan
    {
        /// <summary>Get user name</summary>
        String Name { get; }
        /// <summary>Get user version</summary>
        String Version { get; }
        /// <summary>Get user GUID</summary>
        Guid Guid { get; }
        /// <summary>Get user external IP address</summary>
        IPAddress ExternalIP { get; }
        /// <summary>Get user local IP address</summary>
        IPAddress LocalIP { get; }
        /// <summary>Get user data port</summary>
        ushort Port { get; }
        /// <summary>Unban this user</summary>
        void Unban();
    }
}
