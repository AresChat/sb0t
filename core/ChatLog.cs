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
using System.IO;

namespace core
{
    class ChatLog
    {
        public static void WriteLine(String text)
        {
            if (Settings.Get<bool>("logging"))
            {
                try
                {
                    DateTime d = DateTime.Now;

                    String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                      "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\ChatLogs";

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    path += ("\\" + d.Day + " " + d.Month + " " + d.Year + ".txt");

                    using (StreamWriter writer = File.Exists(path) ? File.AppendText(path) : File.CreateText(path))
                        writer.WriteLine(d.ToShortTimeString() + " " + text);
                }
                catch { }
            }
        }
    }
}
