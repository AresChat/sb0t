using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using captcha;

namespace core
{
    public class ServerCore
    {
        public static event EventHandler<ServerLogEventArgs> LogUpdate;

        private TcpListener tcp;
        private Thread thread;
        private bool terminate = false;

        public static void Log(String message)
        {
            LogUpdate(null, new ServerLogEventArgs { Message = message });
        }

        public static void Log(String message, Exception e)
        {
            LogUpdate(null, new ServerLogEventArgs { Message = message, Error = e });
        }
        
        public bool Open(ServerCredentials credentials)
        {
            Settings.Set("port", credentials.Port);
            Settings.Set("name", credentials.Name);
            Settings.Set("topic", credentials.Topic);
            Settings.Set("bot", credentials.Bot);

            this.tcp = new TcpListener(new IPEndPoint(IPAddress.Any, Settings.Get<ushort>("port")));
            
            try
            {
                this.tcp.Start();
            }
            catch (Exception e)
            {
                LogUpdate(this, new ServerLogEventArgs { Message = "TCP Listener", Error = e });
                return false;
            }

            LogUpdate(this, new ServerLogEventArgs { Message = "Server initialized" });
            this.thread = new Thread(new ThreadStart(this.ServerThread));
            this.thread.Start();

            return true;
        }

        public void Close()
        {
            this.terminate = true;

            try { this.tcp.Stop(); }
            catch { }

            UserPool.Destroy();
        }

        private void ServerThread()
        {
            this.terminate = false;
            
            UserPool.Build();
            Time.Reset();
            Captcha.Initialize();

            while (true)
            {
                if (this.terminate)
                    break;

                uint time = Time.Now;
                this.CheckTCPListener(time);
                this.ServiceSockets(time);
                Thread.Sleep(25);
            }
        }

        private void CheckTCPListener(uint time)
        {
            while (this.tcp.Pending())
                UserPool.CreateAresClient(this.tcp.AcceptSocket(), time);
        }

        private void ServiceSockets(uint time)
        {
            foreach (AresClient client in UserPool.AUsers)
                if (!String.IsNullOrEmpty(client.DNS))
                {
                    TCPPacket packet = null;

                    while ((packet = client.NextReceivedPacket) != null)
                        if (packet.Msg == TCPMsg.MSG_CHAT_CLIENTCOMPRESSED)
                            client.InsertUnzippedData(Zip.Decompress(packet.Packet.ToArray()));
                        else
                            try
                            {
                                TCPProcessor.Eval(client, packet, time);
                            }
                            catch (Exception e)
                            {
                                client.Disconnect();
                                Log("packet read fail from " + client.ID, e);
                                break;
                            }

                    client.SendReceive();
                    client.EnforceRules(time);
                }

            UserPool.AUsers.FindAll(x => !x.SocketConnected).ForEach(x => x.Disconnect());
            UserPool.AUsers.RemoveAll(x => !x.SocketConnected);
        }
    }

    public class ServerCredentials
    {
        public String Name { get; set; }
        public String Topic { get; set; }
        public ushort Port { get; set; }
        public String Bot { get; set; }
    }

    public class ServerLogEventArgs : EventArgs
    {
        public String Message { get; set; }
        public Exception Error { get; set; }
    }
}
