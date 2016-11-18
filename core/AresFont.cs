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
using iconnect;

namespace core
{
    class AresFont : IFont
    {
        public bool Enabled { get; set; }
        public String NameColor { get; set; }
        public String TextColor { get; set; }
        public String FontName { get; set; }
        public bool IsEmote { get; set; }
        public byte oldN { get; set; }
        public byte oldT { get; set; }
        public byte size { get; set; }
    }
}
