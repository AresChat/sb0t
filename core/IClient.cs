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
using iconnect;

namespace core
{
    interface IClient
    {
        bool Ares { get; }
        IFont Font { get; }
        bool SupportsHTML { get; }
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
        ILevel Level { get; set; }
        /// <summary>Current Virtual Room</summary>
        ushort Vroom { get; set; }
        /// <summary>Ghosting Flag</summary>
        bool Ghosting { get; set; }
        /// <summary>Ignore List</summary>
        List<String> IgnoreList { get; set; }
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
        /// <summary>Cloaked</summary>
        bool Cloaked { get; set; }
        /// <summary>IUser</summary>
        IUser IUser { get; }
        /// <summary>Connected</summary>
        bool Connected { get; }
        FloodRecord FloodRecord { get; }
        bool Idled { get; set; }
        ulong IdleStart { get; set; }
        bool Quarantined { get; set; }
        byte[] Password { get; set; }

        /// <summary>Send raw data to the socket</summary>
        void BinaryWrite(byte[] data);
        /// <summary>Print to this client</summary>
        void Print(object text);
        void Unquarantine();
    }
}
