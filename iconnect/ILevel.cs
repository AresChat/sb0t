using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    /// <summary>Admin Levels</summary>
    public enum ILevel : byte
    {
        /// <summary>Regular</summary>
        Regular = 0,
        /// <summary>Moderator</summary>
        Moderator = 1,
        /// <summary>Administrator</summary>
        Administrator = 2,
        /// <summary>Host</summary>
        Host = 3
    }
}
