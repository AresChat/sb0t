using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using captcha;
using core.Udp;

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
            LogUpdate(null, new ServerLogEventArgs { Message = message });
        }

        public static void Log(String message, Exception e)
        {
            LogUpdate(null, new ServerLogEventArgs { Message = message, Error = e });
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
                LogUpdate(this, new ServerLogEventArgs { Message = "TCP Listener", Error = e });
                return false;
            }

            this.udp = new UdpListener(new IPEndPoint(new IPAddress(Settings.Get<byte[]>("udp_address")), Settings.Port));

            try
            {
                this.udp.Start();
            }
            catch (Exception e)
            {
                LogUpdate(this, new ServerLogEventArgs { Message = "UDP Listener", Error = e });
                return false;
            }

            LogUpdate(this, new ServerLogEventArgs { Message = "Server initialized on port " + ((IPEndPoint)this.tcp.LocalEndpoint).Port });
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

            try { this.tcp.Stop(); }
            catch { }

            try { this.udp.Stop(); }
            catch { }

            UserPool.Destroy();
            core.LinkHub.LeafPool.Destroy();
            UdpChannelList.Stop();

            if (Linker != null)
                Linker.KillSocket();
        }

        private void ServerThread()
        {
            this.terminate = false;

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

            if (Settings.Get<bool>("roomsearch"))
                UdpChannelList.Start();

            ulong fast_ping_timer = Time.Now;
            ulong channel_push_timer = (Time.Now - 1200000);
            ulong reset_floods_timer = Time.Now;
            ulong room_search_timer = (Time.Now - 1800000);
            bool can_web_chat = Settings.Get<bool>("enabled", "web");
            core.LinkHub.LinkMode link_mode = (core.LinkHub.LinkMode)Settings.Get<int>("link_mode");
            Linker = new LinkLeaf.LinkClient();

            if (link_mode == LinkHub.LinkMode.Hub)
                Linker.ConnectLocal();

            Events.ServerStarted();

            while (true)
            {
                if (this.terminate)
                    break;

                ulong time = Time.Now;

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
                }

                this.udp.ServiceUdp(time);
                this.CheckTCPListener(time);
                this.ServiceAresSockets(time);
                this.ServiceLeaves(link_mode, time);
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
                while (this.tcp.Pending())
                {
                    Socket sock = this.tcp.AcceptSocket();

                    if (!this.udp.IsTcpChecker(sock))
                        UserPool.CreateAresClient(sock, time);
                }
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
                        catch { client.SocketConnected = false; }
                    }

                    if (client.SocketConnected || (client.Time + 10000) < time)
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
                            Log("packet read fail from " + client.ID + " " + ident, e);
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
                                    Log("packet read fail from " + client.ID + " " + packet.Msg, e);
                                else
                                    Log("packet read fail from " + client.ID + " " + (TCPMsg)packet.Packet.ToArray()[2], e);
                                
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
