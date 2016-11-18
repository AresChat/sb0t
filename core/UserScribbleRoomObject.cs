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

namespace core
{
    class UserScribbleRoomObject
    {
        public uint Size { get; set; }
        public ushort Chunks { get; set; }
        public ushort ChunkCount { get; set; }
        public List<byte> DataIn { get; set; }
        public bool IsReceiving { get; set; }

        public void Reset()
        {
            this.Size = 0;
            this.Chunks = 0;
            this.ChunkCount = 0;

            if (this.DataIn != null)
                this.DataIn.Clear();

            this.DataIn = new List<byte>();
            this.IsReceiving = false;
        }

        public uint ReceivedCount
        {
            get { return (uint)this.DataIn.Count; }
        }
    }
}
