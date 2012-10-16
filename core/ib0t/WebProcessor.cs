using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using captcha;
using iconnect;

namespace core.ib0t
{
    class WebProcessor
    {
        public static void Evaluate(ib0tClient client, String ident, String args, ulong time)
        {
            if (!client.LoggedIn && !ident.StartsWith("LOGIN"))
                throw new Exception("unordered login routine");

            if (client.LoggedIn && ident.StartsWith("LOGIN"))
                return;

            if (client.LoggedIn)
                if (FloodControl.IsFlooding(client, ident, Encoding.UTF8.GetBytes(args), time))
                    if (Events.Flooding(client, (byte)FloodControl.WebMsgToTCPMsg(ident)))
                    {
                        Events.Flooded(client);
                        client.Disconnect();
                        return;
                    }

            switch (ident)
            {
                case "LOGIN":
                    Login(client, args, time);
                    break;

                case "PUBLIC":
                    Text(client, args, time);
                    break;

                case "EMOTE":
                    Emote(client, args, time);
                    break;

                case "COMMAND":
                    Command(client, args);
                    break;

                case "PING":
                    client.Time = time;
                    break;

                default:
                    throw new Exception();
            }
        }

        private static void Login(ib0tClient client, String args, ulong time)
        {
            byte[] g = new byte[16];

            for (int i = 0; i < g.Length; i++)
                g[i] = byte.Parse(args.Substring((i * 2), 2), NumberStyles.HexNumber);

            client.CanScribble = g[0] == 0xff;

            using (MD5 md5 = MD5.Create())
                client.Guid = new Guid(md5.ComputeHash(g));

            client.OrgName = args.Substring(32);
            Helpers.FormatUsername(client);
            client.Name = client.OrgName;
            client.FastPing = false;
            client.FileCount = 0;
            client.DataPort = 0;
            client.NodeIP = IPAddress.Parse("0.0.0.0");
            client.NodePort = 0;
            client.Version = "ib0t web user";
            client.CustomClient = true;
            client.LocalIP = client.ExternalIP;
            client.Browsable = false;
            client.Age = 0;
            client.Sex = 0;
            client.Country = 0;
            client.Region = String.Empty;
            client.Captcha = !Settings.Get<bool>("captcha");

            if (!client.Captcha)
                client.Captcha = CaptchaManager.HasCaptcha(client);

            if ((UserPool.AUsers.FindAll(x => x.ExternalIP.Equals(client.ExternalIP)).Count +
                 UserPool.WUsers.FindAll(x => x.ExternalIP.Equals(client.ExternalIP)).Count) > 3)
            {
                Events.Rejected(client, RejectedMsg.TooManyClients);
                throw new Exception("too many clients from this ip");
            }

            if (UserHistory.IsJoinFlooding(client, time))
            {
                Events.Rejected(client, RejectedMsg.TooSoon);
                throw new Exception("joined too quickly");
            }

            IClient hijack = UserPool.AUsers.Find(x => (x.Name == client.Name ||
                x.OrgName == client.OrgName) && x.ID != client.ID);

            if (hijack == null)
                hijack = UserPool.WUsers.Find(x => (x.Name == client.Name ||
                    x.OrgName == client.OrgName) && x.ID != client.ID);

            if (hijack != null)
                if (hijack.ExternalIP.Equals(client.ExternalIP))
                {
                    if (hijack is AresClient)
                        ((AresClient)hijack).Disconnect(true);
                    else
                        ((ib0t.ib0tClient)hijack).Disconnect();
                }
                else
                {
                    Events.Rejected(client, RejectedMsg.NameInUse);
                    throw new Exception("name in use");
                }

            UserHistory.AddUser(client, time);

            if (BanSystem.IsBanned(client))
            {
                if (hijack != null && hijack is AresClient)
                    ((AresClient)hijack).SendDepart();

                Events.Rejected(client, RejectedMsg.Banned);
                throw new Exception("banned user");
            }

            if (Proxies.Check(client))
                if (Events.ProxyDetected(client))
                {
                    if (hijack != null && hijack is AresClient)
                        ((AresClient)hijack).SendDepart();

                    Events.Rejected(client, RejectedMsg.Proxy);
                    throw new Exception("proxy detected");
                }

            if (!Events.Joining(client))
            {
                if (hijack != null && hijack is AresClient)
                    ((AresClient)hijack).SendDepart();

                Events.Rejected(client, RejectedMsg.UserDefined);
                throw new Exception("user defined rejection");
            }

            client.Quarantined = !client.Captcha && Settings.Get<int>("captcha_mode") == 1;

            if (!client.Quarantined)
            {
                if (hijack == null || !(hijack is AresClient))
                {
                    LinkLeaf.LinkUser other = null;

                    if (ServerCore.Linker.Busy)
                        foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                        {
                            other = leaf.Users.Find(x => x.Vroom == client.Vroom && x.Name == client.Name && x.Link.Visible);

                            if (other != null)
                            {
                                other.LinkCredentials.Visible = false;
                                break;
                            }
                        }

                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(other == null ? TCPOutbound.Join(x, client) : TCPOutbound.UpdateUserStatus(x, client)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                    UserPool.WUsers.ForEachWhere(x => x.QueuePacket(other == null ? ib0t.WebOutbound.JoinTo(x, client.Name, client.Level) : ib0t.WebOutbound.UpdateTo(x, client.Name, client.Level)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);
                }

                client.LoggedIn = true;
                client.QueuePacket(WebOutbound.AckTo(client, client.Name));
                client.QueuePacket(WebOutbound.TopicFirstTo(client, Settings.Get<String>("topic")));
                client.QueuePacket(WebOutbound.UserlistItemTo(client, Settings.Get<String>("bot"), ILevel.Host));

                UserPool.AUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.UserlistItemTo(client, x.Name, x.Level)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                UserPool.WUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.UserlistItemTo(client, x.Name, x.Level)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                client.QueuePacket(WebOutbound.UserlistEndTo(client));
                client.QueuePacket(WebOutbound.UrlTo(client, Settings.Get<String>("link", "url"), Settings.Get<String>("text", "url")));

                UserPool.AUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.FontTo(client, x.Name, x.Font.NameColor, x.Font.TextColor)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && x.Font.HasFont && !x.Quarantined);

                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Avatar(x, client)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.PersonalMessage(x, client)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                FloodControl.Remove(client);

                if (client.SocketConnected)
                    IdleManager.Set(client);

                if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                    ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafJoin(ServerCore.Linker, client));

                Events.Joined(client);
            }
            else
            {
                if (hijack != null && hijack is AresClient)
                    ((AresClient)hijack).SendDepart();

                client.LoggedIn = true;
                client.QueuePacket(WebOutbound.AckTo(client, client.Name));
                client.QueuePacket(WebOutbound.TopicFirstTo(client, Settings.Get<String>("topic")));
                client.QueuePacket(WebOutbound.UserlistEndTo(client));

                CaptchaItem cap = Captcha.Create();
                client.CaptchaWord = cap.Word;
                Events.CaptchaSending(client);
                client.QueuePacket(WebOutbound.NoSuchTo(client, String.Empty));

                foreach (String str in cap.Lines)
                    client.QueuePacket(WebOutbound.NoSuchTo(client, str));

                client.QueuePacket(WebOutbound.NoSuchTo(client, String.Empty));
            }
        }

