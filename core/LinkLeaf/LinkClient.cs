/*
    sb0t ares chat server
    Copyright (C) 2016  AresChat

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace core.LinkLeaf
{
    class LinkClient : iconnect.IHub
    {
        private Socket Sock { get; set; }
        private bool CanReconnect { get; set; }

        private List<byte> data_in = new List<byte>();
        private ConcurrentQueue<byte[]> data_out = new ConcurrentQueue<byte[]>();
        private int socket_health = 0;

        public String HubName { get; set; }
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

        public LinkClient()
        {
            this.Leaves = new List<Leaf>();
        }

        public bool IsLinked { get { return this.Busy; } }
        public String Name { get { return this.HubName; } }

        public void ForEachLeaf(Action<iconnect.ILeaf> action)
        {
            this.Leaves.ForEach(action);
        }

        public iconnect.ILeaf FindLeaf(Predicate<iconnect.ILeaf> predicate)
        {
            return this.Leaves.Find(predicate);
        }

        public IClient FindUser(Predicate<IClient> predicate)
        {
            IClient result = null;

            if (this.Busy)
                this.Leaves.ForEach(x =>
                {
                    result = x.Users.Find(predicate);

                    if (result != null)
                        return;
                });

            return result;
        }

        private void ClearUserlist()
        {
            foreach (Leaf leaf in this.Leaves)
                leaf.Users.ForEachWhere(x =>
                {
                    UserPool.AUsers.ForEachWhere(z => z.SendPacket(TCPOutbound.Part(z, x)),
                        z => z.LoggedIn && !z.Quarantined && z.Vroom == x.Vroom);

                    UserPool.WUsers.ForEachWhere(z => z.QueuePacket(ib0t.WebOutbound.PartTo(z, x.Name)),
                        z => z.LoggedIn && !z.Quarantined && z.Vroom == x.Vroom);
                }, x => x.Link.Visible);

            this.Leaves.Clear();
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
                    this.KillSocket();

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
                        this.KillSocket();
                        bool was_linked = this.LoginPhase == LinkLogin.Ready;

                        if (this.CanReconnect)
                        {
                            this.Time = time;
                            this.LoginPhase = LinkLogin.Sleeping;
                        }
                        else this.Busy = false;

                        Events.LinkError(LinkError.BadProtocol);
                        ServerCore.Log("packet read fail from hub " + packet.Msg, e);

                        if (was_linked)
                            this.ClearUserlist();

                        Events.Unlinked();
                        return;
                    }

                this.SendReceive();

                if (!this.SocketConnected)
                {
                    this.KillSocket();
                    bool unlink_fire = this.LoginPhase == LinkLogin.Ready;

                    if (this.CanReconnect)
                    {
                        this.Time = time;
                        this.LoginPhase = LinkLogin.Sleeping;
                    }
                    else this.Busy = false;
                    
                    Events.LinkError(LinkError.RemoteDisconnect);

                    if (unlink_fire)
                    {
                        this.ClearUserlist();
                        Events.Unlinked();
                    }
                }
                else if (this.LoginPhase == LinkLogin.AwaitingAck)
                {
                    if (time > (this.Time + 60000))
                    {
                        this.KillSocket();

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
                        this.KillSocket();
                        
                        if (this.CanReconnect)
                        {
                            this.Time = time;
                            this.LoginPhase = LinkLogin.Sleeping;
                        }
                        else this.Busy = false;

                        Events.LinkError(LinkError.PingTimeout);
                        this.ClearUserlist();
                        Events.Unlinked();
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
            this.HubName = Settings.Name;
            this.Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Sock.Blocking = false;

            try
            {
                this.Sock.Connect(new IPEndPoint(this.ExternalIP, this.Port));
            }
            catch { }
        }

        public void Connect(String hashlink)
        {
            if (this.Local)
            {
                Events.LinkError(LinkError.HubMode);
                return;
            }

            if (this.Busy)
            {
                Events.LinkError(LinkError.AlreadyLinking);
                return;
            }

            Room obj = Hashlink.DecodeHashlink(hashlink);

            if (obj == null)
            {
                Events.LinkError(LinkError.InvalidHashlink);
                return;
            }

            if (((LinkHub.LinkMode)Settings.Get<int>("link_mode")) != LinkHub.LinkMode.Leaf)
            {
                Events.LinkError(LinkError.EnableLeafLinking);
                return;
            }

            UserPool.AUsers.ForEachWhere(x => x.Cloaked = false, x => x.LoggedIn);
            this.data_in.Clear();
            this.data_out = new ConcurrentQueue<byte[]>();
            this.CanReconnect = Settings.Get<bool>("link_reconnect");
            this.LoginPhase = LinkLogin.Connecting;
            this.Time = core.Time.Now;
            this.Busy = true;
            this.Local = false;
            this.ExternalIP = obj.IP;
            this.Port = obj.Port;
            this.HubName = obj.Name;
            this.Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Sock.Blocking = false;

            try
            {
                this.Sock.Connect(new IPEndPoint(this.ExternalIP, this.Port));
            }
            catch { }
        }

        public void KillSocket()
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

        public bool Disconnect()
        {
            if (this.Local)
            {
                Events.LinkError(LinkError.HubMode);
                return false;
            }

            if (!this.Busy)
            {
                Events.LinkError(LinkError.WasNotLinking);
                return false;
            }

            this.KillSocket();
            this.Busy = false;
            this.ClearUserlist();
            return true;
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
