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

namespace core
{
    class AresClient : IClient, IUser, IQuarantined
    {
        public uint LastScribble { get; set; }
        public bool Ares { get; set; }
        public bool IsCbot { get; set; }
        public ushort ID { get; private set; }
        public IPAddress ExternalIP { get; set; }
        public String DNS { get; set; }
        public bool LoggedIn { get; set; }
        public ulong Time { get; set; }
        public Guid Guid { get; set; }
        public ushort FileCount { get; set; }
        public ushort DataPort { get; set; }
        public IPAddress NodeIP { get; set; }
        public ushort NodePort { get; set; }
        public String OrgName { get; set; }
        public String Version { get; set; }
        public IPAddress LocalIP { get; set; }
        public bool Browsable { get; set; }
        public byte CurrentUploads { get; set; }
        public byte MaxUploads { get; set; }
        public byte CurrentQueued { get; set; }
        public byte Age { get; set; }
        public byte Sex { get; set; }
        public byte Country { get; set; }
        public String Region { get; set; }
        public Encryption Encryption { get; set; }
        public bool FastPing { get; set; }
        public bool Ghosting { get; set; }
        public uint Cookie { get; set; }
        public List<String> IgnoreList { get; set; }
        public bool CustomClient { get; set; }
        public List<SharedFile> SharedFiles { get; set; }
        public List<String> CustomClientTags { get; set; }
        public bool VoiceChatPublic { get; set; }
        public bool VoiceChatPrivate { get; set; }
        public bool VoiceOpusChatPublic { get; set; }
        public bool VoiceOpusChatPrivate { get; set; }
        public List<String> VoiceChatIgnoreList { get; set; }
        public bool WebClient { get; private set; }
        public bool Owner { get; set; }
        public bool IsHTML { get; private set; }
        public bool Captcha { get; set; }
        public String CaptchaWord { get; set; }
        public byte[] OrgAvatar { get; set; }
        public uint JoinTime { get; private set; }
        public ulong IdleStart { get; set; }
        public bool Quarantined { get; set; }
        public bool IsLeaf { get; set; }
        public ILink Link { get { return new UserLinkCredentials(); } }
        public byte[] Password { get; set; }
        public bool SupportsHTML { get; set; }
        public bool IsWebWorker { get; set; }
        public IPEndPoint LocalEP { get; set; }

        public bool BlockCustomNames { get; set; }

        public IFont Font { get; set; }

        public Socket Sock { get; set; }
        public IPAddress SocketAddr { get; set; }
        public bool HasSecureLoginAttempted { get; set; }
        public FloodRecord FloodRecord { get; private set; }
        public bool AvatarReceived { get; set; }
        public ulong AvatarTimeout { get; set; }
        public bool DefaultAvatar { get; set; }

        private List<byte> data_in = new List<byte>();
        private ConcurrentQueue<byte[]> data_out = new ConcurrentQueue<byte[]>();
        private int socket_health = 0;
        private byte[] avatar = new byte[] { };
        private String personal_message = String.Empty;
        private ILevel _level = ILevel.Regular;
        private String _name = String.Empty;
        private ushort _vroom = 0;
        private bool _cloaked = false;
        private String _customname = String.Empty;

        public UserScribbleRoomObject ScribbleRoomObject = new UserScribbleRoomObject();

        public AresClient(Socket sock, ulong time, ushort id)
        {
            this.OrgAvatar = new byte[] { };
            this.ID = id;
            this.Sock = sock;
            this.Sock.Blocking = false;
            this.Time = time;
            this.SocketAddr = ((IPEndPoint)this.Sock.RemoteEndPoint).Address;
            this.ExternalIP = ((IPEndPoint)this.Sock.RemoteEndPoint).Address;
            this.LocalEP = (IPEndPoint)this.Sock.LocalEndPoint;
            this.Cookie = AccountManager.NextCookie;
            this.Encryption = new core.Encryption { Mode = EncryptionMode.Unencrypted };
            this.Version = String.Empty;
            this.IgnoreList = new List<String>();
            this.SharedFiles = new List<SharedFile>();
            this.CustomClientTags = new List<String>();
            this.VoiceChatIgnoreList = new List<String>();
            this.CaptchaWord = String.Empty;
            this.Captcha = !Settings.Get<bool>("captcha");
            this.JoinTime = Helpers.UnixTime;
            this.FloodRecord = new core.FloodRecord();
            this.AvatarTimeout = time;
            this.Font = new AresFont();
            Dns.BeginGetHostEntry(this.ExternalIP, new AsyncCallback(this.DnsReceived), null);
           // this.DNS = "unknown";
        }

