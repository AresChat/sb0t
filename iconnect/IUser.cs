using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace iconnect
{
    public interface IUser
    {
        /// <summary>Session identity</summary>
        ushort ID { get; }
        /// <summary>External IP Address</summary>
        IPAddress ExternalIP { get; set; }
        /// <summary>DNS Host Name</summary>
        String DNS { get; }
        /// <summary>16 byte GUID</summary>
        Guid Guid { get; }
        /// <summary>File Count</summary>
        ushort FileCount { get; }
        /// <summary>Data Port</summary>
        ushort DataPort { get; }
        /// <summary>Super Node IP Address</summary>
        IPAddress NodeIP { get; }
        /// <summary>Super Node Port</summary>
        ushort NodePort { get; }
        /// <summary>Current User Name</summary>
        String Name { get; set; }
        /// <summary>Original User Name</summary>
        String OrgName { get; }
        /// <summary>Client Version</summary>
        String Version { get; }
        /// <summary>Local IP Address</summary>
        IPAddress LocalIP { get; }
        /// <summary>Browse Flag</summary>
        bool Browsable { get; }
        /// <summary>Age</summary>
        byte Age { get; }
        /// <summary>Gender</summary>
        byte Sex { get; }
        /// <summary>Country Code</summary>
        byte Country { get; }
        /// <summary>Regional Location</summary>
        String Region { get; }
        /// <summary>Keep Alive Protocol Flag</summary>
        bool FastPing { get; }
        /// <summary>Admin Level</summary>
        ILevel Level { get; }
        /// <summary>Current Virtual Room</summary>
        ushort Vroom { get; set; }
        /// <summary>Ghosting Flag</summary>
        bool Ghosting { get; }
        /// <summary>Ignore List</summary>
        List<String> IgnoreList { get; set; }
        /// <summary>Custom Font</summary>
        IFont Font { get; }
        /// <summary>Third Party Client Flag</summary>
        bool CustomClient { get; }
        /// <summary>Tag Data</summary>
        List<String> CustomClientTags { get; set; }
        /// <summary>Muzzle Status</summary>
        bool Muzzled { get; set; }
        /// <summary>Custom Name</summary>
        String CustomName { get; set; }
        /// <summary>Web Client Flag</summary>
        bool WebClient { get; }
        /// <summary>Room Owner Flag</summary>
        bool Owner { get; }
        /// <summary>Avatar</summary>
        byte[] Avatar { get; set; }
        /// <summary>Personal Message</summary>
        String PersonalMessage { get; set; }
        /// <summary>Encryption Credentials</summary>
        bool Encrypted { get; }
        /// <summary>Capture Test Flag</summary>
        bool Captcha { get; }
        /// <summary>Logged In Flag</summary>
        bool Registered { get; }
        /// <summary>Cloaked</summary>
        bool Cloaked { get; set; }
        /// <summary>Connected</summary>
        bool Connected { get; }

        /// <summary>Send raw data to the socket</summary>
        void BinaryWrite(byte[] data);
        /// <summary>Print to this client</summary>
        void Print(object text);
        /// <summary>Ban user from the chatroom</summary>
        void Ban();
        /// <summary>Disconnect user from the chatroom</summary>
        void Disconnect();
        /// <summary>Redirect the user to a different chatroom</summary>
        void Redirect(String hashlink);
        /// <summary>Clone the user</summary>
        void SendText(String text);
        /// <summary>Clone the user</summary>
        void SendEmote(String text);
        /// <summary>Send a fake PM to this user</summary>
        void PM(String sender, String text);
        /// <summary>Set a virtual topic for the user</summary>
        void Topic(String text);
        /// <summary>Restore user's avatar</summary>
        void RestoreAvatar();
        /// <summary>Set a URL tag for the user</summary>
        void URL(String address, String text);
    }
}
