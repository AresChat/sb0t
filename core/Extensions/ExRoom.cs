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

namespace core.Extensions
{
    class ExRoom : IRoom
    {
        public String Version
        {
            get { return Settings.VERSION; }
        }

        public byte MinimumAge
        {
            get
            {
                if (!Settings.Get<bool>("age_restrict"))
                    return 0;

                return (byte)Settings.Get<int>("age_restrict_value");
            }
        }

        public bool CustomNamesEnabled
        {
            get { return Settings.Get<bool>("customnames"); }
            set { Settings.Set("customnames", value); }
        }

        public String BotName
        {
            get { return Settings.Get<String>("bot"); }
        }

        public void ClearURL()
        {
            Settings.Set("text", String.Empty, "url");
            Settings.Set("link", String.Empty, "url");
            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Url(x, String.Empty, String.Empty)), x => x.LoggedIn);
            UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.UrlTo(x, String.Empty, String.Empty)), x => x.LoggedIn);
        }

        public void UpdateURL(String address, String text)
        {
            Settings.Set("text", text, "url");
            Settings.Set("link", address, "url");
            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Url(x, address, text)), x => x.LoggedIn);
            UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.UrlTo(x, address, text)), x => x.LoggedIn);
        }

        public IPAddress ExternalIP
        {
            get { return Settings.ExternalIP; }
        }

        public IPAddress LocalIP
        {
            get { return Settings.LocalIP; }
        }

        public String Name
        {
            get { return Settings.Name; }
        }

        public ushort Port
        {
            get { return Settings.Port; }
        }

        public String Hashlink
        {
            get { return core.Hashlink.EncodeHashlink(new Room { IP = ExternalIP, Name = Name, Port = Port }); }
        }

        public bool IsRunning
        {
            get { return Settings.RUNNING; }
        }

        public byte Language
        {
            get { return Settings.Language; }
        }

        public String Topic
        {
            get
            {
                String str = Settings.Topic;

                if (String.IsNullOrEmpty(str))
                    return "welcome to my room";

                return str;
            }
            set
            {
                String str = value;

                while (Encoding.UTF8.GetByteCount(str) > 180)
                    str = str.Substring(0, str.Length - 1);

                Settings.Topic = str;

                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Topic(x, str)), x => x.LoggedIn && x.Vroom == 0);
                UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.TopicTo(x, str)), x => x.LoggedIn && x.Vroom == 0);
            }
        }

        public uint StartTime
        {
            get { return Stats.StartTime; }
        }
    }
}
