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
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace core.Udp
{
    class UdpNodeManager
    {
        private static List<UdpNode> Nodes { get; set; }
        private static String DataPath { get; set; }

        public static UdpNode Find(Predicate<UdpNode> condition)
        {
            return Nodes.Find(condition);
        }

        public static void Initialize()
        {
            DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
               "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName;

            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);

            DataPath += "\\nodes.xml";

            if (!LoadList())
                LoadDefaultList();
        }

        public static void Add(EndPoint ep)
        {
            UdpNode node = new UdpNode();
            node.IP = ((IPEndPoint)ep).Address;
            node.Port = (ushort)((IPEndPoint)ep).Port;
            Add(node);
        }

        public static void Add(UdpNode server)
        {
         
            if (!server.IP.Equals(Settings.ExternalIP))
            {
                UdpNode sobj = Nodes.Find(s => s.IP.Equals(server.IP));

                if (sobj != null)
                    sobj.Port = server.Port;
                else
                {
                    if (Nodes.Count > 4000)
                    {
                        Nodes.Sort((x, y) => x.LastConnect.CompareTo(y.LastConnect));
                        Nodes.RemoveRange(0, 250);
                    }

                    Nodes.Add(server);
                }
            }
        }

        public static void Expire(ulong time)
        {
            Nodes.RemoveAll(s => s.Try > 4 &&
                (time - s.LastSentIPS) > 60000 &&
                (time - s.LastConnect) > 3600000);

            if (Nodes.Count < 10)
            {
                ServerCore.Log("local node list expired");
                LoadDefaultList();
            }
        }

        public static void Update(ulong time)
        {
            Nodes.ForEachWhere(x => x.Ack = 40000, x => x.Ack > 65000);

            try
            {
                var linq = from x in Nodes
                           where x.Ack > 0 && (time - x.LastConnect) < 1800000
                           orderby x.Ack descending
                           select x;

                int counter = 0;
                XDocument xml = new XDocument(new XElement("NodeList"));

                foreach (UdpNode i in linq)
                {
                    xml.Element("NodeList").Add(new XElement("node",
                        new XElement("ip", i.IP),
                        new XElement("port", i.Port),
                        new XElement("ack", i.Ack > 65000 ? 40000 : i.Ack)));

                    if (++counter == 100)
                        break;
                }

                xml.Save(DataPath);
            }
            catch { }

            if (UdpStats.ACKIPS == 0)
                LoadDefaultList();
        }

        private static bool LoadList()
        {
            try
            {
                XDocument xml = XDocument.Load(DataPath);

                Nodes = (from i in xml.Descendants("node")
                         select new UdpNode
                         {
                             IP = IPAddress.Parse(i.Element("ip").Value),
                             Port = ushort.Parse(i.Element("port").Value),
                             Ack = int.Parse(i.Element("ack").Value)
                         }).ToList<UdpNode>();


                if (Nodes.Count == 0)
                    return false;

                ServerCore.Log("local node list loaded [" + Nodes.Count + "]");
                return true;
            }
            catch { }

            return false;
        }

        private static void LoadDefaultList()
        {
            Nodes = new List<UdpNode>();

            using (FileStream fs = new FileStream("servers.dat", FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[6];

                while (fs.Read(buffer, 0, buffer.Length) == 6)
                {
                    UdpNode node = new UdpNode();
                    node.IP = new IPAddress(buffer.Take(4).ToArray());
                    node.Port = BitConverter.ToUInt16(buffer.ToArray(), 4);
                    Nodes.Add(node);
                }
            }

            ServerCore.Log("default node list loaded");
        }

        public static UdpNode[] GetServers(int max_servers, ulong time)
        {
            Nodes.Randomize();

            List<UdpNode> results = Nodes.FindAll(s => s.Ack > 0
                && (s.LastConnect + 900000) > time);

            if (results.Count > max_servers)
                results = results.GetRange(0, max_servers);

            return results.ToArray();
        }

        public static UdpNode[] GetServers(IPAddress target_ip, int max_servers, ulong time)
        {
            Nodes.Randomize();

            List<UdpNode> results = Nodes.FindAll(s => !s.IP.Equals(target_ip)
                && s.Ack > 0 && (s.LastConnect + 900000) > time);

            if (results.Count > max_servers)
                results = results.GetRange(0, max_servers);

            return results.ToArray();
        }

        public static UdpNode[] GetServers()
        {
            return Nodes.ToArray();
        }

        public static UdpNode NextPusher(ulong time)
        {
            if (Nodes.Count == 0)
                return null;

            Nodes.Sort((x, y) => x.LastSentIPS.CompareTo(y.LastSentIPS));

            if ((Nodes[0].LastSentIPS + 900000) < time)
            {
                Nodes[0].LastSentIPS = time;
                Nodes[0].Try++;
                return Nodes[0];
            }

            return null;
        }
    }
}
