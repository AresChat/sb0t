using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace core
{
    interface IClient
    {
        /// <summary>Session identity</summary>
        ushort ID { get; }
        /// <summary>External IP Address</summary>
        IPAddress ExternalIP { get; set; }
        /// <summary>DNS Host Name</summary>
        String DNS { get; set; }
        /// <summary>16 byte GUID</summary>
        Guid Guid { get; set; }
        /// <summary>File Count</summary>
        ushort FileCount { get; set; }
        /// <summary>Data Port</summary>
        ushort DataPort { get; set; }
        /// <summary>Super Node IP Address</summary>
        IPAddress NodeIP { get; set; }
        /// <summary>Super Node Port</summary>
        ushort NodePort { get; set; }
        /// <summary>Current User Name</summary>
        String Name { get; set; }
        /// <summary>Original User Name</summary>
        String OrgName { get; set; }
        /// <summary>Client Version</summary>
        String Version { get; }
        /// <summary>Local IP Address</summary>
        IPAddress LocalIP { get; set; }
        /// <summary>Browse Flag</summary>
        bool Browsable { get; set; }
        /// <summary>Age</summary>
        byte Age { get; set; }
        /// <summary>Gender</summary>
        byte Sex { get; set; }
        /// <summary>Country Code</summary>
        byte Country { get; set; }
        /// <summary>Regional Location</summary>
        String Region { get; set; }
        /// <summary>Keep Alive Protocol Flag</summary>
        bool FastPing { get; set; }
        /// <summary>Admin Level</summary>
        Level Level { get; set; }
        /// <summary>Current Virtual Room</summary>
        ushort Vroom { get; set; }
        /// <summary>Ghosting Flag</summary>
        bool Ghosting { get; set; }
        /// <summary>Ignore List</summary>
        List<String> IgnoreList { get; set; }
        /// <summary>Custom Font</summary>
        Font Font { get; set; }
        /// <summary>Third Party Client Flag</summary>
        bool CustomClient { get; set; }
        /// <summary>Tag Data</summary>
        List<String> CustomClientTags { get; set; }
        /// <summary>Muzzle Status</summary>
        bool Muzzled { get; set; }
        /// <summary>Custom Name</summary>
        String CustomName { get; set; }
        /// <summary>Web Client Flag</summary>
        bool WebClient { get; }
        /// <summary>Room Owner Flag</summary>
        bool Owner { get; set; }
        /// <summary>Avatar</summary>
        byte[] Avatar { get; set; }
        /// <summary>Personal Message</summary>
        String PersonalMessage { get; set; }
        /// <summary>Encryption Credentials</summary>
        Encryption Encryption { get; set; }
        /// <summary>Capture Test Flag</summary>
        bool Captcha { get; set; }
        /// <summary>Logged In Flag</summary>
        bool Registered { get; set; }
        /// <summary>Secure Login Cookie</summary>
        uint Cookie { get; set; }
        /// <summary>Captcha Word</summary>
        String CaptchaWord { get; set; }

        /// <summary>Send raw data to the socket</summary>
        void BinaryWrite(byte[] data);
        /// <summary>Print to this client</summary>
        void Print(object text);
    }
}
