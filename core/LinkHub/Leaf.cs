using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace core.LinkHub
{
    class Leaf
    {
        public Socket Sock { get; set; }
        public IPAddress ExternalIP { get; set; }
        public bool LoggedIn { get; set; }
        public uint Ident { get; set; }
        public ulong Time { get; set; }

        private List<byte> data_in = new List<byte>();
        private ConcurrentQueue<byte[]> data_out = new ConcurrentQueue<byte[]>();
        private int socket_health = 0;

        public Leaf(Socket sock, ulong time, byte[] buffer)
        {
            this.Sock = sock;
            this.ExternalIP = ((IPEndPoint)sock.RemoteEndPoint).Address;
            this.Ident = LeafPool.NextIdent;
            this.Time = time;
            data_in.AddRange(BitConverter.GetBytes((ushort)buffer.Length));
            data_in.Add((byte)TCPMsg.MSG_LINK_PROTO);
            data_in.AddRange(buffer);
        }

        public bool SocketConnected
        {
            get { return this.socket_health < 10; }
            set { this.socket_health = value ? 0 : 10; }
        }

        public void SendReceive()
        {
            while (this.data_out.Count > 0)
            {
                try
                {
                    byte[] packet;

                    if (this.data_out.TryPeek(out packet))
                    {
                        this.Sock.Send(packet);
                        Stats.DataSent += (ulong)packet.Length;

                        while (!this.data_out.TryDequeue(out packet))
                            continue;
                    }
                    else break;
                }
                catch { break; }
            }

            byte[] buffer = new byte[8192];
            int received = 0;
            SocketError e = SocketError.Success;

            try { received = this.Sock.Receive(buffer, 0, buffer.Length, SocketFlags.None, out e); }
            catch { }

            if (received == 0)
                this.socket_health = e == SocketError.WouldBlock ? 0 : (this.socket_health + 1);
            else
            {
                this.socket_health = 0;
                this.data_in.AddRange(buffer.Take(received));
                Stats.DataReceived += (ulong)received;
            }
        }

        public LinkPacket NextReceivedPacket
        {
            get
            {
                if (this.data_in.Count < 3)
                    return null;

                ushort size = BitConverter.ToUInt16(this.data_in.ToArray(), 0);
                byte id = this.data_in[2];

                if (this.data_in.Count >= (size + 3))
                {
                    byte[] data = this.data_in.GetRange(3, size).ToArray();
                    this.data_in.RemoveRange(0, (size + 3));
                    LinkPacket packet = new LinkPacket();
                    packet.Msg = (LinkMsg)data[2];
                    packet.Packet = new TCPPacketReader(data.Skip(3).ToArray());
                    return packet;
                }

                return null;
            }
        }

        public void Disconnect()
        {
            while (this.data_out.Count > 0)
            {
                try
                {
                    byte[] packet;

                    if (this.data_out.TryPeek(out packet))
                    {
                        this.Sock.Send(packet);
                        Stats.DataSent += (ulong)packet.Length;

                        while (!this.data_out.TryDequeue(out packet))
                            continue;
                    }
                    else break;
                }
                catch { break; }
            }

            try { this.Sock.Disconnect(false); }
            catch { }
            try { this.Sock.Shutdown(SocketShutdown.Both); }
            catch { }
            try { this.Sock.Close(); }
            catch { }
            try { this.Sock.Dispose(); }
            catch { }

            this.SocketConnected = false;

            if (this.LoggedIn)
            {
                this.LoggedIn = false;
                // event
            }
        }

        public void EnforceRules(ulong time)
        {
            if ((!this.LoggedIn && time > (this.Time + 15000)) ||
                (this.LoggedIn && time > (this.Time + 240000)))
            {
                this.SocketConnected = false;
                ServerCore.Log("ping timeout or login timeout from " + this.ExternalIP + " id: " + this.Ident);
            }
        }

        public void SendPacket(byte[] packet)
        {
            this.data_out.Enqueue(packet);
        }
    }
}
