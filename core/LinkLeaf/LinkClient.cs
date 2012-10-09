using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace core.LinkLeaf
{
    class LinkClient
    {
        private Socket Sock { get; set; }
        private bool CanReconnect { get; set; }

        private List<byte> data_in = new List<byte>();
        private ConcurrentQueue<byte[]> data_out = new ConcurrentQueue<byte[]>();
        private int socket_health = 0;

        public IPAddress ExternalIP { get; set; }
        public ushort Port { get; set; }
        public bool Busy { get; set; }
        public ulong Time { get; set; }
        public ulong LastPong { get; set; }
        public LinkLogin LoginPhase { get; set; }
        public uint Ident { get; set; }
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
        public bool Local { get; set; }
        public List<Leaf> Leaves { get; set; }

        private void ClearUserlist()
        {

        }

        public void Service(ulong time)
        {
            if (!this.Busy)
                return;

            if (this.LoginPhase == LinkLogin.Sleeping)
            {
                if (time > (this.Time + 30000))
                {
                    this.LoginPhase = LinkLogin.Connecting;
                    this.Time = time;
                    this.Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    this.Sock.Blocking = false;

                    try
                    {
                        this.Sock.Connect(new IPEndPoint(this.ExternalIP, this.Port));
                    }
                    catch { }
                }

                return;
            }

            if (this.LoginPhase == LinkLogin.Connecting)
            {
                if (this.Sock.Poll(0, SelectMode.SelectWrite))
                {
                    this.LoginPhase = LinkLogin.AwaitingAck;
                    this.SendPacket(LeafOutbound.LeafLogin());
                    this.Time = time;
                    this.LastPong = time;
                }
                else if (time > (this.Time + 15000))
                {
                    this.Disconnect();

                    if (this.CanReconnect)
                    {
                        this.Time = time;
                        this.LoginPhase = LinkLogin.Sleeping;
                    }
                    else this.Busy = false;

                    Events.LinkError(LinkError.UnableToConnect);
                }
            }
            else
            {
                core.LinkHub.LinkPacket packet = null;

                while ((packet = this.NextReceivedPacket) != null && this.SocketConnected)
                    try
                    {
                        LeafProcessor.Eval(this, packet.Msg, packet.Packet, time);
                    }
                    catch (Exception e)
                    {
                        this.Disconnect();

                        if (this.CanReconnect)
                        {
                            this.Time = time;
                            this.LoginPhase = LinkLogin.Sleeping;
                        }
                        else this.Busy = false;

                        Events.LinkError(LinkError.BadProtocol);
                        ServerCore.Log("packet read fail from hub " + packet.Msg, e);

                        if (this.LoginPhase == LinkLogin.Ready)
                            this.ClearUserlist();

                        return;
                    }

                this.SendReceive();

                if (!this.SocketConnected)
                {
                    this.Disconnect();

                    if (this.CanReconnect)
                    {
                        this.Time = time;
                        this.LoginPhase = LinkLogin.Sleeping;
                    }
                    else this.Busy = false;
                    
                    Events.LinkError(LinkError.RemoteDisconnect);

                    if (this.LoginPhase == LinkLogin.Ready)
                        this.ClearUserlist();
                }
                else if (this.LoginPhase == LinkLogin.AwaitingAck)
                {
                    if (time > (this.Time + 60000))
                    {
                        this.Disconnect();

                        if (this.CanReconnect)
                        {
                            this.Time = time;
                            this.LoginPhase = LinkLogin.Sleeping;
                        }
                        else this.Busy = false;

                        Events.LinkError(LinkError.HandshakeTimeout);
                    }
                }
                else if (this.LoginPhase == LinkLogin.Ready)
                {
                    if (time > (this.Time + 60000))
                    {
                        this.Time = time;
                        this.SendPacket(LeafOutbound.LeafPing());
                    }

                    if (time > (this.LastPong + 240000))
                    {
                        this.Disconnect();
                        
                        if (this.CanReconnect)
                        {
                            this.Time = time;
                            this.LoginPhase = LinkLogin.Sleeping;
                        }
                        else this.Busy = false;

                        Events.LinkError(LinkError.PingTimeout);
                        this.ClearUserlist();
                    }
                }
            }
        }

        private void SendReceive()
        {
            while (this.data_out.Count > 0)
            {
                try
                {
                    byte[] packet;

                    if (this.data_out.TryPeek(out packet))
                    {
                        this.Sock.Send(packet);

                        if (!this.Local)
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

                if (!this.Local)
                    Stats.DataReceived += (ulong)received;
            }
        }

        public void ConnectLocal()
        {
            this.CanReconnect = Settings.Get<bool>("link_reconnect");
            this.LoginPhase = LinkLogin.Connecting;
            this.Time = core.Time.Now;
            this.Busy = true;
            this.Local = true;
            this.ExternalIP = IPAddress.Loopback;
            this.Port = Settings.Port;
            this.Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Sock.Blocking = false;

            try
            {
                this.Sock.Connect(new IPEndPoint(this.ExternalIP, this.Port));
            }
            catch { }
        }

        public bool Connect(String hashlink)
        {
            if (this.Busy)
                return false;

            this.CanReconnect = Settings.Get<bool>("link_reconnect");
            this.Busy = true;
            this.Local = false;
            return true;
        }

        public void Disconnect()
        {
            if (this.Sock != null)
            {
                while (this.data_out.Count > 0)
                {
                    try
                    {
                        byte[] packet;

                        if (this.data_out.TryPeek(out packet))
                        {
                            this.Sock.Send(packet);

                            if (!this.Local)
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
                try { this.Sock = null; }
                catch { }
            }
        }

        public void EndSession()
        {
            this.Disconnect();
            this.Busy = false;
            this.ClearUserlist();
        }

        private bool SocketConnected
        {
            get { return this.socket_health < 10; }
        }

        public void SendPacket(byte[] packet)
        {
            this.data_out.Enqueue(packet);
        }

        private core.LinkHub.LinkPacket NextReceivedPacket
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
                    core.LinkHub.LinkPacket packet = new core.LinkHub.LinkPacket();
                    packet.Msg = (core.LinkHub.LinkMsg)data[2];
                    packet.Packet = new TCPPacketReader(data.Skip(3).ToArray());
                    return packet;
                }

                return null;
            }
        }
    }
}
