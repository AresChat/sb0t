using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    /// <summary>Script Credentials</summary>
    public interface IScripting
    {
        /// <summary>Get script level</summary>
        byte ScriptLevel { get; }
        /// <summary>Get script in room capability setting</summary>
        bool ScriptInRoom { get; }
        /// <summary>Get script capability setting</summary>
        bool ScriptEnabled { get; }
    }
}