        public void SendHTML(String text)
        {
            if (this.SupportsHTML)
                this.SendPacket(TCPOutbound.HTML(text));
        }

        public void Release()
        {
            this.Unquarantine();
        }

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

        public void Scribble(String sender, byte[] img, int height)
        {
            List<byte> b = new List<byte>(img);

            if (b.Count <= 4000)
                this.SendPacket(TCPOutbound.CustomData(this, sender, "cb0t_scribble_once", img));
            else
            {
                List<byte[]> p = new List<byte[]>();

                while (b.Count > 4000)
                {
                    p.Add(b.GetRange(0, 4000).ToArray());
                    b.RemoveRange(0, 4000);
                }

                if (b.Count > 0)
                    p.Add(b.ToArray());

                for (int i = 0; i < p.Count; i++)
                {
                    if (i == 0)
                        this.SendPacket(TCPOutbound.CustomData(this, sender, "cb0t_scribble_first", p[i]));
                    else if (i == (p.Count - 1))
                        this.SendPacket(TCPOutbound.CustomData(this, sender, "cb0t_scribble_last", p[i]));
                    else
                        this.SendPacket(TCPOutbound.CustomData(this, sender, "cb0t_scribble_chunk", p[i]));
                }
            }
        }

        public void Nudge(String sender)
        {
            byte[] buf = Encoding.UTF8.GetBytes("0" + sender);
            buf = Crypto.e67(buf, 1488);
            buf = Encoding.Default.GetBytes(Convert.ToBase64String(buf));
            this.SendPacket(TCPOutbound.CustomData(this, sender, "cb0t_nudge", buf));
        }

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

        public void Unquarantine()
        {
            this.LoggedIn = false;
            this.Quarantined = false;

            if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafJoin(ServerCore.Linker, this));

            Helpers.FakeRejoinSequence(this, true);
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
            int len = Encoding.UTF8.GetByteCount(sender);

            if (len >= 2 && len <= 20)
            {
                len = Encoding.UTF8.GetByteCount(text);

                if (len > 0 && len <= 300)
                    this.SendPacket(TCPOutbound.Private(this, sender, text));
            }
        }

        public void Redirect(String hashlink)
        {
            Room room = Hashlink.DecodeHashlink(hashlink);

            if (room != null && this.Level == ILevel.Regular)
            {
                byte[] buf;

                while (this.data_out.Count > 0)
                    if (!this.data_out.TryDequeue(out buf))
                        break;

                this.SendPacket(TCPOutbound.Redirect(this, room));
                this.Disconnect();
            }
        }

