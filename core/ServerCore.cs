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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using captcha;
using core.Extensions;
using core.Udp;
using iconnect;
using Newtonsoft.Json;

namespace core
{
    public class ServerCore
    {
        public static event EventHandler<ServerLogEventArgs> LogUpdate;
        internal static core.LinkLeaf.LinkClient Linker;

        public ServerCore()
        {
            Events.InitializeCommandsExtension();
        }

        private TcpListenerEx tcp;
        private UdpListener udp;
        private Thread thread;
        private bool terminate = false;

        public bool Running { get; private set; }

        public iconnect.ICommandDefault[] DefaultCommandLevels
        {
            get { return Events.DefaultCommandLevels; }
        }

        public static void Log(String message)
        {
            LogUpdate(null, new ServerLogEventArgs {Message = message});
        }

        public static void Log(String message, Exception e)
        {
            LogUpdate(null, new ServerLogEventArgs {Message = message, Error = e});
        }

        public bool Open()
        {
            Settings.Reset();
            Time.Reset();
            this.tcp = new TcpListenerEx(new IPEndPoint(IPAddress.Any, Settings.Port));

            try
            {
                this.tcp.Start();
            }
            catch (Exception e)
            {
                LogUpdate(this, new ServerLogEventArgs {Message = "TCP Listener", Error = e});
                return false;
            }

            this.udp = new UdpListener(new IPEndPoint(new IPAddress(Settings.Get<byte[]>("udp_address")), Settings.Port));

            try
            {
                this.udp.Start();
            }
            catch (Exception e)
            {
                LogUpdate(this, new ServerLogEventArgs {Message = "UDP Listener", Error = e});
                return false;
            }

            LogUpdate(this,
                new ServerLogEventArgs
                {
                    Message = "Server initialized on port " + ((IPEndPoint) this.tcp.LocalEndpoint).Port
                });
            this.thread = new Thread(new ThreadStart(this.ServerThread));
            this.thread.Start();
            this.Running = true;
            Settings.RUNNING = true;

            return true;
        }

        public void Close()
        {
            Settings.RUNNING = false;
            this.Running = false;
            this.terminate = true;

            try
            {
                this.tcp.Stop();
            }
            catch
            {
            }

            try
            {
                this.udp.Stop();
            }
            catch
            {
            }

            UserPool.Destroy();
            core.LinkHub.LeafPool.Destroy();
            UdpChannelList.Stop();

            if (Linker != null)
                Linker.KillSocket();
        }

        private void ServerThread()
        {
            this.terminate = false;

            FilterImporter.DoTasks();
            ObSalt.Init();
            CaptchaManager.Load();
            FloodControl.Reset();
            Stats.Reset();
            UserPool.Build();
            core.LinkHub.LeafPool.Build();
            Captcha.Initialize();
            UserHistory.Initialize();
            AccountManager.LoadPasswords();
            BanSystem.LoadBans();
            IdleManager.Reset();
            Proxies.Start(Helpers.UnixTime);
            IgnoreManager.init();

            if (Settings.Get<bool>("roomsearch"))
                UdpChannelList.Start();

            ulong last_update_check = 0;
            ulong fast_ping_timer = Time.Now;
            ulong channel_push_timer = (Time.Now - 1200000);
            ulong reset_floods_timer = Time.Now;
            ulong room_search_timer = (Time.Now - 1800000);
            bool can_web_chat = Settings.Get<bool>("enabled", "web");
            core.LinkHub.LinkMode link_mode = (core.LinkHub.LinkMode) Settings.Get<int>("link_mode");
            Linker = new LinkLeaf.LinkClient();

            if (link_mode == LinkHub.LinkMode.Hub)
                Linker.ConnectLocal();

            Events.ServerStarted();

            while (true)
            {
                if (this.terminate)
                    break;

                ulong time = Time.Now;

                if (time > (last_update_check + (30 * 60 * 1000)))
                {
                    last_update_check = time;
                    CheckLatestVersion();
                }

                if (time > (fast_ping_timer + 2000))
                {
                    fast_ping_timer = time;

                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.FastPing()),
                        x => x.LoggedIn && x.FastPing);

                    BanSystem.AutoClearBans();
                    Avatars.CheckAvatars(time);
                }

