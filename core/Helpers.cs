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
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Security.Cryptography;
using System.Drawing;
using System.Globalization;
using core.ib0t;
using iconnect;

namespace core
{
    class Helpers
    {
        public static String AresColorToHTMLColor(byte c)
        {
            switch (c)
            {
                case 1: return "#000000";
                case 0: return "#FFFFFF";
                case 8: return "#FFFF00";
                case 11: return "#00FFFF";
                case 12: return "#0000FF";
                case 2: return "#000080";
                case 6: return "#800080";
                case 9: return "#00FF00";
                case 13: return "#FF00FF";
                case 14: return "#808080";
                case 15: return "#C0C0C0";
                case 7: return "#FFA500";
                case 5: return "#800000";
                case 10: return "#008080";
                case 3: return "#008000";
                case 4: return "#FF0000";
                default: return null;
            }
        }

        private static Color[] acols = new Color[]
        {
            Color.White, Color.Black, Color.Navy, Color.Green, Color.Red, Color.Maroon, Color.Purple, Color.Orange,
            Color.Yellow, Color.Lime, Color.Teal, Color.Aqua, Color.Blue, Color.Fuchsia, Color.Gray, Color.Silver
        };

        public static byte HTMLColorToAresColor(String h)
        {
            byte r = byte.Parse(h.Substring(1, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(h.Substring(3, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(h.Substring(5, 2), NumberStyles.HexNumber);

            double closest = double.MaxValue;
            int result = 0;

            for (int i = 0; i < acols.Length; i++)
            {
                double d = Math.Sqrt(Math.Pow((r - acols[i].R), 2) +
                                     Math.Pow((g - acols[i].G), 2) +
                                     Math.Pow((b - acols[i].B), 2));

                if (d < closest)
                {
                    closest = d;
                    result = i;
                }
            }

            return (byte)result;
        }

        public static String StripColors(String input)
        {
            if (Regex.IsMatch(input, @"\x03|\x05", RegexOptions.IgnoreCase))
                input = Regex.Replace(input, @"(\x03|\x05)[0-9]{2}", "");

            input = input.Replace("\x06", "");
            input = input.Replace("\x07", "");
            input = input.Replace("\x09", "");
            input = input.Replace("\x02", "");
            input = input.Replace("­", "");

            return input;
        }

        public static String ObfuscateDns(String dns)
        {
            char[] chrs = dns.ToCharArray();
            String[] results = new String[chrs.Length];

            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] buf = sha1.ComputeHash(Encoding.UTF8.GetBytes(dns));
                int counter = 0;

                for (int i = (chrs.Length - 1); i > -1; i--)
                {
                    int c = chrs[i];

                    if (c >= 49 && c <= 57)
                        results[i] = buf[++counter].ToString();
                    else
                        results[i] = chrs[i].ToString();

                    if (counter == 20)
                        counter = 0;
                }
            }

            return String.Join(String.Empty, results);
        }

        public static bool IsLocalHost(IClient client)
        {
            if (!Settings.Get<bool>("local_host"))
                return false;

            byte[] buf = client.ExternalIP.GetAddressBytes();

            switch (buf[0])
            {
                case 192:
                    return buf[1] == 168;

                case 127:
                    return true;

                case 10:
                    return buf[1] == 0 || buf[1] == 1;
            }

            buf = Settings.Get<byte[]>("ip");

            if (buf != null)
                return client.ExternalIP.Equals(new IPAddress(buf));

            return false;
        }

        public static bool IsUnacceptableGender(IClient client)
        {
            switch (client.Sex)
            {
                case 2:
                    return Settings.Get<bool>("reject_female");

                case 1:
                    return Settings.Get<bool>("reject_male");

                default:
                    return Settings.Get<bool>("reject_unknown");
            }
        }

        public static void FormatUsername(IClient client)
        {
            if (client.OrgName == Settings.Get<String>("bot"))
                client.OrgName = String.Empty;

            client.OrgName = client.OrgName.Trim();

            client.OrgName = Regex.Replace(client.OrgName, Regex.Escape("_"), " ", RegexOptions.IgnoreCase);
            client.OrgName = Regex.Replace(client.OrgName, Regex.Escape("\""), String.Empty, RegexOptions.IgnoreCase);
            client.OrgName = Regex.Replace(client.OrgName, Regex.Escape("/"), String.Empty, RegexOptions.IgnoreCase);
            client.OrgName = Regex.Replace(client.OrgName, Regex.Escape("\\"), String.Empty, RegexOptions.IgnoreCase);
            client.OrgName = Regex.Replace(client.OrgName, Regex.Escape("www."), String.Empty, RegexOptions.IgnoreCase);

            while (Encoding.UTF8.GetByteCount(client.OrgName) > 20)
                client.OrgName = client.OrgName.Substring(0, client.OrgName.Length - 1);

            if (client.OrgName.Length < 2 || string.IsNullOrWhiteSpace(client.OrgName))
            {
                client.OrgName = client.WebClient ? "ib0t" : "anon ";

                foreach (byte b in client.ExternalIP.GetAddressBytes())
                    client.OrgName += String.Format("{0:x2}", b);
            }
        }

        public static bool NameAvailable(IClient client, String name)
        {
            if (name == Settings.Get<String>("bot"))
                return false;

            if (Encoding.UTF8.GetByteCount(name) > 20 || Encoding.UTF8.GetByteCount(name) < 2)
                return false;

            foreach (AresClient a in UserPool.AUsers)
                if (a.LoggedIn)
                    if (a.ID != client.ID)
                        if (name == a.Name || name == a.OrgName)
                            return false;

            foreach (ib0t.ib0tClient i in UserPool.WUsers)
                if (i.LoggedIn)
                    if (i.ID != client.ID)
                        if (name == i.Name || name == i.OrgName)
                            return false;

            return true;
        }

        public static void PopulateCommand(Command cmd)
        {
            String str = cmd.Text;
            int space = str.IndexOf(" ");
            ushort id;

            if (space == -1)
                return;

            str = str.Substring(str.IndexOf(" ") + 1);
            cmd.Target = UserPool.AUsers.Find(x => x.Name == str);
            cmd.Args = String.Empty;

            if (cmd.Target == null)
                cmd.Target = UserPool.WUsers.Find(x => x.Name == str);

            if (cmd.Target == null && ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                cmd.Target = ServerCore.Linker.FindUser(x => x.Name == str);

            if (cmd.Target == null && str.Length > 0)
                if (str.StartsWith("\"") && str.LastIndexOf("\"") > str.IndexOf("\""))
                {
                    str = str.Substring(1);
                    cmd.Target = UserPool.AUsers.Find(x => x.Name == str.Substring(0, str.IndexOf("\"")));

                    if (cmd.Target == null)
                        cmd.Target = UserPool.WUsers.Find(x => x.Name == str.Substring(0, str.IndexOf("\"")));

                    if (cmd.Target == null && ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                        cmd.Target = ServerCore.Linker.FindUser(x => x.Name == str.Substring(0, str.IndexOf("\"")));

                    if (cmd.Target == null)
                        cmd.Target = UserPool.AUsers.Find(x => x.Name.StartsWith(str.Substring(0, str.IndexOf("\""))));

                    if (cmd.Target == null)
                        cmd.Target = UserPool.WUsers.Find(x => x.Name.StartsWith(str.Substring(0, str.IndexOf("\""))));

                    if (cmd.Target == null && ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                        cmd.Target = ServerCore.Linker.FindUser(x => x.Name.StartsWith(str.Substring(0, str.IndexOf("\""))));

                    str = str.Substring(str.IndexOf("\"") + 1);

                    if (str.StartsWith(" "))
                        str = str.Substring(1);

                    if (cmd.Target != null)
                        cmd.Args = str;
                }
                else if (str.IndexOf(" ") > 0)
                {
                    String id_str = str.Substring(0, str.IndexOf(" "));
                    cmd.Args = str.Substring(str.IndexOf(" ") + 1);

                    if (ushort.TryParse(id_str, out id))
                    {
                        cmd.Target = UserPool.AUsers.Find(x => x.ID == id);

                        if (cmd.Target == null)
                            cmd.Target = UserPool.WUsers.Find(x => x.ID == id);

                        if (cmd.Target == null && ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                            cmd.Target = ServerCore.Linker.FindUser(x => x.ID == id);
                    }

                    if (cmd.Target == null)
                        cmd.Args = String.Empty;
                }
                else if (ushort.TryParse(str, out id))
                {
                    cmd.Target = UserPool.AUsers.Find(x => x.ID == id);

                    if (cmd.Target == null)
                        cmd.Target = UserPool.WUsers.Find(x => x.ID == id);

                    if (cmd.Target == null && ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                        cmd.Target = ServerCore.Linker.FindUser(x => x.ID == id);
                }
        }

        public static void UncloakedSequence(AresClient client)
        {
            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Join(x, client)),
                x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

            UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.JoinTo(x, client.Name, client.Level)),
                x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

            if (client.Avatar.Length > 0)
            {
                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Avatar(x, client)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                UserPool.WUsers.ForEachWhere(x => x.QueuePacket(WebOutbound.AvatarTo(x, client.Name, client.Avatar)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined && x.Extended);
            }

            if (client.PersonalMessage.Length > 0)
            {
                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.PersonalMessage(x, client)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                UserPool.WUsers.ForEachWhere(x => x.QueuePacket(WebOutbound.PersMsgTo(x, client.Name, client.PersonalMessage)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined && x.Extended);
            }

            if (client.Font.Enabled)
            {
                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.CustomFont(x, client)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined && x.IsCbot);

                AresFont f = (AresFont)client.Font;
                UserPool.WUsers.ForEachWhere(x => x.QueuePacket(WebOutbound.FontTo(x, client.Name, f.oldN, f.oldT)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);
            }

            if (client.VoiceChatPrivate || client.VoiceChatPublic)
                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.VoiceChatUserSupport(x, client)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && (x.VoiceChatPrivate || x.VoiceChatPublic) && !x.Quarantined);
        }

        public static void FakeRejoinSequence(AresClient client, bool features)
        {
            if (!client.Cloaked)
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

            client.SharedFiles.Clear();
            client.LoggedIn = true;
            client.SendPacket(TCPOutbound.Ack(client));

            if (features)
            {
                client.SendPacket(TCPOutbound.MyFeatures(client));
                client.SendPacket(TCPOutbound.FavIcon());
            }

            client.SendPacket(TCPOutbound.UserlistBot(client));

            UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.Userlist(client, x)),
                x => x.LoggedIn && x.Vroom == client.Vroom && !x.Cloaked && !x.Quarantined);

            UserPool.WUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.Userlist(client, x)),
                x => x.LoggedIn && x.Vroom == client.Vroom && !x.Cloaked && !x.Quarantined);

            if (ServerCore.Linker.Busy)
                foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                    leaf.Users.ForEachWhere(x => client.SendPacket(TCPOutbound.Userlist(client, x)),
                        x => x.Vroom == client.Vroom && x.Link.Visible);

            client.SendPacket(TCPOutbound.UserListEnd());
            client.SendPacket(TCPOutbound.OpChange(client));

            if (features)
            {
                client.SendPacket(TCPOutbound.SupportsVoiceClips());
                client.SendPacket(TCPOutbound.Url(client, Settings.Get<String>("link", "url"), Settings.Get<String>("text", "url")));
            }

            client.SendPacket(Avatars.Server(client));
            client.SendPacket(TCPOutbound.PersonalMessageBot(client));

            if (client.CustomClient)
                UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.VoiceChatUserSupport(client, x)),
                    x => (x.VoiceChatPrivate || x.VoiceChatPublic) && !x.Cloaked && !x.Quarantined);

            UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.Avatar(client, x)),
                x => x.LoggedIn && x.Vroom == client.Vroom && x.Avatar.Length > 0 && !x.Cloaked && !x.Quarantined);

