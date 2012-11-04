using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Data.SQLite;

namespace core.Udp
{
    class UdpChannelList
    {
        private static bool terminate { get; set; }
        private static ConcurrentStack<UdpChannelItem> primary_list { get; set; }

        public static void Start()
        {
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
            
        }

        private static void SaveLocal()
        {

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
                    return;

                long now = sw.ElapsedMilliseconds;

                if (now > (last_action + 15000))
                {
                    primary_list.Clear();
                    primary_list.PushRange(servers_ponged.ToArray());

                    servers_to_ping.Clear();
                    servers_been_pinged.Clear();
                    servers_ponged.Clear();

                    try { sock.Shutdown(SocketShutdown.Both); }
                    catch { }
                    try { sock.Close(); }
                    catch { }

                    SaveLocal();
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
                                if (servers_to_ping.Find(x => x.IP.Equals(s.Address)) == null)
                                    if (servers_been_pinged.Find(x => x.IP.Equals(s.Address)) == null)
                                        servers_to_ping.Add(new UdpChannelItem(new UdpNode { IP = s.Address, Port = (ushort)s.Port }));

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
