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
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Data.SQLite;
using iconnect;

namespace core.Udp
{
    class UdpChannelList
    {
        private static bool terminate { get; set; }
        private static ConcurrentStack<UdpChannelItem> primary_list { get; set; }
        private static String DataPath { get; set; }

        public static bool Available
        {
            get
            {
                if (primary_list == null)
                    return false;

                return primary_list.Count > 0;
            }
        }

        public static void ForEach(Action<IChannelItem> action)
        {
            if (primary_list != null)
            {
                List<UdpChannelItem> items = primary_list.ToList();

                foreach (UdpChannelItem i in items)
                    action(i);
            }
        }

        public static void Start()
        {
            DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
               "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\Dat";

            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);

            DataPath += "\\roomsearch.dat";

            primary_list = new ConcurrentStack<UdpChannelItem>();
            terminate = false;
            LoadLocal();
        }

        public static void Stop()
        {
            terminate = true;
        }

        public static void Update()
        {
            UdpNode[] nodes = UdpNodeManager.GetServers();

            if (nodes.Length > 0)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(BackgroundWorker));
                thread.Start(nodes);
            }
        }

        private static void LoadLocal()
        {
            if (!File.Exists(DataPath))
                return;

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand("select * from roomsearch", connection))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                            primary_list.Push(new UdpChannelItem
                            {
                                Name = (String)reader["name"],
                                Topic = (String)reader["topic"],
                                Version = (String)reader["version"],
                                IP = IPAddress.Parse((String)reader["ip"]),
                                Port = (ushort)(int)reader["port"],
                                Users = (ushort)(int)reader["users"],
                                Language = (byte)(int)reader["language"]
                            });
                }
            }
            catch { }
        }

        private static void SaveLocal()
        {
            CreateDatabase();

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();
                UdpChannelItem[] rooms = primary_list.ToArray();

                foreach (UdpChannelItem item in rooms)
                {
                    String query = @"insert into roomsearch (name, topic, version, ip, port, users, language) 
                                 values (@name, @topic, @version, @ip, @port, @users, @language)";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.Add(new SQLiteParameter("@name", item.Name));
                        command.Parameters.Add(new SQLiteParameter("@topic", item.Topic));
                        command.Parameters.Add(new SQLiteParameter("@version", item.Version));
                        command.Parameters.Add(new SQLiteParameter("@ip", item.IP.ToString()));
                        command.Parameters.Add(new SQLiteParameter("@port", (int)item.Port));
                        command.Parameters.Add(new SQLiteParameter("@users", (int)item.Users));
                        command.Parameters.Add(new SQLiteParameter("@language", (int)item.Language));
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void CreateDatabase()
        {
            if (File.Exists(DataPath))
                File.Delete(DataPath);

            SQLiteConnection.CreateFile(DataPath);

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=\"" + DataPath + "\""))
            {
                connection.Open();

                String query = @"create table roomsearch
                                 (
                                     name text not null,
                                     topic text not null,
                                     version text not null,
                                     ip text not null,
                                     port int not null,
                                     users int not null,
                                     language int not null
                                 )";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    command.ExecuteNonQuery();
            }
        }

        private static void BackgroundWorker(object args)
        {
            UdpNode[] nodes = (UdpNode[])args;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            byte[] OP_SERVERLIST_SENDINFO = new byte[] { 2 };

            List<UdpChannelItem> servers_to_ping = new List<UdpChannelItem>();
            List<UdpChannelItem> servers_been_pinged = new List<UdpChannelItem>();
            List<UdpChannelItem> servers_ponged = new List<UdpChannelItem>();

            EndPoint addr = new IPEndPoint(IPAddress.Any, 0);
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.Blocking = false;

            foreach (UdpNode node in nodes)
                servers_to_ping.Add(new UdpChannelItem(node));

            long last_action = sw.ElapsedMilliseconds;

            while (true)
            {
                if (terminate)
                {
                    sw.Stop();
                    return;
                }

                long now = sw.ElapsedMilliseconds;

                if (now > (last_action + 15000))
                {
                    sw.Stop();

                    if (servers_ponged.Count > 10)
                    {
                        primary_list.Clear();
                        primary_list.PushRange(servers_ponged.ToArray());
                        SaveLocal();
                    }

                    servers_to_ping.Clear();
                    servers_been_pinged.Clear();
                    servers_ponged.Clear();

                    try { sock.Shutdown(SocketShutdown.Both); }
                    catch { }
                    try { sock.Close(); }
                    catch { }

                    break;
                }

                if (servers_to_ping.Count > 0) // send
                {
                    UdpChannelItem i = servers_to_ping[0];
                    bool attempted = true;

                    try
                    {
                        sock.SendTo(OP_SERVERLIST_SENDINFO, new IPEndPoint(i.IP, i.Port));
                    }
                    catch (SocketException e)
                    {
                        attempted = e.SocketErrorCode != SocketError.WouldBlock;
                    }
                    catch { }

                    if (attempted)
                    {
                        servers_to_ping.RemoveAt(0);
                        servers_been_pinged.Add(i);
                        last_action = now;
                    }
                }

                while (sock.Available > 0) // receive
                {
                    byte[] buf = new byte[4096];
                    int size = 0;

                    try { size = sock.ReceiveFrom(buf, ref addr); }
                    catch { }

                    if (size > 0)
                    {
                        last_action = now;
                        buf = buf.Take(size).ToArray();

                        try
                        {
                            UdpChannelItem channel = new UdpChannelItem(addr, buf);                                             

                            foreach (IPEndPoint s in channel.Servers)
                            {
                                if (servers_to_ping.Find(x => x.IP.Equals(s.Address)) == null)
                                    if (servers_been_pinged.Find(x => x.IP.Equals(s.Address)) == null)
                                        servers_to_ping.Add(new UdpChannelItem(new UdpNode { IP = s.Address, Port = (ushort)s.Port }));
                            }

                            if (channel.Users > 0)
                                servers_ponged.Add(channel);
                        }
                        catch { }
                    }
                    else break;
                }

                Thread.Sleep(100);
            }
        }
    }
}
