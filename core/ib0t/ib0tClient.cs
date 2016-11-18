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
using iconnect;

namespace core.ib0t
{
    class ib0tClient : IClient, IUser, IQuarantined
    {
        public uint LastScribble { get; set; }
        public bool Ares { get; set; }
        public bool SupportsHTML { get { return false; } }
        public ushort ID { get; private set; }
        public IPAddress ExternalIP { get; set; }
        public String DNS { get; set; }
        public Guid Guid { get; set; }
        public ushort FileCount { get; set; }
        public ushort DataPort { get; set; }
        public IPAddress NodeIP { get; set; }
        public ushort NodePort { get; set; }
        public String OrgName { get; set; }
        public String Version { get; set; }
        public IPAddress LocalIP { get; set; }
        public bool Browsable { get; set; }
        public byte Age { get; set; }
        public byte Sex { get; set; }
        public byte Country { get; set; }
        public String Region { get; set; }
        public bool FastPing { get; set; }
        public bool Ghosting { get; set; }
        public List<String> IgnoreList { get; set; }
        public bool CustomClient { get; set; }
        public List<String> CustomClientTags { get; set; }
        public bool WebClient { get; private set; }
        public bool Owner { get; set; }
        public Encryption Encryption { get; set; }
        public bool Captcha { get; set; }
        public uint Cookie { get; set; }
        public uint JoinTime { get; set; }
        public String CaptchaWord { get; set; }
        public ulong IdleStart { get; set; }
        public bool Quarantined { get; set; }
        public ILink Link { get { return new UserLinkCredentials(); } }
        public byte[] Password { get; set; }
        public bool Extended { get; set; }
        public IFont Font { get; set; }
        public IPEndPoint LocalEP { get; set; }

        public Html5RequestEventArgs WebCredentials { get; set; }
        public Socket Sock { get; set; }
        public ulong Time { get; set; }
        public bool ProtoConnected { get; set; }
        public bool LoggedIn { get; set; }
        public FloodRecord FloodRecord { get; private set; }

        private int socket_health = 0;
        private List<byte> data_in = new List<byte>();
        private ConcurrentQueue<byte[]> data_out = new ConcurrentQueue<byte[]>();
        private ILevel _level = ILevel.Regular;
        private String _name = String.Empty;
        private ushort _vroom = 0;
        private String _customname = String.Empty;

        public ib0tClient(AresClient client, ulong time, ushort id)
        {
            this.WebClient = true;
            this.WebCredentials = new Html5RequestEventArgs();
            this.ID = id;
            this.Sock = client.Sock;
            this.WebCredentials.Key = String.Empty;
            this.data_in.AddRange(client.ReceiveDump);
            this.WebCredentials.Key1 = String.Empty;
            this.WebCredentials.Key2 = String.Empty;
            this.WebCredentials.KeyData = new byte[] { };
            this.WebCredentials.OldProto = false;
            this.WebCredentials.Host = String.Empty;
            this.WebCredentials.Origin = String.Empty;
            this.Time = time;
            this.ProtoConnected = false;
            this.ExternalIP = ((IPEndPoint)this.Sock.RemoteEndPoint).Address;
            this.LocalEP = (IPEndPoint)this.Sock.LocalEndPoint;
            this._vroom = 0;
            this.Version = String.Empty;
            this.IgnoreList = new List<String>();
            this.CustomClientTags = new List<String>();
            this.Encryption = new Encryption { Mode = EncryptionMode.Unencrypted };
            this.CaptchaWord = String.Empty;
            this.Captcha = !Settings.Get<bool>("captcha");
            this.DNS = client.DNS;
            this.JoinTime = Helpers.UnixTime;
            this.FloodRecord = new FloodRecord();
            this.Font = new AresFont();
            
            // vvvvvvvvvvvvvvvvvv
            this.Extended = true;
        }

        public void SendHTML(String text) { }

        public void SetLevel(ILevel level)
        {
            if (!this.LoggedIn)
                return;

            this.Registered = true;

            if (this.Quarantined)
                this.Unquarantine();

            this.Captcha = true;
            this.Level = level;
        }

        public void Scribble(String sender, byte[] img, int h)
        {
            if (!this.Extended || !this.ProtoConnected)
                return;

            byte[] buf = Zip.Decompress(img);
            String height = h.ToString();
            String base64 = Convert.ToBase64String(buf);
            List<String> packets = new List<String>();

            while (base64.Length > 1024)
            {
                packets.Add(base64.Substring(0, 1024));
                base64 = base64.Substring(1024);
            }

            if (base64.Length > 0)
                packets.Add(base64);

            this.QueuePacket(WebOutbound.ScribbleHead(this, sender, packets.Count, height));

            foreach (String str in packets)
                this.QueuePacket(WebOutbound.ScribbleBlock(this, str));
        }

