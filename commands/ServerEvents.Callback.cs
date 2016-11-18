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
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using iconnect;

namespace commands
{
    public partial class ServerEvents
    {
        public ServerEvents(IHostApp cb)
        {
            Server.SetCallback(cb);
        }

        public ICommandDefault[] DefaultCommandLevels
        {
            get
            {
                List<CommandItem> list = new List<CommandItem>();

                foreach (MethodInfo m in typeof(Eval).GetMethods())
                {
                    CommandLevel c = (CommandLevel)Attribute.GetCustomAttribute(m, typeof(CommandLevel), false);

                    if (c != null)
                        if (list.Find(x => x.Name == c.Name) == null)
                            list.Add(new CommandItem
                            {
                                Level = c.Level,
                                Name = c.Name
                            });
                }

                return list.ToArray();
            }
        }

        public BitmapSource Icon
        {
            get { return null; }
        }

        public UserControl GUI
        {
            get { return null; }
        }

        public void Dispose() { }
        public void Load() { }
    }
}
