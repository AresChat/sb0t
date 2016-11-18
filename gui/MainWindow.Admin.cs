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

namespace gui
{
    partial class MainWindow
    {
        private List<Command> commands = new List<Command>();

        private void AdminCommandSetup()
        {
            listView1.ItemsSource = commands;
            ICommandDefault[] list = this.server.DefaultCommandLevels;

            foreach (ICommandDefault c in this.server.DefaultCommandLevels)
            {
                ILevel l = CommandManager.GetLevel(c.Name);

                if (l == (ILevel)255)
                {
                    l = c.Level;
                    CommandManager.SetLevel(c.Name, c.Level);
                }

                commands.Add(new Command { Level = CommandManager.LevelToString(l), Name = c.Name });
            }
        }        
    }
}
