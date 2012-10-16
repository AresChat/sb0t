using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using core.ib0t;
using iconnect;

namespace core
{
    class Helpers
    {
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

            client.OrgName = Regex.Replace(client.OrgName, Regex.Escape("_"), " ", RegexOptions.IgnoreCase);
            client.OrgName = Regex.Replace(client.OrgName, Regex.Escape("\""), String.Empty, RegexOptions.IgnoreCase);
            client.OrgName = Regex.Replace(client.OrgName, Regex.Escape("/"), String.Empty, RegexOptions.IgnoreCase);
            client.OrgName = Regex.Replace(client.OrgName, Regex.Escape("\\"), String.Empty, RegexOptions.IgnoreCase);
            client.OrgName = Regex.Replace(client.OrgName, Regex.Escape("www."), String.Empty, RegexOptions.IgnoreCase);

            while (Encoding.UTF8.GetByteCount(client.OrgName) > 20)
                client.OrgName = client.OrgName.Substring(0, client.OrgName.Length - 1);

            if (client.OrgName.Length < 2)
            {
                client.OrgName = "anon ";

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
                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Avatar(x, client)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

            if (client.PersonalMessage.Length > 0)
                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.PersonalMessage(x, client)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

            if (client.Font.HasFont)
            {
                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.CustomFont(x, client)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && x.CustomClient && !x.Quarantined);

                UserPool.WUsers.ForEachWhere(x => x.QueuePacket(WebOutbound.FontTo(x, client.Name, client.Font.NameColor, client.Font.TextColor)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && x.CustomClient && !x.Quarantined);
            }

            if (client.VoiceChatPrivate || client.VoiceChatPublic)
                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.VoiceChatUserSupport(x, client)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && (x.VoiceChatPrivate || x.VoiceChatPublic) && !x.Quarantined);

            foreach (CustomEmoticon em in client.EmoticonList)
                UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.CustomEmoteItem(x, client, em)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && x.CustomEmoticons && !x.Quarantined);
        }

        public static void UncloakedSequence(ib0t.ib0tClient client)
        {
            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Join(x, client)),
                x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

            UserPool.WUsers.ForEachWhere(x => x.QueuePacket(ib0t.WebOutbound.JoinTo(x, client.Name, client.Level)),
                x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.Avatar(x, client)),
                x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);

            UserPool.AUsers.ForEachWhere(x => x.SendPacket(TCPOutbound.PersonalMessage(x, client)),
                x => x.LoggedIn && x.Vroom == client.Vroom && !x.Quarantined);
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
            client.CustomEmoticons = false;
            client.EmoticonList.Clear();
            client.SendPacket(TCPOutbound.Ack(client));

            if (features)
                client.SendPacket(TCPOutbound.MyFeatures(client));

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
            client.SendPacket(TCPOutbound.SupportsCustomEmotes());

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

            if (ServerCore.Linker.Busy)
                foreach (LinkLeaf.Leaf leaf in ServerCore.Linker.Leaves)
                    leaf.Users.ForEachWhere(x => client.SendPacket(TCPOutbound.PersonalMessage(client, x)),
                        x => x.Vroom == client.Vroom && x.Link.Visible && x.PersonalMessage.Length > 0);

            if (client.CustomClient)
                UserPool.AUsers.ForEachWhere(x => client.SendPacket(TCPOutbound.CustomFont(client, x)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && x.Font.HasFont && !x.Cloaked && !x.Quarantined);

            // are we sending avatars to others?

            if (features)
            {
                if (client.SocketConnected)
                    IdleManager.Set(client);

                Events.Joined(client);
            }
        }

        public static void FakeRejoinSequence(ib0t.ib0tClient client)
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

            client.QueuePacket(WebOutbound.UserlistEndTo(client));

            if (client.CustomClient)
                UserPool.AUsers.ForEachWhere(x => client.QueuePacket(WebOutbound.FontTo(client, x.Name, x.Font.NameColor, x.Font.TextColor)),
                    x => x.LoggedIn && x.Vroom == client.Vroom && x.Font.HasFont && !x.Cloaked && !x.Quarantined);
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
