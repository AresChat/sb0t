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
    class RoomInfo
    {
        private static uint last = 0;

        private static void Show()
        {
            Server.Print(Template.Text(Category.RoomInfo, 0));
            Server.Print(String.Empty);
            int counter = 0;
            Server.Users.Ares(x => { if (x.Level == ILevel.Host) counter++; });
            Server.Users.Web(x => { if (x.Level == ILevel.Host) counter++; });
            Server.Print(Template.Text(Category.RoomInfo, 1).Replace("+n", counter.ToString()));
            counter = 1;
            Server.Users.Ares(x => { counter++; });
            Server.Users.Web(x => { counter++; });
            Server.Print(Template.Text(Category.RoomInfo, 2).Replace("+n", counter.ToString()));
            counter = 0;
            Server.Users.Ares(x => { if (x.Level > ILevel.Regular) counter++; });
            Server.Users.Web(x => { if (x.Level > ILevel.Regular) counter++; });
            Server.Print(Template.Text(Category.RoomInfo, 3).Replace("+n", counter.ToString()));
            Server.Print(Template.Text(Category.RoomInfo, 4).Replace("+n", Helpers.GetUptime));
            String status = Settings.Status;

            if (String.IsNullOrEmpty(status))
                status = String.Empty;

            Server.Print(Template.Text(Category.RoomInfo, 5).Replace("+n", status));
            Server.Print(String.Empty);
            Server.Print(Template.Text(Category.RoomInfo, 0));
        }

        public static void ForceUpdate()
        {
            last = 1201;
        }

        public static void Tick(uint time)
        {
            if (time > (last + 1200)) // 20 minutes
            {
                last = time;

                if (Settings.RoomInfo)
                    Show();
            }
        }
    }
}
