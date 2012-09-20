using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    /// <summary>Admin GUI Tab</summary>
    public interface ICommandDefault
    {
        /// <summary>Command Name</summary>
        String Name { get; }
        /// <summary>Command Default Level</summary>
        ILevel Level { get; }
    }
}
