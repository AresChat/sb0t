using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace iconnect
{
    /// <summary>Link leaf</summary>
    public interface ILeaf
    {
        /// <summary>Leaf identification</summary>
        uint Ident { get; }
        /// <summary>Leaf name</summary>
        String Name { get; }
        /// <summary>Leaf ip address</summary>
        IPAddress ExternalIP { get; }
        /// <summary>Leaf port</summary>
        ushort Port { get; }
        /// <summary>Perform action on the user collection for this leaf</summary>
        void ForEachUser(Action<IUser> action);
    }
}
