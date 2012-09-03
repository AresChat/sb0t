using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace core
{
    class AresClient
    {
        public ushort ID { get; private set; }
        public IPAddress ExternalIP { get; set; }
        public String DNS { get; set; }
        public bool LoggedIn { get; set; }
        public uint Time { get; set; }
        public Guid Guid { get; set; }
        public ushort FileCount { get; set; }
        public ushort DataPort { get; set; }
        public IPAddress NodeIP { get; set; }
        public ushort NodePort { get; set; }
        public String Name { get; set; }
        public String Version { get; set; }
        public IPAddress LocalIP { get; set; }
        public bool Browsable { get; set; }
        public bool Compression { get; set; }
        public byte CurrentUploads { get; set; }
        public byte MaxUploads { get; set; }
        public byte CurrentQueued { get; set; }
        public byte Age { get; set; }
        public byte Sex { get; set; }
        public byte Country { get; set; }
        public String Region { get; set; }


        private Socket Sock { get; set; }
        private List<byte> data_in = new List<byte>();
        private ConcurrentQueue<byte[]> data_out = new ConcurrentQueue<byte[]>();
        private int socket_health = 0;

        public AresClient(Socket sock, uint time, ushort id)
        {
            this.ID = id;
            this.Sock = sock;
            this.Sock.Blocking = false;
            this.Time = time;
            this.ExternalIP = ((IPEndPoint)this.Sock.RemoteEndPoint).Address;
            Dns.BeginGetHostEntry(this.ExternalIP, new AsyncCallback(this.DnsReceived), null);
            ServerCore.Log(this.ID + " connects");
        }

        private void DnsReceived(IAsyncResult result)
        {
            try
            {
                IPHostEntry i = Dns.EndGetHostEntry(result);
                this.DNS = i.HostName;
                this.SendPacket(TCPOutbound.NoSuch("\x000302Hostname: " + this.DNS));
            }
            catch
            {
                try
                {
                    this.DNS = this.ExternalIP.ToString();
                    this.SendPacket(TCPOutbound.NoSuch("\x000302Hostname: " + this.DNS));
                }
                catch { }
            }
        }

        public bool SocketConnected
        {
            get { return this.socket_health < 10; }
            set { this.socket_health = value ? 0 : 10; }
        }

        public void SendPacket(byte[] packet)
        {
            this.data_out.Enqueue(packet);
        }

        public void SendReceive()
        {
            while (this.data_out.Count > 0)
            {
                try
                {
                    byte[] packet;
                    this.data_out.TryPeek(out packet);
                    this.Sock.Send(packet);
                    this.data_out.TryDequeue(out packet);
                }
                catch { break; }
            }

            byte[] buffer = new byte[8192];
            int received = 0;
            SocketError e = SocketError.Success;

            try { received = this.Sock.Receive(buffer, 0, this.Sock.Available, SocketFlags.None, out e); }
            catch { }

            if (received == 0)
                this.socket_health = e == SocketError.WouldBlock ? 0 : (this.socket_health + 1);
            else
            {
                this.socket_health = 0;
                this.data_in.AddRange(buffer.Take(received));
            }
        }

        public void EnforceRules(uint time)
        {
            if ((!this.LoggedIn && time > (this.Time + 15000)) ||
                (this.LoggedIn && time > (this.Time + 240000)))
            {
                this.SocketConnected = false;
                ServerCore.Log("ping timeout or login timeout from " + this.ExternalIP + " id: " + this.ID);
            }
        }

        public void Disconnect()
        {
            while (this.data_out.Count > 0)
            {
                try
                {
                    byte[] packet;
                    this.data_out.TryPeek(out packet);
                    this.Sock.Send(packet);
                    this.data_out.TryDequeue(out packet);
                }
                catch { break; }
            }

            try { this.Sock.Disconnect(false); }
            catch { }
            try { this.Sock.Shutdown(SocketShutdown.Both); }
            catch { }
            try { this.Sock.Close(); }
            catch { }

            this.SocketConnected = false;

            ServerCore.Log(this.ID + " disconnects");
        }

        public TCPPacket NextReceivedPacket
        {
            get
            {
                if (this.data_in.Count < 3)
                    return null;

                ushort size = BitConverter.ToUInt16(this.data_in.ToArray(), 0);
                byte id = this.data_in[2];

                if (this.data_in.Count >= (size + 3))
                {
                    TCPPacket packet = new TCPPacket();
                    packet.Msg = (TCPMsg)id;
                    packet.Packet = new TCPPacketReader(this.data_in.GetRange(3, size).ToArray());
                    this.data_in.RemoveRange(0, (size + 3));
                    return packet;
                }

                return null;
            }
        }

        public void InsertUnzippedData(byte[] data)
        {
            this.data_in.InsertRange(0, data);
        }
    }
}
