using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace core.Udp
{
    class UdpListener
    {
        public IPEndPoint EndPoint { get; private set; }
        public Socket Sock { get; private set; }

        private Queue<UdpItem> data_out = new Queue<UdpItem>();
        private Queue<UdpItem> data_in = new Queue<UdpItem>();

        public UdpListener(IPEndPoint ep)
        {
            this.EndPoint = ep;
        }

        public void Start()
        {
            this.data_out.Clear();
            this.data_in.Clear();

            UdpStats.Reset();

            this.Sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.Sock.Blocking = false;
            this.Sock.Bind(this.EndPoint);
        }

        public void Stop()
        {
            try { this.Sock.Shutdown(SocketShutdown.Both); }
            catch { }
            try { this.Sock.Close(); }
            catch { }
            try { this.Sock.Dispose(); }
            catch { }
            try { this.Sock = null; }
            catch { }
        }

        public void SendDatagram(UdpItem item)
        {
            this.data_out.Enqueue(item);
        }

        private bool Pending
        {
            get { return this.Sock.Available > 0; }
        }

        private byte[] ReceiveFrom(out EndPoint ep)
        {
            ep = new IPEndPoint(IPAddress.Any, 0);
            byte[] buf = new byte[8192];
            int size = 0;

            try
            {
                size = this.Sock.ReceiveFrom(buf, ref ep);
            }
            catch { }

            return buf.Take(size).ToArray();
        }

        private bool SendTo(byte[] packet, EndPoint ep)
        {
            try
            {
                int size = this.Sock.SendTo(packet, ep);
                return packet.Length == size;
            }
            catch { }

            return false;
        }

        public void ServiceUdp(ulong time)
        {
            this.SendReceive();

            while (this.data_in.Count > 0)
                try
                {
                    UdpProcessor.Eval(this.data_in.Dequeue(), this, time);
                }
                catch { }

            // timers
        }

        private void SendReceive()
        {
            while (this.data_out.Count > 0)
            {
                UdpItem item = this.data_out.Peek();

                try
                {
                    int size = this.Sock.SendTo(item.Data, item.EndPoint);

                    if (size == item.Data.Length)
                    {
                        this.data_out.Dequeue();
                        UdpStats.Record(item.Msg);
                    }
                    else throw new Exception();
                }
                catch
                {
                    if (item.Attempts++ > 2)
                        this.data_out.Dequeue();
                }
            }

            EndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            byte[] buffer = new byte[8192];

            while (this.Pending)
            {
                try
                {
                    int size = this.Sock.ReceiveFrom(buffer, ref ep);

                    if (size > 0)
                    {
                        UdpItem item = new UdpItem();
                        item.Msg = (UdpMsg)buffer[0];
                        item.Data = buffer.Skip(1).Take(size).ToArray();
                        item.EndPoint = ep;
                        this.data_in.Enqueue(item);
                    }
                }
                catch { break; }
            }
        }

    }
}
