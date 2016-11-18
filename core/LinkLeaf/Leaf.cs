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

namespace core.LinkLeaf
{
    class Leaf : iconnect.ILeaf
    {
        public uint Ident { get; set; }
        public String Name { get; set; }
        public IPAddress ExternalIP { get; set; }
        public ushort Port { get; set; }
        public List<LinkUser> Users { get; set; }

        public Leaf()
        {
            this.Users = new List<LinkUser>();
        }

        public void ForEachUser(Action<iconnect.IUser> action)
        {
            this.Users.ForEach(action);
        }

        public void Print(String text)
        {
            if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLogin.Ready)
                ServerCore.Linker.SendPacket(LeafOutbound.LeafPrintAll(ServerCore.Linker, this.Ident, text));
        }

        public void Print(ushort vroom, String text)
        {
            if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLogin.Ready)
                ServerCore.Linker.SendPacket(LeafOutbound.LeafPrintVroom(ServerCore.Linker, this.Ident, vroom, text));
        }

        public void Print(iconnect.ILevel level, String text)
        {
            if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLogin.Ready)
                ServerCore.Linker.SendPacket(LeafOutbound.LeafPrintLevel(ServerCore.Linker, this.Ident, level, text));
        }

        public void SendText(String sender, String text)
        {
            if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLogin.Ready)
                ServerCore.Linker.SendPacket(LeafOutbound.LeafPublicTextToLeaf(ServerCore.Linker, this.Ident, sender, text));
        }

        public void SendEmote(String sender, String text)
        {
            if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLogin.Ready)
                ServerCore.Linker.SendPacket(LeafOutbound.LeafEmoteTextToLeaf(ServerCore.Linker, this.Ident, sender, text));
        }

        public void Scribble(String sender, byte[] img, int height)
        {
            if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLogin.Ready)
                ServerCore.Linker.SendPacket(LeafOutbound.LeafScribbleLeaf(ServerCore.Linker, this.Ident, sender, height, img));
        }
    }
}
