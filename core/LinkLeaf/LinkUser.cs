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
        public bool Muzzled { get; set; }
        public String CustomName { get; set; }
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
        public uint LeafIdent { get; private set; }
        public bool Visible { get; set; }
        public uint Cookie { get; set; }
        public Encryption Encryption { get; set; }
        public FloodRecord FloodRecord { get; set; }
        public bool Idled { get; set; }
        public ulong IdleStart { get; set; }
        public IUser IUser { get { return this; } }
        public bool Linked { get { return true; } }

        public void SetAvatar(byte[] data)
        {
            this._avatar = data;
        }

        private byte[] _avatar;
        public byte[] Avatar
        {
            get { return this._avatar; }
            set
            {
                this._avatar = value;
            }
        }

        public void SetPersonalMessage(String str)
        {
            this._personalmessage = str;
        }

        private String _personalmessage;
        public String PersonalMessage
        {
            get { return this._personalmessage; }
            set
            {
                this._personalmessage = value;
            }
        }

        public void SetName(String name)
        {
            this._name = name;
        }

        private String _name;
        public String Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
            }
        }

        public void SetVroom(ushort vroom)
        {
            this._vroom = vroom;
        }

        private ushort _vroom;
        public ushort Vroom
        {
            get { return this._vroom; }
            set
            {
                this._vroom = value;
            }
        }

        public void Unquarantine() { }

        public void BinaryWrite(byte[] data)
        {

        }

        public void Print(object text)
        {

        }

        public void Ban()
        {

        }

        public void Disconnect()
        {

        }

        public void Redirect(String hashlink)
        {

        }

        public void SendText(String text)
        {

        }

        public void SendEmote(String text)
        {

        }

        public void PM(String sender, String text)
        {

        }

        public void Topic(String text)
        {

        }

        public void RestoreAvatar()
        {

        }

        public void URL(String address, String text)
        {

        }

        public LinkUser(uint ident)
        {
            this._avatar = new byte[] { };
            this._personalmessage = String.Empty;
            this.LeafIdent = ident;
        }
    }
}
