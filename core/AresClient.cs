using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using iconnect;

namespace core
{
    class AresClient : IClient, IUser
    {
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
        public IFont Font { get; set; }
        public bool CustomClient { get; set; }
        public List<SharedFile> SharedFiles { get; set; }
        public List<String> CustomClientTags { get; set; }
        public bool VoiceChatPublic { get; set; }
        public bool VoiceChatPrivate { get; set; }
        public List<String> VoiceChatIgnoreList { get; set; }
        public bool Muzzled { get; set; }
        public bool CustomEmoticons { get; set; }
        public List<CustomEmoticon> EmoticonList { get; set; }
        public bool WebClient { get; private set; }
        public bool Owner { get; set; }
        public bool IsHTML { get; private set; }
        public bool Captcha { get; set; }
        public bool Registered { get; set; }
        public String CaptchaWord { get; set; }
        public byte[] OrgAvatar { get; set; }
        public uint JoinTime { get; private set; }
        public bool Idled { get; set; }
        public ulong IdleStart { get; set; }
        public bool Quarantined { get; set; }

        public Socket Sock { get; set; }
        public bool HasSecureLoginAttempted { get; set; }
        public FloodRecord FloodRecord { get; private set; }
        public bool AvatarReceived { get; set; }
        public ulong AvatarTimeout { get; set; }

        private List<byte> data_in = new List<byte>();
        private ConcurrentQueue<byte[]> data_out = new ConcurrentQueue<byte[]>();
        private int socket_health = 0;
        private byte[] avatar = new byte[] { };
        private String personal_message = String.Empty;
        private ILevel _level = ILevel.Regular;
        private String _name = String.Empty;
        private ushort _vroom = 0;
        private bool _cloaked = false;
        private String _customname = null;

        public AresClient(Socket sock, ulong time, ushort id)
        {
            this.OrgAvatar = new byte[] { };
            this.ID = id;
            this.Sock = sock;
            this.Sock.Blocking = false;
            this.Time = time;
            this.ExternalIP = ((IPEndPoint)this.Sock.RemoteEndPoint).Address;
            this.Cookie = AccountManager.NextCookie;
            this.Encryption = new core.Encryption { Mode = EncryptionMode.Unencrypted };
            this.Name = String.Empty;
            this.Version = String.Empty;
            this.IgnoreList = new List<String>();
            this.Font = new core.Font();
            this.SharedFiles = new List<SharedFile>();
            this.CustomClientTags = new List<String>();
            this.VoiceChatIgnoreList = new List<String>();
            this.EmoticonList = new List<CustomEmoticon>();
            this.CaptchaWord = String.Empty;
            this.Captcha = !Settings.Get<bool>("captcha");
            this.JoinTime = Helpers.UnixTime;
            this.FloodRecord = new core.FloodRecord();
            this.AvatarTimeout = time;
            Dns.BeginGetHostEntry(this.ExternalIP, new AsyncCallback(this.DnsReceived), null);
        }

        public bool Idle { get { return this.Idled; } }

        public void Unquarantine()
        {
            this.LoggedIn = false;
            this.Quarantined = false;
            Helpers.FakeRejoinSequence(this, true);
        }

        public String CustomName
        {
            get
            {
                if (!Settings.Get<bool>("customnames"))
                    return null;

                return this._customname;
            }
            set { this._customname = value; }
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
            }
        }

        public void SendText(String text)
        {
            if (!String.IsNullOrEmpty(text) && !this.Quarantined)
            {
                UserPool.AUsers.ForEachWhere(x => x.SendPacket(String.IsNullOrEmpty(this.CustomName) ?
                    TCPOutbound.Public(x, this.Name, text) : TCPOutbound.NoSuch(x, this.CustomName + text)),
                    x => x.LoggedIn && x.Vroom == this.Vroom && !x.IgnoreList.Contains(this.Name) && !x.Quarantined);

                UserPool.WUsers.ForEachWhere(x => x.QueuePacket(String.IsNullOrEmpty(this.CustomName) ?
                    ib0t.WebOutbound.PublicTo(x, this.Name, text) : ib0t.WebOutbound.NoSuchTo(x, this.CustomName + text)),
                    x => x.LoggedIn && x.Vroom == this.Vroom && !x.IgnoreList.Contains(this.Name) && !x.Quarantined);
            }
        }

        public void Topic(String text)
        {
            if (text != null)
                if (Encoding.UTF8.GetByteCount(text) <= 180)
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
                    if (!this.LoggedIn || !this.Quarantined)
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
                            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Part(x, this)),
                                x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                            UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.PartTo(x, this.Name)),
                                x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);
                        }

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

                            if (!this.Cloaked)
                            {
                                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Part(x, this)),
                                    x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                                UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.PartTo(x, this.Name)),
                                    x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);
                            }

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
                this._level = value;

                if (this.LoggedIn && !this.Cloaked)
                {
                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.UpdateUserStatus(x, this)),
                        x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                    UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.UpdateTo(x, this.Name, this._level)),
                        x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);
                }

                Events.AdminLevelChanged(this);
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
                    this.DNS = i.HostName;
                }
                catch
                {
                    try
                    {
                        this.DNS = this.ExternalIP.ToString();
                    }
                    catch { }
                }
        }

        public byte[] Avatar
        {
            get { return this.avatar; }
            set
            {
                if (value.Length < 10)
                {
                    this.avatar = new byte[] { };

                    if (!this.Cloaked)
                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.AvatarCleared(x, this)),
                            x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);
                }
                else
                {
                    this.avatar = value;

                    if (!this.Cloaked)
                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Avatar(x, this)),
                            x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);
                }
            }
        }

        public String PersonalMessage
        {
            get { return this.personal_message; }
            set
            {
                this.personal_message = value;

                if (!this.Cloaked)
                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.PersonalMessage(x, this)),
                        x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);
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
                    Stats.DataSent += (ulong)packet.Length;
                    this.data_out.TryDequeue(out packet);
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
                        this.IsHTML = Encoding.Default.GetString(this.data_in.ToArray(), 0, 3).ToUpper() == "GET";
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
                    this.data_out.TryPeek(out packet);
                    this.Sock.Send(packet);
                    Stats.DataSent += (ulong)packet.Length;
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
            try { this.Sock.Dispose(); }
            catch { }

            this.SocketConnected = false;

            if (!ghost)
                this.SendDepart();
        }

        public void SendDepart()
        {
            if (this.LoggedIn)
            {
                this.LoggedIn = false;
                Events.Parting(this);

                if (!this.Cloaked && !this.Quarantined)
                {
                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Part(x, this)),
                        x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);

                    UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.PartTo(x, this.Name)),
                        x => x.LoggedIn && x.Vroom == this.Vroom && !x.Quarantined);
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
