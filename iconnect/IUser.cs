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
using System.Linq;
using System.Text;
using System.Net;

namespace iconnect
{
    /// <summary>Chatroom Client</summary>
    public interface IUser
    {
        /// <summary>Get user's custom font</summary>
        uint LastScribble { get; set; }
        /// <summary>Get user's custom font</summary>
        IFont Font { get; }
        /// <summary>Set user admin level</summary>
        void SetLevel(ILevel level);
        /// <summary>Get user link credentials</summary>
        ILink Link { get; }
        /// <summary>Get join timestamp</summary>
        uint JoinTime { get; }
        /// <summary>Get session identity</summary>
        ushort ID { get; }
        /// <summary>Get External IP Address</summary>
        IPAddress ExternalIP { get; }
        /// <summary>Get DNS Host Name</summary>
        String DNS { get; }
        /// <summary>Get 16 byte GUID</summary>
        Guid Guid { get; }
        /// <summary>Get File Count</summary>
        ushort FileCount { get; }
        /// <summary>Get Data Port</summary>
        ushort DataPort { get; }
        /// <summary>Get Super Node IP Address</summary>
        IPAddress NodeIP { get; }
        /// <summary>Get Super Node Port</summary>
        ushort NodePort { get; }
        /// <summary>Get Current User Name</summary>
        String Name { get; set; }
        /// <summary>Get Original User Name</summary>
        String OrgName { get; }
        /// <summary>Get Client Version</summary>
        String Version { get; }
        /// <summary>Get Local IP Address</summary>
        IPAddress LocalIP { get; }
        /// <summary>Get Browse Flag</summary>
        bool Browsable { get; }
        /// <summary>Get Age</summary>
        byte Age { get; }
        /// <summary>Get Gender</summary>
        byte Sex { get; }
        /// <summary>Get Country Code</summary>
        byte Country { get; }
        /// <summary>Get Regional Location</summary>
        String Region { get; }
        /// <summary>Get Keep Alive Protocol Flag</summary>
        bool FastPing { get; }
        /// <summary>Get Admin Level</summary>
        ILevel Level { get; }
        /// <summary>Get or Set Current Virtual Room</summary>
        ushort Vroom { get; set; }
        /// <summary>Get Ghosting Flag</summary>
        bool Ghosting { get; }
        /// <summary>Get Ignore List</summary>
        List<String> IgnoreList { get; set; }
        /// <summary>Get Third Party Client Flag</summary>
        bool CustomClient { get; }
        /// <summary>Get or Tag Data</summary>
        List<String> CustomClientTags { get; set; }
        /// <summary>Get or Set Muzzle Status</summary>
        bool Muzzled { get; set; }
        /// <summary>Get or Set Custom Name</summary>
        String CustomName { get; set; }
        /// <summary>Get Web Client Flag</summary>
        bool WebClient { get; }
        /// <summary>Get Room Owner Flag</summary>
        bool Owner { get; }
        /// <summary>Get or Set Avatar</summary>
        byte[] Avatar { get; set; }
        /// <summary>Get or Set Personal Message</summary>
        String PersonalMessage { get; set; }
        /// <summary>Get Encryption Status</summary>
        bool Encrypted { get; }
        /// <summary>Get Capture Test Flag</summary>
        bool Captcha { get; }
        /// <summary>Get Logged In Flag</summary>
        bool Registered { get; }
        /// <summary>Get or Set Cloaked Status</summary>
        bool Cloaked { get; set; }
        /// <summary>Get Connected Status</summary>
        bool Connected { get; }
        /// <summary>Get Idle Status</summary>
        bool Idle { get; }
        /// <summary>Get Quarantine Status</summary>
        bool Quarantined { get; }
        /// <summary>Get HTML support status</summary>
        bool SupportsHTML { get; }

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
        /// <summary>Set custom HTML content to user</summary>
        void SendHTML(String text);
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
        /// <summary>Send a scribble to the user (if client supports this feature)</summary>
        void Scribble(String sender, byte[] img, int height);
        /// <summary>Nudge the user (if client supports this feature)</summary>
        void Nudge(String sender);
        /// <summary>Get Local Endpoint</summary>
        IPEndPoint LocalEP { get; }
    }
}
