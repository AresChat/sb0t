using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Net;

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

            switch (ident)
            {
                case "LOGIN":
                    ProcessLogin(client, args, time);
                    break;

                case "PUBLIC":
                    ProcessText(client, args, time);
                    break;

                case "EMOTE":
                    ProcessEmote(client, args, time);
                    break;

                case "COMMAND":
                    ProcessCommand(client, args, time);
                    break;

                case "PING":
                    client.Time = time;
                    break;

                default:
                    throw new Exception();
            }
        }

        private static void ProcessLogin(ib0tClient client, String args, ulong time)
        {
            byte[] g = new byte[16];

            for (int i = 0; i < g.Length; i++)
                g[i] = byte.Parse(args.Substring((i * 2), 2), NumberStyles.HexNumber);

            client.CanScribble = g[0] == 0xff;
            client.Guid = new Guid(g);
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

            if (BanPool.IsBanned(client))
            {
                if (hijack != null && hijack is AresClient)
                    ((AresClient)hijack).SendDepart();

                Events.Rejected(client, RejectedMsg.Banned);
                throw new Exception("banned user");
            }

            if (!Events.Joining(client))
            {
                if (hijack != null && hijack is AresClient)
                    ((AresClient)hijack).SendDepart();

                Events.Rejected(client, RejectedMsg.UserDefined);
                throw new Exception("user defined rejection");
            }

            if (hijack == null || !(hijack is AresClient))
            {
                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Join(x, client)),
                    x => x.LoggedIn && x.Vroom == client.Vroom);

                UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.JoinTo(x, client.Name, client.Level)),
                    x => x.LoggedIn && x.Vroom == client.Vroom);
            }

            client.LoggedIn = true;
            client.QueuePacket(WebOutbound.AckTo(client, client.Name));
            client.QueuePacket(WebOutbound.TopicFirstTo(client, Settings.Get<String>("topic")));
            client.QueuePacket(WebOutbound.UserlistItemTo(client, Settings.Get<String>("bot"), Level.Host));

            UserPool.AUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.UserlistItemTo(client, x.Name, x.Level)),
                x => x.LoggedIn && x.Vroom == client.Vroom);

            UserPool.WUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.UserlistItemTo(client, x.Name, x.Level)),
                x => x.LoggedIn && x.Vroom == client.Vroom);

            client.QueuePacket(WebOutbound.UserlistEndTo(client));
            client.QueuePacket(WebOutbound.UrlTo(client, Settings.Get<String>("link", "url"), Settings.Get<String>("text", "url")));

            UserPool.AUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.FontTo(client, x.Name, x.Font.NameColor, x.Font.TextColor)),
                x => x.LoggedIn && x.Vroom == client.Vroom && x.Font.HasFont);

            Events.Joined(client);
        }

        private static void ProcessText(ib0tClient userobj, String args, ulong time)
        {

        }

        private static void ProcessEmote(ib0tClient userobj, String args, ulong time)
        {

        }

        private static void ProcessCommand(ib0tClient userobj, String args, ulong time)
        {

        }
    }
}