        public void RestoreAvatar()
        {
            if (this.rest_av != null)
                this.OrgAvatar = this.rest_av;

            this.Avatar = this.OrgAvatar;
        }

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
            if (text != null)
                if (Encoding.UTF8.GetByteCount(text) <= 500)
                    this.SendPacket(TCPOutbound.TopicFirst(this, text));
        }

        public void URL(String address, String text)
        {
            if (address != null && text != null)
                this.SendPacket(TCPOutbound.Url(this, address, text));
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
            get { return this.Encryption.Mode == EncryptionMode.Encrypted; }
        }

        public bool Cloaked
        {
            get { return this._cloaked; }
            set
            {
                if (value)
                    if (!this.LoggedIn || this.Quarantined)
                        return;

                if (ServerCore.Linker.Busy)
                    return;

                if (value == this._cloaked)
                    return;

                this._cloaked = value;

                if (value)
                {
                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Part(x, this)),
                        x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                    UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.PartTo(x, this.Name)),
                        x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);
                }
                else Helpers.UncloakedSequence(this);
            }
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

                        if (!this.Cloaked)
                        {
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
                        }

                        String current = this._name;
                        this._name = value;
                        Helpers.FakeRejoinSequence(this, false);
                        
                        if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                            ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafNameChanged(ServerCore.Linker, current, this._name));
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

                            if (!this.Cloaked)
                            {
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
                            }

                            this._vroom = value;
                            Helpers.FakeRejoinSequence(this, false);

                            if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                                ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafVroomChanged(ServerCore.Linker, this));
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

                    if (this.LoggedIn && !this.Cloaked)
                    {
                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.UpdateUserStatus(x, this)),
                            x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.UpdateTo(x, this.Name, this._level)),
                            x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                        if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                            ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafUserUpdated(ServerCore.Linker, this));
                    }

                    this.SendPacket(TCPOutbound.OpChange(this));
                    Events.AdminLevelChanged(this);
                }
            }
        }

        public void Print(object text)
        {
            this.SendPacket(TCPOutbound.NoSuch(this, text.ToString()));
        }

        public void BinaryWrite(byte[] data)
        {
            this.SendPacket(data);
        }

        private void DnsReceived(IAsyncResult result)
        {
            if (this.SocketConnected)
                try
                {
                    IPHostEntry i = Dns.EndGetHostEntry(result);
                    this.DNS = Helpers.ObfuscateDns(i.HostName);
                }
                catch
                {
                    try
                    {
                        this.DNS = Helpers.ObfuscateDns(this.ExternalIP.ToString());
                    }
                    catch { }
                }
        }

        private byte[] rest_av = null;

        public byte[] Avatar
        {
            get { return this.avatar; }
            set
            {
                if (value == null)
                {
                    value = new byte[] { };

                    if (this.avatar != null)
                        if (this.avatar.Length >= 10)
                            this.rest_av = this.avatar;
                }

                if (value.Length < 10)
                {
                    this.avatar = new byte[] { };
                    this.AvatarReceived = false;

                    if (!this.Cloaked && !this.Quarantined)
                    {
                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.AvatarCleared(x, this)),
                            x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.AvatarClearTo(x, this.Name)),
                            x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined && x.Extended);

                        if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                            ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafAvatar(ServerCore.Linker, this));
                    }
                }
                else
                {
                    this.avatar = value;

                    if (!this.Cloaked && !this.Quarantined)
                    {
                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Avatar(x, this)),
                            x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.AvatarTo(x, this.Name, this.Avatar)),
                            x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined && x.Extended);

                        if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                            ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafAvatar(ServerCore.Linker, this));
                    }
                }
            }
        }

        public String PersonalMessage
        {
            get { return this.personal_message; }
            set
            {
                this.personal_message = value == null ? String.Empty : value;

                if (!this.Cloaked && !this.Quarantined)
                {
                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.PersonalMessage(x, this)),
                        x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                    UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.PersMsgTo(x, this.Name, this.personal_message)),
                        x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined && x.Extended);

                    if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                        ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafPersonalMessage(ServerCore.Linker, this));
                }
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

            if (!this.LoggedIn)
                if (!this.IsHTML)
                    if (this.data_in.Count >= 3)
                    {
                        String test_str = Encoding.UTF8.GetString(this.data_in.ToArray()).ToUpper();
                        this.IsHTML = test_str.StartsWith("GET / ");
                        
                        if (!this.IsHTML)
                            this.IsWebWorker = test_str.StartsWith("GET");
                    }
        }

        public void EnforceRules(ulong time)
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
            this.Disconnect(false);
        }

        public void Disconnect(bool ghost)
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

            if (!ghost)
                this.SendDepart();
            else if (this.LoggedIn && !this.Quarantined)
            {
                this.LoggedIn = false;
                Events.Parting(this);

                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafPart(ServerCore.Linker, this));

                Events.Parted(this);
            }

            this.LoggedIn = false;
        }

        public void SendDepart()
        {
            if (this.LoggedIn && !this.Quarantined)
            {
                this.LoggedIn = false;
                Events.Parting(this);

                if (!this.Cloaked)
                {
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
                }

                Events.Parted(this);
            }
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

        public byte[] ReceiveDump
        {
            get { return this.data_in.ToArray(); }
        }

        
    }
}