        public void Nudge(String sender) { }

        private bool _muzzled;
        public bool Muzzled
        {
            get { return this._muzzled; }
            set
            {
                this._muzzled = value;

                if (ServerCore.Linker.Busy && this.LoggedIn && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafUserUpdated(ServerCore.Linker, this));
            }
        }

        private bool _registered;
        public bool Registered
        {
            get { return this._registered; }
            set
            {
                this._registered = value;

                if (ServerCore.Linker.Busy && this.LoggedIn && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafUserUpdated(ServerCore.Linker, this));
            }
        }

        private bool _idled;
        public bool Idled
        {
            get { return this._idled; }
            set
            {
                this._idled = value;

                if (ServerCore.Linker.Busy && this.LoggedIn && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafUserUpdated(ServerCore.Linker, this));
            }
        }

        public bool Idle { get { return this.Idled; } }

        public void Release()
        {
            this.Unquarantine();
        }
        
        public void Unquarantine()
        {
            this.LoggedIn = false;
            this.Quarantined = false;
            Helpers.FakeRejoinSequence(this, true);

            if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafJoin(ServerCore.Linker, this));
        }

        public String CustomName
        {
            get
            {
                if (!Settings.Get<bool>("customnames"))
                    return String.Empty;

                return this._customname;
            }
            set
            {
                this._customname = value == null ? String.Empty : value;

                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafCustomName(ServerCore.Linker, this));
            }
        }

        public void Ban()
        {
            if (!this.Owner)
                BanSystem.AddBan(this);

            this.Disconnect();
        }

        public void PM(String sender, String text)
        {
            if (!this.Extended)
                return;

            int len = Encoding.UTF8.GetByteCount(sender);

            if (len >= 2 && len <= 20)
            {
                len = Encoding.UTF8.GetByteCount(text);

                if (len > 0 && len <= 300)
                    this.QueuePacket(WebOutbound.PrivateTo(this, sender, text));
            }
        }

        public void Redirect(String hashlink) { }
        public void RestoreAvatar() { }

        public void SendEmote(String text)
        {
            if (!String.IsNullOrEmpty(text) && !this.Quarantined)
            {
                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Emote(x, this.Name, text)),
                    x => x.LoggedIn && x.Vroom == this.Vroom && !x.IgnoreList.Contains(this.Name) && !x.Quarantined);

                UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.EmoteTo(x, this.Name, text)),
                    x => x.LoggedIn && x.Vroom == this.Vroom && !x.IgnoreList.Contains(this.Name) && !x.Quarantined);

                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafEmoteText(ServerCore.Linker, this.Name, text));
            }
        }

        public void SendText(String text)
        {
            if (!String.IsNullOrEmpty(text) && !this.Quarantined)
            {
                UserPool.AUsers.ForEachWhere(x => x.SendPacket((String.IsNullOrEmpty(this.CustomName) || x.BlockCustomNames) ?
                    TCPOutbound.Public(x, this.Name, text) : TCPOutbound.NoSuch(x, this.CustomName + text)),
                    x => x.LoggedIn && x.Vroom == this.Vroom && !x.IgnoreList.Contains(this.Name) && !x.Quarantined);

                UserPool.WUsers.ForEachWhere(x => x.QueuePacket(String.IsNullOrEmpty(this.CustomName) ?
                    ib0t.WebOutbound.PublicTo(x, this.Name, text) : ib0t.WebOutbound.NoSuchTo(x, this.CustomName + text)),
                    x => x.LoggedIn && x.Vroom == this.Vroom && !x.IgnoreList.Contains(this.Name) && !x.Quarantined);

                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafPublicText(ServerCore.Linker, this.Name, text));
            }
        }

        public void Topic(String text)
        {
            if (text != null && this.ProtoConnected)
                if (Encoding.UTF8.GetByteCount(text) <= 500)
                    this.QueuePacket(WebOutbound.TopicFirstTo(this, text));
        }

        public void URL(String address, String text)
        {
            if (this.ProtoConnected)
                if (address != null && text != null)
                    this.QueuePacket(WebOutbound.UrlTo(this, address, text));
        }

        public IUser IUser
        {
            get { return this; }
        }

        public bool Connected
        {
            get { return this.SocketConnected; }
        }

        public bool Encrypted
        {
            get { return false; }
        }

        public bool Cloaked
        {
            get { return false; }
            set { }
        }

        public String Name
        {
            get { return this._name; }
            set
            {
                if (this.SocketConnected && Helpers.NameAvailable(this, value))
                    if (!this.LoggedIn)
                        this._name = value;
                    else
                    {
                        if (this.Quarantined)
                            return;

                        this.LoggedIn = false;
                        LinkLeaf.LinkUser other = null;

                        if (ServerCore.Linker.Busy)
                            foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                            {
                                other = leaf.Users.Find(x => x.Vroom == this.Vroom && x.Name == this.Name && !x.Link.Visible);

                                if (other != null)
                                {
                                    other.LinkCredentials.Visible = true;
                                    break;
                                }
                            }

                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(other == null ? TCPOutbound.Part(x, this) : TCPOutbound.UpdateUserStatus(x, other)),
                            x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(other == null ? ib0t.WebOutbound.PartTo(x, this.Name) : ib0t.WebOutbound.UpdateTo(x, other.Name, other.Level)),
                            x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                        this._name = value;
                        Helpers.FakeRejoinSequence(this, false);
                    }
            }
        }

        public ushort Vroom
        {
            get { return this._vroom; }
            set
            {
                if (this.SocketConnected)
                    if (Events.VroomChanging(this, value))
                    {
                        if (!this.LoggedIn)
                            this._vroom = value;
                        else
                        {
                            if (this.Quarantined)
                                return;

                            this.LoggedIn = false;
                            LinkLeaf.LinkUser other = null;

                            if (ServerCore.Linker.Busy)
                                foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                                {
                                    other = leaf.Users.Find(x => x.Vroom == this.Vroom && x.Name == this.Name && !x.Link.Visible);

                                    if (other != null)
                                    {
                                        other.LinkCredentials.Visible = true;
                                        break;
                                    }
                                }

                            UserPool.AUsers.ForEachWhere(x => x.SendPacket(other == null ? TCPOutbound.Part(x, this) : TCPOutbound.UpdateUserStatus(x, other)),
                                x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                            UserPool.WUsers.ForEachWhere(x => x.QueuePacket(other == null ? ib0t.WebOutbound.PartTo(x, this.Name) : ib0t.WebOutbound.UpdateTo(x, other.Name, other.Level)),
                                x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                            this._vroom = value;
                            Helpers.FakeRejoinSequence(this, false);
                        }

                        Events.VroomChanged(this);
                    }
            }
        }

        public ILevel Level
        {
            get { return this._level; }
            set
            {
                if (value != this._level)
                {
                    this._level = value;

                    if (this.LoggedIn)
                    {
                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.UpdateUserStatus(x, this)),
                            x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(WebOutbound.UpdateTo(x, this.Name, this._level)),
                            x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                        if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                            ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafUserUpdated(ServerCore.Linker, this));
                    }

                    Events.AdminLevelChanged(this);
                }
            }
        }

        public void Print(object text)
        {
            if (this.ProtoConnected)
                this.QueuePacket(WebOutbound.NoSuchTo(this, text.ToString()));
        }

        public String _pmsg = "ib0t web user";
        public String PersonalMessage
        {
            get { return this._pmsg; }
            set { }
        }

        public byte[] Avatar
        {
            get { return Resource1.web; }
            set { }
        }

        public void BinaryWrite(byte[] data)
        {
            this.QueuePacket(data);
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
            this.SendDepart();
            this.LoggedIn = false;
        }

        public void SendDepart()
        {
            if (this.LoggedIn && !this.Quarantined)
            {
                this.LoggedIn = false;
                Events.Parting(this);

                LinkLeaf.LinkUser other = null;

                if (ServerCore.Linker.Busy)
                    foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                    {
                        other = leaf.Users.Find(x => x.Vroom == this.Vroom && x.Name == this.Name && !x.Link.Visible);

                        if (other != null)
                        {
                            other.LinkCredentials.Visible = true;
                            break;
                        }
                    }

                UserPool.AUsers.ForEachWhere(x => x.SendPacket(other == null ? TCPOutbound.Part(x, this) : TCPOutbound.UpdateUserStatus(x, other)),
                    x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                UserPool.WUsers.ForEachWhere(x => x.QueuePacket(other == null ? ib0t.WebOutbound.PartTo(x, this.Name) : ib0t.WebOutbound.UpdateTo(x, other.Name, other.Level)),
                    x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafPart(ServerCore.Linker, this));

                Events.Parted(this);
            }
        }

        public void QueuePacket(byte[] data)
        {
            this.data_out.Enqueue(data);
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

        private bool pending_keydata = false;

        internal bool LoadProtocol()
        {
            if (this.pending_keydata)
            {
                if (this.data_in.Count >= 8)
                {
                    this.WebCredentials.KeyData = this.data_in.GetRange(0, 8).ToArray();
                    this.data_in.RemoveRange(0, 8);
                    return true;
                }
            }
            else
            {
                while (this.data_in.CanTakeLine())
                {
                    String str = this.data_in.TakeLine();
                    String up = str.ToUpper();

                    if (up.StartsWith("SEC-WEBSOCKET-KEY:"))
                    {
                        str = str.Substring(18);

                        if (str.StartsWith(" "))
                            str = str.Substring(1);

                        this.WebCredentials.Key = str;
                    }
                    else if (up.StartsWith("SEC-WEBSOCKET-KEY1:"))
                    {
                        str = str.Substring(19);

                        if (str.StartsWith(" "))
                            str = str.Substring(1);

                        this.WebCredentials.Key1 = str;
                    }
                    else if (up.StartsWith("SEC-WEBSOCKET-KEY2:"))
                    {
                        str = str.Substring(19);

                        if (str.StartsWith(" "))
                            str = str.Substring(1);

                        this.WebCredentials.Key2 = str;
                    }
                    else if (up.StartsWith("HOST:"))
                    {
                        str = str.Substring(5);

                        if (str.StartsWith(" "))
                            str = str.Substring(1);

                        this.WebCredentials.Host = str;
                    }
                    else if (up.StartsWith("ORIGIN:"))
                    {
                        str = str.Substring(7);

                        if (str.StartsWith(" "))
                            str = str.Substring(1);

                        this.WebCredentials.Origin = str;
                    }
                    else if (String.IsNullOrEmpty(str.Trim()))
                        if (this.WebCredentials.Key1.Length > 0)
                        {
                            this.pending_keydata = true;
                            this.WebCredentials.OldProto = true;
                        }
                        else return true;
                }
            }

            return false;
        }

        internal String NextMessage
        {
            get { return this.WebCredentials.OldProto ? this.CheckPacketOld() : this.CheckPacketNew(); }
        }

        private String CheckPacketOld()
        {
            if (this.data_in.Count >= 2)
            {
                int index = this.data_in.IndexOf(255);

                if (index > 0)
                {
                    byte[] buffer = this.data_in.GetRange(1, (index - 1)).ToArray();
                    this.data_in.RemoveRange(0, (index + 1));
                    return Encoding.UTF8.GetString(buffer);
                }
            }

            return null;
        }

        private String CheckPacketNew()
        {
            if (this.data_in.Count >= 6)
            {
                int max_size = this.data_in.Count;
                byte msg = this.data_in[0];
                int len = (this.data_in[1] & 127);
                byte[] buffer;

                if (len == 126)
                {
                    if (max_size < 8)
                        return null;

                    buffer = this.data_in.GetRange(2, 2).ToArray();
                    Array.Reverse(buffer);
                    len = (int)BitConverter.ToUInt16(buffer, 0);

                    if (max_size < (8 + len))
                        return null;

                    this.data_in.RemoveRange(0, 4);
                }
                else if (len == 127)
                {
                    if (max_size < 14)
                        return null;

                    buffer = this.data_in.GetRange(2, 8).ToArray();
                    Array.Reverse(buffer);
                    len = (int)BitConverter.ToUInt64(buffer, 0);

                    if (max_size < (14 + len))
                        return null;

                    this.data_in.RemoveRange(0, 10);
                }
                else
                {
                    if (max_size < (6 + len))
                        return null;

                    this.data_in.RemoveRange(0, 2);
                }
                if (this.data_in.Count >= (len + 4))
                {
                    byte[] key = this.data_in.GetRange(0, 4).ToArray();
                    byte[] text = this.data_in.GetRange(4, len).ToArray();
                    this.data_in.RemoveRange(0, (len + 4));

                    for (int i = 0; i < text.Length; i++)
                        text[i] ^= key[i % 4];

                    switch (msg)
                    {
                        case 136:
                            this.SocketConnected = false;
                            return null;

                        case 129:
                            return Encoding.UTF8.GetString(text);

                        default:
                            return null;
                    }
                }
            }

            return null;
        }
    }
}
