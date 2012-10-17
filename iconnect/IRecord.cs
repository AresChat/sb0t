using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace iconnect
{
    /// <summary>Historic user record</summary>
    public interface IRecord
    {
        /// <summary>Get joining timestamp</summary>
        uint JoinTime { get; }
        /// <summary>Get joining user name</summary>
        String Name { get; }
        /// <summary>Get external IP address</summary>
        IPAddress ExternalIP { get; }
        /// <summary>Get local IP address</summary>
        IPAddress LocalIP { get; }
        /// <summary>Get client version</summary>
        String Version { get; }
        /// <summary>Get data port</summary>
        ushort DataPort { get; }
        /// <summary>Get 16 byte GUID</summary>
        Guid Guid { get; }
        /// <summary>Get DNS host name</summary>
        String DNS { get; }
        /// <summary>Add this user to the ban list</summary>
        void Ban();
    }
}
