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
    /// <summary>Private Message</summary>
    public interface IPrivateMsg
    {
        /// <summary>Check if PM contains specific text</summary>
        bool Contains(String text);
        /// <summary>Replace text in PM with new text</summary>
        void Replace(String oldText, String newText);
        /// <summary>Remove specific text from PM</summary>
        void Remove(String text);
        /// <summary>Prevent PM from being sent</summary>
        bool Cancel { get; set; }
    }
}
