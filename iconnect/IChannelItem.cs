using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace iconnect
{
    /// <summary>Channel search result</summary>    
    public interface IChannelItem
    {
        /// <summary>Name</summary>
        String Name { get; }
        /// <summary>Topic</summary>
        String Topic { get; }
        /// <summary>Version</summary>
        String Version { get; }
        /// <summary>User count</summary>
        ushort Users { get; }
        /// <summary>Port</summary>
        ushort Port { get; }
        /// <summary>IP Address</summary>
        IPAddress IP { get; }
        /// <summary>Language Code</summary>
        byte Language { get; }
    }
}
