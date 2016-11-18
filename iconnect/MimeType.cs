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

namespace iconnect
{
    /// <summary>File Mime Types</summary>
    public enum MimeType : byte
    {
        /// <summary>Mime</summary>
        ARES_MIME_OTHER = 0,
        /// <summary>Mime</summary>
        ARES_MIME_MP3 = 1,
        /// <summary>Mime</summary>
        ARES_MIME_AUDIOOTHER1 = 2,
        /// <summary>Mime</summary>
        ARES_MIME_SOFTWARE = 3,
        /// <summary>Mime</summary>
        ARES_MIME_AUDIOOTHER2 = 4,
        /// <summary>Mime</summary>
        ARES_MIME_VIDEO = 5,
        /// <summary>Mime</summary>
        ARES_MIME_DOCUMENT = 6,
        /// <summary>Mime</summary>
        ARES_MIME_IMAGE = 7
    }
}
