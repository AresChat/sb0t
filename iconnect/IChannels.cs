using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iconnect
{
    /// <summary>Channel search helper</summary>
    public interface IChannels
    {
        /// <summary>Action all channels</summary>
        void ForEach(Action<IChannelItem> action);
        /// <summary>Check if there are any channels available to search</summary>
        bool Available { get; }
        /// <summary>Check if room search is enabled</summary>
        bool Enabled { get; }
    }
}
