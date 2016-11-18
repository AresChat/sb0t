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
    /// <summary>User Pool</summary>
    public interface IPool
    {
        /// <summary>Find a specific user</summary>
        IUser Find(Predicate<IUser> predicate);
        /// <summary>Action all Ares clients</summary>
        void Ares(Action<IUser> action);
        /// <summary>Action all Web Browser clients</summary>
        void Web(Action<IUser> action);
        /// <summary>Action all Linked clients</summary>
        void Linked(Action<IUser> action);
        /// <summary>Action all clients</summary>
        void All(Action<IUser> action);
        /// <summary>Action all banned users</summary>
        void Banned(Action<IBan> action);
        /// <summary>Action all user records from this session</summary>
        void Records(Action<IRecord> action);
        /// <summary>Action all quarantined users</summary>
        void Quarantined(Action<IQuarantined> action);
    }
}
