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

namespace commands
{
    /// <summary>
    /// Allow this command to feature on the Admin GUI Tab
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    class CommandLevel : Attribute
    {
        public ILevel Level { get; private set; }
        public String Name { get; private set; }

        /// <summary>
        /// Allow this command to feature on the Admin GUI Tab
        /// </summary>
        /// <param name="name">Command Name</param>
        /// <param name="level">Default Level for this command if it hasn't been user assigned</param>
        public CommandLevel(String name, ILevel level)
        {
            this.Name = name;
            this.Level = level;
        }
    }

    class CommandItem : ICommandDefault
    {
        public String Name { get; set; }
        public ILevel Level { get; set; }
    }
}
