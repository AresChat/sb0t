using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    /// <summary>User Link Credentials</summary>
    public interface ILink
    {
        /// <summary>Get linked client status</summary>
        bool IsLinked { get; }
        /// <summary>Get leaf ident</summary>
        uint Ident { get; }
        /// <summary>Get status of link user's visibility on the user list</summary>
        bool Visible { get; }
    }
}
