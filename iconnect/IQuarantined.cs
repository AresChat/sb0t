using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace iconnect
{
    /// <summary>Quarantined user</summary>
    public interface IQuarantined
    {
        /// <summary>Name</summary>
        String Name { get; }
        /// <summary>Guid</summary>
        Guid Guid { get; }
        /// <summary>External IP Address</summary>
        IPAddress ExternalIP { get; }
        /// <summary>Release this user from quarantine</summary>
        void Release();
    }
}