        private static void Text(ib0tClient client, String args, ulong time)
        {
            String text = args;

            if (text.StartsWith("#login") || text.StartsWith("#register"))
            {
                Command(client, text.Substring(1));
                return;
            }

            if (text.StartsWith("#") && client.SocketConnected)
                Command(client, text.Substring(1));

            if (client.SocketConnected)
            {
                if (!client.Captcha)
                {
                    if (String.IsNullOrEmpty(client.CaptchaWord) || (client.CaptchaWord.Length > 0 && client.CaptchaWord.ToUpper() != text.ToUpper()))
                    {
                        if (client.CaptchaWord.Length > 0 && client.CaptchaWord.ToUpper() != text.ToUpper())
                        {
                            Events.CaptchaReply(client, text);

                            if (!client.SocketConnected)
                                return;
                        }

                        CaptchaItem cap = Captcha.Create();
                        client.CaptchaWord = cap.Word;
                        Events.CaptchaSending(client);
                        client.QueuePacket(WebOutbound.NoSuchTo(client, String.Empty));

                        foreach (String str in cap.Lines)
                            client.QueuePacket(WebOutbound.NoSuchTo(client, str));

                        client.QueuePacket(WebOutbound.NoSuchTo(client, String.Empty));
                        return;
                    }
                    else
                    {
                        client.Captcha = true;
                        Events.CaptchaReply(client, text);
                        CaptchaManager.AddCaptcha(client);

                        if (client.Quarantined)
                            client.Unquarantine();

                        return;
                    }
                }
                else Events.TextReceived(client, text);
            }
            else return;

            if (client.SocketConnected)
            {
                text = Events.TextSending(client, text);

                if (!String.IsNullOrEmpty(text) && client.SocketConnected && !client.Muzzled)
                {
                    if (client.Idled)
                    {
                        uint seconds_away = (uint)((Time.Now - client.IdleStart) / 1000);
                        IdleManager.Remove(client);
                        Events.Unidled(client, seconds_away);
                    }

                    if (client.SocketConnected)
                    {
                        UserPool.AUsers.ForEachWhere(x => x.SendPacket(String.IsNullOrEmpty(client.CustomName) ?
                            TCPOutbound.Public(x, client.Name, text) : TCPOutbound.NoSuch(x, client.CustomName + text)),
                            x => x.LoggedIn && x.Vroom == client.Vroom && !x.IgnoreList.Contains(client.Name) && !x.Quarantined);

                        UserPool.WUsers.ForEachWhere(x => x.QueuePacket(String.IsNullOrEmpty(client.CustomName) ?
                            ib0t.WebOutbound.PublicTo(x, client.Name, text) : ib0t.WebOutbound.NoSuchTo(x, client.CustomName + text)),
                            x => x.LoggedIn && x.Vroom == client.Vroom && !x.IgnoreList.Contains(client.Name) && !x.Quarantined);

                        if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                            ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafPublicText(ServerCore.Linker, client.Name, text));

                        Events.TextSent(client, text);
                    }
                }
            }
        }

