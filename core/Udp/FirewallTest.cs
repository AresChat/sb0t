using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace core.Udp
{
    class FirewallTest
    {
        public EndPoint EndPoint { get; set; }
        public ulong Time { get; set; }
        public uint Cookie { get; set; }
        public Socket Sock { get; private set; }
        public bool Completed { get; private set; }

        public void Start()
        {
            this.Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Sock.Blocking = false;

            try
            {
                this.Sock.Connect(this.EndPoint);
            }
            catch { }
        }

        public void Stop()
        {
            try { this.Sock.Disconnect(false); }
            catch { }
            try { this.Sock.Shutdown(SocketShutdown.Both); }
            catch { }
            try { this.Sock.Close(); }
            catch { }
            try { this.Sock.Dispose(); }
            catch { }
            try { this.Sock = null; }
            catch { }

            this.Completed = true;
        }
    }
}
