using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace iconnect
{
    /// <summary>Link hub</summary>
    public interface IHub
    {
        /// <summary>Connect to a hub</summary>
        void Connect(String hashlink);
        /// <summary>Disconnect from a hub</summary>
        bool Disconnect();
        /// <summary>Get status of connection to hub</summary>
        bool IsLinked { get; }
        /// <summary>Get hub name</summary>
        String Name { get; }
        /// <summary>Get hub ip address</summary>
        IPAddress ExternalIP { get; }
        /// <summary>Get hub port</summary>
        ushort Port { get; }
        /// <summary>Perform action on the leaf collection</summary>
        void ForEachLeaf(Action<ILeaf> action);
        /// <summary>Find a leaf based on a condition</summary>
        ILeaf FindLeaf(Predicate<ILeaf> predicate);
    }
}
