using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    /// <summary>User password</summary>
    public interface IPassword
    {
        /// <summary>Name</summary>
        String Name { get; }
        /// <summary>Admin level</summary>
        ILevel Level { get; }
    }
}