        private static void Emote(ib0tClient client, String args, ulong time)
        {
            if (!client.Captcha)
                return;

            String text = args;

            Events.EmoteReceived(client, text);

            if (client.SocketConnected)
            {
                text = Events.EmoteSending(client, text);

                if (!String.IsNullOrEmpty(text) && client.SocketConnected && !client.Muzzled)
                {
                    if (client.Idled)
                    {
                        uint seconds_away = (uint)((Time.Now - client.IdleStart) / 1000);
                        IdleManager.Remove(client);
                        Events.Unidled(client, seconds_away);
                    }

                    if (client.SocketConnected)
                    {
                        if (text.StartsWith("idles"))
                        {
                            IdleManager.Add(client);
                            Events.Idled(client);
                        }

                        if (client.SocketConnected)
                        {
                            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Emote(x, client.Name, text)),
                                x => x.LoggedIn && x.Vroom == client.Vroom && !x.IgnoreList.Contains(client.Name) && !x.Quarantined);

                            UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.EmoteTo(x, client.Name, text)),
                                x => x.LoggedIn && x.Vroom == client.Vroom && !x.IgnoreList.Contains(client.Name) && !x.Quarantined);

                            if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                                ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafEmoteText(ServerCore.Linker, client.Name, text));

                            Events.EmoteSent(client, text);
                        }
                    }
                }
            }
        }

        private static void Command(ib0tClient client, String args)
        {
            Command cmd = new Command { Text = args, Args = String.Empty };
            Helpers.PopulateCommand(cmd);
            Events.Command(client, args, cmd.Target, cmd.Args);
        }
    }
}
