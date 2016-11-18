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
using System.Net.Sockets;
using core.ib0t;

namespace core
{
    class UserPool
    {
        public static List<AresClient> AUsers;
        public static List<ib0t.ib0tClient> WUsers;
        public static List<WebWorker> WW;

        public static void Build()
        {
            AUsers = new List<AresClient>();
            WUsers = new List<ib0t.ib0tClient>();
            WW = new List<WebWorker>();
        }

        public static void Destroy()
        {
            try
            {
                AUsers.ForEach(x => x.LoggedIn = false);
                AUsers.ForEach(x => x.Disconnect());
                AUsers.Clear();
                AUsers = null;
            }
            catch { }

            try
            {
                WUsers.ForEach(x => x.LoggedIn = false);
                WUsers.ForEach(x => x.Disconnect());
                WUsers.Clear();
                WUsers = null;
            }
            catch { }

            try
            {
                WW.ForEach(x => x.Disconnect());
                WW.Clear();
                WW = null;
            }
            catch { }
        }

        public static void CreateAresClient(Socket sock, ulong time)
        {
            try
            {
                for (ushort u = 0; u < ushort.MaxValue; u++)
                {
                    int index = AUsers.FindIndex(x => x.ID == u);

                    if (index == -1)
                    {
                        AUsers.Add(new AresClient(sock, time, u));
                        AUsers.Sort((x, y) => x.ID.CompareTo(y.ID));
                        break;
                    }
                }
            }
            catch(Exception e)
            {
                ServerCore.Log(e.ToString());
            }
        }

        public static void CreateIb0tClient(AresClient client, ulong time)
        {
            try
            {
                for (ushort u = 700; u < ushort.MaxValue; u++)
                {
                    int index = WUsers.FindIndex(x => x.ID == u);

                    if (index == -1)
                    {
                        WUsers.Add(new ib0tClient(client, time, u));
                        WUsers.Sort((x, y) => x.ID.CompareTo(y.ID));
                        client.Sock = null;
                        AUsers.RemoveAll(x => x.ID == client.ID);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                ServerCore.Log(e.ToString());

            }
        }

        public static void CreateWW(AresClient client)
        {
            WW.Add(new WebWorker(client));
            client.Sock = null;
            AUsers.RemoveAll(x => x.ID == client.ID);
        }

        public static ushort UserCount
        {
            get
            {
                ushort result = (ushort)AUsers.FindAll(x => x.LoggedIn).Count;
                result += (ushort)WUsers.FindAll(x => x.LoggedIn).Count;
                return result;
            }
        }
    }
}