            UserPool.WUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.Avatar(client, x)),
                x => x.LoggedIn && x.Vroom == client.Vroom && !x.Cloaked && !x.Quarantined);

            if (ServerCore.Linker.Busy)
                foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                    leaf.Users.ForEachWhere(x => client.SendPacket(TCPOutbound.Avatar(client, x)),
                        x => x.Vroom == client.Vroom && x.Link.Visible && x.Avatar.Length > 0);

            UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.PersonalMessage(client, x)),
                x => x.LoggedIn && x.Vroom == client.Vroom && x.PersonalMessage.Length > 0 && !x.Cloaked && !x.Quarantined);

            UserPool.WUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.PersonalMessage(client, x)),
                x => x.LoggedIn && x.Vroom == client.Vroom && !x.Cloaked && !x.Quarantined);

            if (client.IsCbot)
                UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.CustomFont(client, x)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && x.Font.Enabled);

            if (ServerCore.Linker.Busy)
                foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                    leaf.Users.ForEachWhere(x => client.SendPacket(TCPOutbound.PersonalMessage(client, x)),
                        x => x.Vroom == client.Vroom && x.Link.Visible && x.PersonalMessage.Length > 0);

            if (client.Avatar.Length > 0)
                if (!client.Cloaked)
                {
                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Avatar(x, client)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                    UserPool.WUsers.ForEachWhere(x => x.QueuePacket(WebOutbound.AvatarTo(x, client.Name, client.Avatar)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined && x.Extended);

                    if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                        ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafAvatar(ServerCore.Linker, client));
                }

            if (!String.IsNullOrEmpty(client.PersonalMessage))
                if (!client.Cloaked)
                {
                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.PersonalMessage(x, client)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                    UserPool.WUsers.ForEachWhere(x => x.QueuePacket(WebOutbound.PersMsgTo(x, client.Name, client.PersonalMessage)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined && x.Extended);

                    if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                        ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafPersonalMessage(ServerCore.Linker, client));
                }

            if (client.Font.Enabled)
                if (!client.Cloaked)
                {
                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.CustomFont(x, client)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined && x.IsCbot);

                    AresFont f = (AresFont)client.Font;
                    UserPool.WUsers.ForEachWhere(x => x.QueuePacket(WebOutbound.FontTo(x, client.Name, f.oldN, f.oldT)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);
                }

            if (features)
            {
                if (client.SocketConnected)
                    IdleManager.Set(client);

                Events.Joined(client);
            }
        }

        public static void FakeRejoinSequence(ib0t.ib0tClient client, bool features)
        {
            if (!client.Cloaked)
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
            client.QueuePacket(WebOutbound.UserlistItemTo(client, Settings.Get<String>("bot"), ILevel.Host));

            UserPool.AUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.UserlistItemTo(client, x.Name, x.Level)),
                x => x.LoggedIn && x.Vroom == client.Vroom && !x.Cloaked && !x.Quarantined);

            UserPool.WUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.UserlistItemTo(client, x.Name, x.Level)),
                x => x.LoggedIn && x.Vroom == client.Vroom && !x.Cloaked && !x.Quarantined);

            UserPool.AUsers.ForEachWhere(x =>
            {
                AresFont f = (AresFont)x.Font;
                client.QueuePacket(ib0t.WebOutbound.FontTo(client, x.Name, f.oldN, f.oldT));
            }, x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined && x.Font.Enabled);

            if (ServerCore.Linker.Busy)
                foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                    leaf.Users.ForEachWhere(x => client.QueuePacket(WebOutbound.UserlistItemTo(client, x.Name, x.Level)),
                        x => x.Vroom == client.Vroom && x.Link.Visible);

            client.QueuePacket(WebOutbound.UserlistEndTo(client));

            if (features)
                client.QueuePacket(WebOutbound.UrlTo(client, Settings.Get<String>("link", "url"), Settings.Get<String>("text", "url")));

            if (client.Extended)
            {
                client.QueuePacket(WebOutbound.PerMsgBotTo(client));
                client.QueuePacket(Avatars.Server(client));

                UserPool.AUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.AvatarTo(client, x.Name, x.Avatar)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && x.Avatar.Length > 0 && !x.Cloaked && !x.Quarantined);

                UserPool.WUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.AvatarTo(client, x.Name, x.Avatar)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Cloaked && !x.Quarantined);

                if (ServerCore.Linker.Busy)
                    foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                        leaf.Users.ForEachWhere(x => client.QueuePacket(WebOutbound.AvatarTo(client, x.Name, x.Avatar)),
                            x => x.Vroom == client.Vroom && x.Link.Visible && x.Avatar.Length > 0);

                UserPool.AUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.PersMsgTo(client, x.Name, x.PersonalMessage)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && x.PersonalMessage.Length > 0 && !x.Cloaked && !x.Quarantined);

                UserPool.WUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.PersMsgTo(client, x.Name, x.PersonalMessage)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Cloaked && !x.Quarantined);

                if (ServerCore.Linker.Busy)
                    foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                        leaf.Users.ForEachWhere(x => client.QueuePacket(WebOutbound.PersMsgTo(client, x.Name, x.PersonalMessage)),
                            x => x.Vroom == client.Vroom && x.Link.Visible && x.PersonalMessage.Length > 0);
            }

            if (client.Avatar.Length > 0)
                if (!client.Cloaked)
                {
                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Avatar(x, client)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                    UserPool.WUsers.ForEachWhere(x => x.QueuePacket(WebOutbound.AvatarTo(x, client.Name, client.Avatar)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined && x.Extended);

                    if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                        ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafAvatar(ServerCore.Linker, client));
                }

            if (!String.IsNullOrEmpty(client.PersonalMessage))
                if (!client.Cloaked)
                {
                    UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.PersonalMessage(x, client)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

                    UserPool.WUsers.ForEachWhere(x => x.QueuePacket(WebOutbound.PersMsgTo(x, client.Name, client.PersonalMessage)),
                        x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined && x.Extended);

                    if (ServerCore.Linker.Busy && ServerCore.Linker.LoginPhase == LinkLeaf.LinkLogin.Ready)
                        ServerCore.Linker.SendPacket(LinkLeaf.LeafOutbound.LeafPersonalMessage(ServerCore.Linker, client));
                }

            if (features)
            {
                if (client.SocketConnected)
                    IdleManager.Set(client);

                Events.Joined(client);
            }
        }

        public static uint UnixTime
        {
            get
            {
                TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                return (uint)ts.TotalSeconds;
            }
        }
    }

    class Command
    {
        public String Text { get; set; }
        public IClient Target { get; set; }
        public String Args { get; set; }
    }
}
