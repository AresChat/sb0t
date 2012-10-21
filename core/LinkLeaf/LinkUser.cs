using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using iconnect;

namespace core.LinkLeaf
{
    class LinkUser : IClient, IUser
    {
        public uint JoinTime { get; set; }
        public ushort ID { get { return 999; } }
        public IPAddress ExternalIP { get; set; }
        public String DNS { get; set; }
        public Guid Guid { get; set; }
        public ushort FileCount { get; set; }
        public ushort DataPort { get; set; }
        public IPAddress NodeIP { get { return IPAddress.Any; } set { } }
        public ushort NodePort { get { return 0; } set { } }
        public String OrgName { get; set; }
        public String Version { get; set; }
        public IPAddress LocalIP { get; set; }
        public bool Browsable { get; set; }
        public byte Age { get; set; }
        public byte Sex { get; set; }
        public byte Country { get; set; }
        public String Region { get; set; }
        public bool FastPing { get { return false; } set { } }
        public ILevel Level { get; set; }
        public bool Ghosting { get { return false; } set { } }
        public List<String> IgnoreList { get { return new List<String>(); } set { } }
        public IFont Font { get { return new Font(); } set { } }
        public bool CustomClient { get; set; }
        public List<String> CustomClientTags { get { return new List<String>(); } set { } }
        public bool WebClient { get; set; }
        public bool Owner { get { return false; } set { } }
        public bool Encrypted { get; set; }
        public bool Captcha { get { return true; } set { } }
        public String CaptchaWord { get; set; }
        public bool Registered { get; set; }
        public bool Cloaked { get { return false; } set { } }
        public bool Connected { get { return true; } }
        public bool Idle { get; set; }
        public bool Quarantined { get { return false; } set { } }
        public uint Cookie { get; set; }
        public Encryption Encryption { get; set; }
        public FloodRecord FloodRecord { get; set; }
        public bool Idled { get; set; }
        public ulong IdleStart { get; set; }
        public IUser IUser { get { return this; } }
        public UserLinkCredentials LinkCredentials { get; set; }
        public ILink Link { get { return this.LinkCredentials; } set { } }

        public void Scribble(String sender, byte[] img, int height)
        {
            
        }

        public void Nudge(String sender)
        {

        }

        public void SetMuzzled(bool b) { this._muzzled = b; }
        private bool _muzzled;
        public bool Muzzled
        {
            get { return this._muzzled; }
            set
            {
                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LeafOutbound.LeafIUser(ServerCore.Linker,
                        this, value ? "muzzle" : "unmuzzle", String.Empty));
            }
        }

        public void SetCustomName(String str) { this._customname = str; }
        private String _customname;
        public String CustomName
        {
            get { return this._customname; }
            set
            {
                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LeafOutbound.LeafIUser(ServerCore.Linker,
                        this, "customname", value == null ? String.Empty : value));
            }
        }


        public void SetAvatar(byte[] data) { this._avatar = data; }
        private byte[] _avatar;
        public byte[] Avatar
        {
            get { return this._avatar; }
            set
            {
                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LeafOutbound.LeafIUserBin(ServerCore.Linker,
                        this, "avatar", value == null ? new byte[] { } : value));
            }
        }

        public void SetPersonalMessage(String str) { this._personalmessage = str; }
        private String _personalmessage;
        public String PersonalMessage
        {
            get { return this._personalmessage; }
            set
            {
                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LeafOutbound.LeafIUser(ServerCore.Linker,
                        this, "personalmessage", value == null ? String.Empty : value));
            }
        }

        public void SetName(String name) { this._name = name; }
        private String _name;
        public String Name
        {
            get { return this._name; }
            set
            {
                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LeafOutbound.LeafIUser(ServerCore.Linker,
                        this, "name", value == null ? String.Empty : value));
            }
        }

        public void SetVroom(ushort vroom) { this._vroom = vroom; }
        private ushort _vroom;
        public ushort Vroom
        {
            get { return this._vroom; }
            set
            {
                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LeafOutbound.LeafIUserBin(ServerCore.Linker,
                        this, "vroom", BitConverter.GetBytes(value)));
            }
        }

        public void Unquarantine() { }

        public void BinaryWrite(byte[] data)
        {
            if (data != null)
                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LeafOutbound.LeafIUserBin(ServerCore.Linker,
                        this, "binary", data));
        }

        public void Print(object text)
        {
            if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                ServerCore.Linker.SendPacket(LeafOutbound.LeafIUser(ServerCore.Linker,
                    this, "print", text.ToString()));
        }

        public void Ban()
        {
            if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                ServerCore.Linker.SendPacket(LeafOutbound.LeafIUser(ServerCore.Linker,
                    this, "ban", String.Empty));
        }

        public void Disconnect()
        {
            if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                ServerCore.Linker.SendPacket(LeafOutbound.LeafIUser(ServerCore.Linker,
                    this, "disconnect", String.Empty));
        }

        public void Redirect(String hashlink)
        {
            if (!String.IsNullOrEmpty(hashlink))
                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LeafOutbound.LeafIUser(ServerCore.Linker,
                        this, "redirect", hashlink));
        }

        public void SendText(String text)
        {
            if (!String.IsNullOrEmpty(text))
                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LeafOutbound.LeafIUser(ServerCore.Linker,
                        this, "sendtext", text));
        }

        public void SendEmote(String text)
        {
            if (!String.IsNullOrEmpty(text))
                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LeafOutbound.LeafIUser(ServerCore.Linker,
                        this, "sendemote", text));
        }

        public void PM(String sender, String text)
        {
            if (!String.IsNullOrEmpty(sender) && !String.IsNullOrEmpty(text))
                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LeafOutbound.LeafIUser(ServerCore.Linker,
                        this, "pm", sender, text));
        }

        public void Topic(String text)
        {
            if (text != null)
                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LeafOutbound.LeafIUser(ServerCore.Linker,
                        this, "topic", text));
        }

        public void RestoreAvatar()
        {
            if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                ServerCore.Linker.SendPacket(LeafOutbound.LeafIUser(ServerCore.Linker,
                    this, "restoreavatar", String.Empty));
        }

        public void URL(String address, String text)
        {
            if (!String.IsNullOrEmpty(address) && !String.IsNullOrEmpty(text))
                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LeafOutbound.LeafIUser(ServerCore.Linker,
                        this, "url", address, text));
        }

        public LinkUser(uint ident)
        {
            this._avatar = new byte[] { };
            this._personalmessage = String.Empty;

            this.LinkCredentials = new UserLinkCredentials
            {
                Ident = ident,
                IsLinked = true
            };
        }
    }
}
