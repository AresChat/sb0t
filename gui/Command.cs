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
using System.Windows;

namespace gui
{
    public class Command : DependencyObject
    {
        public static readonly DependencyProperty NameProperty =
          DependencyProperty.Register("Name", typeof(String),
         typeof(Command), new UIPropertyMetadata(null));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty LevelProperty =
           DependencyProperty.Register("Level", typeof(String),
          typeof(Command), new UIPropertyMetadata(null));

        public string Level
        {
            get { return (string)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        public String[] Options
        {
            get
            {
                return new String[]
                {
                    "Regular",
                    "Moderator",
                    "Administrator",
                    "Host",
                    "Disabled"
                };
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!MainWindow.SETTING_UP)
                CommandManager.UpdateCommand(this.Name, this.Level);

            base.OnPropertyChanged(e);
        }
    }
}