                if (time > (reset_floods_timer + 60000))
                {
                    reset_floods_timer = time;
                    FloodControl.Reset();
                    Proxies.Updater(Helpers.UnixTime);
                }

                this.udp.ServiceUdp(time);
                this.CheckTCPListener(time);
                this.ServiceAresSockets(time);
                this.ServiceLeaves(link_mode, time);
                this.ServiceWW();
                Linker.Service(time);

                if (can_web_chat)
                {
                    this.ServiceWebSockets(time);

                    if (time > (channel_push_timer + 1200000))
                    {
                        channel_push_timer = time;
                        ib0t.ChannelPusher.Push();
                    }
                }

                if (time > (room_search_timer + 1800000))
                {
                    room_search_timer = time;

                    if (Settings.Get<bool>("roomsearch"))
                        UdpChannelList.Update();
                }

                Events.CycleTick();
                Thread.Sleep(25);
            }
        }

        private void CheckTCPListener(ulong time)
        {
            if (this.tcp.Active)
            {
                for(int i = 0; i < 5; i++)
                {
                    if(!this.tcp.Pending())
                    {
                        break;
                    }

                    Socket sock = this.tcp.AcceptSocket();

                    if (!this.udp.IsTcpChecker(sock))
                        UserPool.CreateAresClient(sock, time);
                }
            }
        }

        private void ServiceWW()
        {
            foreach (WebWorker w in UserPool.WW)
                w.DoSocketTasks();

            UserPool.WW.RemoveAll(x => x.Dead);
        }

        private void ServiceWebSockets(ulong time)
        {
            foreach (ib0t.ib0tClient client in UserPool.WUsers)
            {
                client.SendReceive();

                if (!client.ProtoConnected)
                {
                    if (client.LoadProtocol())
                    {
                        try
                        {
                            byte[] packet = ib0t.WebSockets.Html5HandshakeReplyPacket(client.WebCredentials);
                            client.QueuePacket(packet);
                            client.ProtoConnected = true;
                            client.Time = time;
                            continue;
                        }
                        catch
                        {
                            client.SocketConnected = false;
                        }
                    }

                    if (!client.SocketConnected || (client.Time + 10000) < time)
                        client.Disconnect();
                }
                else
                {
                    if (!client.LoggedIn)
                        if (!client.SocketConnected || (client.Time + 15000) < time)
                        {
                            client.Disconnect();
                            continue;
                        }

                    String message;

                    while ((message = client.NextMessage) != null && client.SocketConnected)
                    {
                        int i = message.IndexOf(":");

                        if (i == -1)
                        {
                            Log("bad protocol from " + client.ID);
                            client.Disconnect();
                            break;
                        }

                        String ident = message.Substring(0, i);
                        String args = message.Substring(i + 1);

                        try
                        {
                            ib0t.WebProcessor.Evaluate(client, ident, args, time);
                        }
                        catch (Exception e)
                        {
                            client.Disconnect();
                            Log("packet read fail from " + client.ExternalIP + " " + ident, e);
                            break;
                        }
                    }

                    if (!client.SocketConnected || (client.Time + 120000) < time)
                        client.Disconnect();
                }
            }

            UserPool.WUsers.ForEachWhere(x => x.Disconnect(), x => !x.SocketConnected);
            UserPool.WUsers.RemoveAll(x => !x.SocketConnected);
        }

        private void ServiceAresSockets(ulong time)
        {
            foreach (AresClient client in UserPool.AUsers)
                if (!String.IsNullOrEmpty(client.DNS))
                {
                    if (client.IsHTML)
                    {
                        if (Settings.Get<bool>("enabled", "web"))
                            UserPool.CreateIb0tClient(client, time);
                        else
                            client.Disconnect();
                        break;
                    }

                    if (client.IsWebWorker)
                    {
                        UserPool.CreateWW(client);
                        client.Disconnect();
                        break;
                    }

                    TCPPacket packet = null;

                    while ((packet = client.NextReceivedPacket) != null && client.SocketConnected)
                        if (packet.Msg == TCPMsg.MSG_CHAT_CLIENTCOMPRESSED)
                            client.InsertUnzippedData(Zip.Decompress(packet.Packet.ToArray()));
                        else
                            try
                            {
                                TCPProcessor.Eval(client, packet, time);

                                if (client.IsLeaf)
                                    break;
                            }
                            catch (Exception e)
                            {
                                client.Disconnect();

                                if (packet.Msg != TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL)
                                    Log("packet read fail from " + client.ExternalIP + " " + packet.Msg, e);
                                else if (packet.Packet.ToArray().Length >= 3)
                                    Log(
                                        "packet read fail from " + client.ExternalIP + " " +
                                        (TCPMsg)packet.Packet.ToArray()[2], e);
                                else
                                    Log("packet read fail from " + client.ExternalIP + " (Advanced Protocol Exploit)", e);


                                break;
                            }

                    if (client.IsLeaf)
                        continue;

                    client.SendReceive();

                    if (client.SocketConnected)
                        client.EnforceRules(time);
                }

            UserPool.AUsers.ForEachWhere(x => x.Disconnect(), x => !x.SocketConnected);
            UserPool.AUsers.RemoveAll(x => !x.SocketConnected || x.IsLeaf);
        }

        private void ServiceLeaves(core.LinkHub.LinkMode mode, ulong time)
        {
            foreach (core.LinkHub.Leaf leaf in core.LinkHub.LeafPool.Leaves)
            {
                core.LinkHub.LinkPacket packet = null;

                while ((packet = leaf.NextReceivedPacket) != null && leaf.SocketConnected)
                    try
                    {
                        core.LinkHub.HubProcessor.Eval(leaf, packet.Msg, packet.Packet, time, mode);
                    }
                    catch (Exception e)
                    {
                        leaf.Disconnect();
                        Log("packet read fail from leaf " + leaf.Ident + " " + packet.Msg, e);
                        break;
                    }

                leaf.SendReceive();

                if (leaf.SocketConnected)
                    leaf.EnforceRules(time);
            }

            core.LinkHub.LeafPool.Leaves.ForEachWhere(x => x.Disconnect(), x => !x.SocketConnected);
            core.LinkHub.LeafPool.Leaves.RemoveAll(x => !x.SocketConnected);
        }

        private static async void CheckLatestVersion()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add(
                        "User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36"
                    );

                    using (var response = await client.GetAsync(Settings.VERSION_CHECK_URL))
                    using (var content = response.Content)
                    {
                        {
                            var result = await content.ReadAsStringAsync();

                            if (result == null)
                            {
                                return;
                            }

                            dynamic json = JsonConvert.DeserializeObject(result);
                            string version = json[0].tag_name;

                            version = version.ToLower().Replace("v", "").Trim();                        

                            if (version.ToLower().Equals(Settings.VERSION_NUMBER.ToLower()))
                            {
                                return;
                            }

                            String message =
                                "A new version(v" + version + ") of sb0t is available, download it at " +
                                Settings.RELEASE_URL;

                            // ares users
                            UserPool.AUsers.ForEachWhere(x =>
                            {
                                if (x.Level >= ILevel.Host)
                                    x.Print(message);
                                x.PM(Settings.Get<String>("bot"), message);
                            }, x => x.LoggedIn && !x.Quarantined && x.Owner);

                            // web users
                            UserPool.WUsers.ForEachWhere(x =>
                            {
                                if (x.Level >= ILevel.Host)
                                    x.Print(message);
                                x.PM(Settings.Get<String>("bot"), message);
                            }, x => x.LoggedIn && !x.Quarantined && x.Owner);
                
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }
        }
    }

    public class ServerLogEventArgs : EventArgs
    {
        public String Message { get; set; }
        public Exception Error { get; set; }
    }

    class TcpListenerEx : TcpListener
    {
        public TcpListenerEx(IPEndPoint addr) : base(addr) { }
        
        public new bool Active
        {
            get
            {
                return base.Active;
            }
        } 
    }
}
