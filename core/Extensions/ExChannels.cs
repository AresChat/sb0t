using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace core.Extensions
{
    class ExChannels : IChannels
    {
        public void ForEach(Action<IChannelItem> action)
        {
            Udp.UdpChannelList.ForEach(action);
        }

        public bool Available
        {
            get { return Udp.UdpChannelList.Available; }
        }

        public bool Enabled
        {
            get { return Settings.Get<bool>("roomsearch"); }
        }
    }
}
