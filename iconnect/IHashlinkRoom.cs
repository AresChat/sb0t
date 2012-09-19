using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace iconnect
{
    /// <summary>Hashlink Room</summary>
    public interface IHashlinkRoom
    {
        /// <summary>Get or set name of room</summary>
        String Name { get; set; }
        /// <summary>Get or set external IP address of room</summary>
        IPAddress IP { get; set; }
        /// <summary>Get or set data port of room</summary>
        ushort Port { get; set; }
    }
}
