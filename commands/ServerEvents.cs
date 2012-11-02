using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace commands
{
    public partial class ServerEvents : IExtension
    {
        public void ServerStarted()
        {
            Motd.LoadMOTD();
            Template.Load();
            Muzzles.Load();
            CustomNames.Load();
            KewlText.Clear();
            Lowered.Clear();
            Kiddied.Clear();
            Echo.Clear();
            Paint.Clear();
            RangeBans.Load();
            LoginAttempts.Clear();
            PMBlocking.Load();
            Captchas.Clear();
            AutoLogin.Load();
        }

        public void CycleTick() { }

        public void UnhandledProtocol(IUser client, bool custom, byte msg, byte[] packet) { }

        public bool Joining(IUser client)
        {
            if (RangeBans.IsRangeBanned(client))
            {
                client.Print(Template.Text(Category.Rejected, 6).Replace("+n", client.Name));
                return false;
            }

            if (Settings.AnonMonitoring)
                if (client.Name.StartsWith("anon "))
                {
                    client.Print(Template.Text(Category.Rejected, 2).Replace("+n", client.Name));
                    return false;
                }

            if (Settings.ShareFileMonitoring)
                if (client.FileCount == 0)
                {
                    client.Print(Template.Text(Category.Rejected, 3).Replace("+n", client.Name));
                    return false;
                }

            if (Settings.Filtering)
                if (JoinFilter.IsPreFiltered(client))
                {
                    client.Print(Template.Text(Category.Rejected, 7).Replace("+n", client.Name));
                    return false;
                }

            client.Print("\x000500\x000302" + Template.Text(Category.Credit, 0));

            byte[] buf = client.ExternalIP.GetAddressBytes();
            buf[3] = (byte)Math.Floor(new Random().NextDouble() * 255);
            client.ExternalIP = new System.Net.IPAddress(buf);

            return true;
        }

        public void Joined(IUser client)
        {
            KewlText.Remove(client);

            if (!client.Link.IsLinked)
            {
                if (!client.FastPing)
                    Motd.ViewMOTD(client);

                if (Muzzles.IsMuzzled(client))
                    client.Muzzled = true;

                CustomNames.Set(client);
                ILevel level = AutoLogin.GetLevel(client);

                if (level > ILevel.Regular)
                    client.SetLevel(level);
            }
        }

        public void Rejected(IUser client, RejectedMsg msg)
        {
            switch (msg)
            {
                case RejectedMsg.Banned:
                    client.Print(Template.Text(Category.Rejected, 5).Replace("+n", client.Name));
                    break;

                case RejectedMsg.NameInUse:
                    client.Print(Template.Text(Category.Rejected, 0).Replace("+n", client.Name));
                    break;

                case RejectedMsg.TooManyClients:
                    client.Print(Template.Text(Category.Rejected, 1).Replace("+n", client.Name));
                    break;

                case RejectedMsg.TooSoon:
                    client.Print(Template.Text(Category.Rejected, 4).Replace("+n", client.Name));
                    break;

                case RejectedMsg.UnacceptableGender:
                    client.Print(Template.Text(Category.Rejected, 9).Replace("+n", client.Name));
                    break;

                case RejectedMsg.UnderAge:
                    client.Print(Template.Text(Category.Rejected, 8).Replace("+n", client.Name).Replace("+a", Server.Chatroom.MinimumAge.ToString()));
                    break;
            }
        }

        public void Parting(IUser client) { }

        public void Parted(IUser client) { }

        public bool AvatarReceived(IUser client) { return true; }

        public bool PersonalMessageReceived(IUser client, String text) { return true; }

        public void TextReceived(IUser client, String text) { }

        public String TextSending(IUser client, String text)
        {
            if (client.Muzzled)
                client.Print(Template.Text(Category.Notification, 0));
            else
            {
                if (KewlText.IsKewlText(client))
                    text = KewlText.UnicodeText(text);

                if (Lowered.IsLowered(client))
                    text = text.ToLower();

                if (Settings.CapsMonitoring && client.Level == ILevel.Regular)
                    if (Lowered.HasExceeded(text))
                    {
                        text = text.ToLower();

                        if (!client.Link.IsLinked)
                            client.Print(Template.Text(Category.Notification, 4).Replace("+n", client.Name));
                    }

                String paint = Paint.IsPainted(client);

                if (paint != null)
                    text = paint + text;

                if (Kiddied.IsKiddied(client))
                    text = "\x000313" + Helpers.StripColors(text) + "(A)";
            }

            return text;
        }

        public void TextSent(IUser client, String text)
        {
            String echo = Echo.IsEcho(client);

            if (echo != null)
                client.SendText(echo);
        }

        public void EmoteReceived(IUser client, String text) { }

        public String EmoteSending(IUser client, String text)
        {
            if (client.Muzzled)
                client.Print(Template.Text(Category.Notification, 0));
            else
            {
                if (KewlText.IsKewlText(client))
                    text = KewlText.UnicodeText(text);

                if (Lowered.IsLowered(client))
                    text = text.ToLower();

                if (Settings.CapsMonitoring && client.Level == ILevel.Regular)
                    if (Lowered.HasExceeded(text))
                    {
                        text = text.ToLower();

                        if (!client.Link.IsLinked)
                            client.Print(Template.Text(Category.Notification, 4).Replace("+n", client.Name));
                    }
            }

            return text;
        }

        public void EmoteSent(IUser client, String text)
        {
            String echo = Echo.IsEcho(client);

            if (echo != null)
                client.SendEmote(echo);
        }

        public void PrivateSending(IUser client, IUser target, IPrivateMsg msg)
        {
            if (PMBlocking.IsBlocking(target))
                if (target.Level > client.Level)
                {
                    msg.Cancel = true;
                    client.PM(target.Name, Template.Text(Category.PmBlocking, 2).Replace("+n", client.Name).Replace("+t", target.Name));
                }
        }

        public void PrivateSent(IUser client, IUser target) { }

        public void BotPrivateSent(IUser client, String text) { }

        public bool Nick(IUser client, String name) { return true; }

        public void Help(IUser admin)
        {
            if (!admin.Registered)
                return;

            admin.Print("/id");

            if (admin.Level > ILevel.Regular || Settings.General)
            {
                admin.Print("/vroom <number>");
                admin.Print("/customname <user> <custom name>");
                admin.Print("/uncustomname <user> <custom name>");
                admin.Print("/pmblock <on or off>");
                admin.Print("/shout <message>");
                admin.Print("/whisper <user> <message>");
            }

            if (admin.Level >= Server.GetLevel("ban"))
                admin.Print("/ban <user> [<message>]");
            if (admin.Level >= Server.GetLevel("unban"))
                admin.Print("/unban <user>");
            if (admin.Level >= Server.GetLevel("kick"))
                admin.Print("/kick <user> [<message>]");
            if (admin.Level >= Server.GetLevel("muzzle"))
                admin.Print("/muzzle <user> [<message>]");
            if (admin.Level >= Server.GetLevel("muzzle"))
                admin.Print("/unmuzzle <user> [<message>]");
            if (admin.Level >= Server.GetLevel("kewltext"))
                admin.Print("/addkewltext <user>");
            if (admin.Level >= Server.GetLevel("kewltext"))
                admin.Print("/remkewltext <user>");
            if (admin.Level >= Server.GetLevel("lower"))
                admin.Print("/lower <user> [<message>]");
            if (admin.Level >= Server.GetLevel("unlower"))
                admin.Print("/unlower <user> [<message>]");
            if (admin.Level >= Server.GetLevel("kiddy"))
                admin.Print("/kiddy <user> [<message>]");
            if (admin.Level >= Server.GetLevel("kiddy"))
                admin.Print("/unkiddy <user> [<message>]");
            if (admin.Level >= Server.GetLevel("echo"))
                admin.Print("/echo <user> <messasge>");
            if (admin.Level >= Server.GetLevel("echo"))
                admin.Print("/unecho <user>");
            if (admin.Level >= Server.GetLevel("paint"))
                admin.Print("/paint <user> <message>");
            if (admin.Level >= Server.GetLevel("paint"))
                admin.Print("/unpaint <user>");
            if (admin.Level >= Server.GetLevel("rangeban"))
                admin.Print("/rangeban <ip range>");
            if (admin.Level >= Server.GetLevel("rangeunban"))
                admin.Print("/rangeunban <ip range>");
            if (admin.Level >= Server.GetLevel("listrangebans"))
                admin.Print("/listrangebans");
            if (admin.Level >= Server.GetLevel("cbans"))
                admin.Print("/cbans");
            if (admin.Level >= Server.GetLevel("adminmsg"))
                admin.Print("/adminmsg <message>");
            if (admin.Level >= Server.GetLevel("link"))
                admin.Print("/link <hashlink>");
            if (admin.Level >= Server.GetLevel("unlink"))
                admin.Print("/unlink");
            if (admin.Level >= Server.GetLevel("admins"))
                admin.Print("/admins");
            if (admin.Owner)
                admin.Print("/addautologin <user> <level>");
            if (admin.Owner)
                admin.Print("/remautologin <id>");
            if (admin.Owner)
                admin.Print("/autologins");
        }

        public void FileReceived(IUser client, String filename, String title, MimeType type) { }

        public bool Ignoring(IUser client, IUser target) { return true; }

        public void IgnoredStateChanged(IUser client, IUser target, bool ignored)
        {
            if (target != null)
                if (ignored)
                    client.Print(Template.Text(Category.Notification, 2).Replace("+n", target.Name));
                else
                    client.Print(Template.Text(Category.Notification, 3).Replace("+n", target.Name));
        }

        public void InvalidLoginAttempt(IUser client)
        {
            LoginAttempts.Add(client);

            if (LoginAttempts.Count(client) == 3)
            {
                LoginAttempts.Remove(client);

                Server.Print(Template.Text(Category.AdminLogin, 2).Replace("+n",
                    client.Name).Replace("+ip", client.ExternalIP.ToString()));

                client.Ban();
            }
            else Server.Print(Template.Text(Category.AdminLogin, 1).Replace("+n",
                client.Name).Replace("+ip", client.ExternalIP.ToString()));
        }

        public void LoginGranted(IUser client)
        {
            LoginAttempts.Remove(client);
            client.Print(Template.Text(Category.Registration, 1));
        }

        public void AdminLevelChanged(IUser client)
        {
            if (client.Level == ILevel.Regular)
                Server.Print(Template.Text(Category.AdminLogin, 3).Replace("+n", client.Name));
            else
                Server.Print(Template.Text(Category.AdminLogin, 0).Replace("+n",
                    client.Name).Replace("+l", ((int)client.Level).ToString()));
        }

        public bool Registering(IUser client) { return true; }

        public void Registered(IUser client)
        {
            client.Print(Template.Text(Category.Registration, 0));
        }

        public void Unregistered(IUser client)
        {
            client.Print(Template.Text(Category.Registration, 2));
        }

        public void CaptchaSending(IUser client)
        {
            client.Print(Template.Text(Category.Captcha, 2));
        }

        public void CaptchaReply(IUser client, String reply)
        {
            if (client.Captcha)
            {
                Captchas.Remove(client);
                client.Print(Template.Text(Category.Captcha, 3));
            }
            else
            {
                Captchas.Add(client);

                if (Captchas.Count(client) > 3)
                {
                    Captchas.Remove(client);
                    client.Print(Template.Text(Category.Captcha, 0));
                    client.Disconnect();
                }
                else client.Print(Template.Text(Category.Captcha, 1).Replace("+a", reply));
            }
        }

        public bool VroomChanging(IUser client, ushort vroom) { return true; }

        public void VroomChanged(IUser client) { }

        public bool Flooding(IUser client, byte msg) { return true; }

        public void Flooded(IUser client) { }

        public bool ProxyDetected(IUser client) { return true; }

        public void Logout(IUser client) { }

        public void Idled(IUser client)
        {
            if (Settings.IdleMonitoring)
            {
                String time = Helpers.Time();
                Server.Print(Template.Text(Category.Idle, 0).Replace("+n", client.Name).Replace("+t", time));
            }
        }

        public void Unidled(IUser client, uint seconds_away)
        {
            if (Settings.IdleMonitoring)
            {
                IdleTime to = Helpers.GetIdleUptime(seconds_away);
                String time = Helpers.Time();

                if (to.Days > 0)
                {
                    Server.Print(Template.Text(Category.Idle, 4).
                        Replace("+n", client.Name).
                        Replace("+t", time).
                        Replace("+d", to.Days.ToString()).
                        Replace("+h", to.Hours.ToString()).
                        Replace("+m", to.Minutes.ToString()).
                        Replace("+s", to.Seconds.ToString()));
                }
                else if (to.Hours > 0)
                {
                    Server.Print(Template.Text(Category.Idle, 3).
                        Replace("+n", client.Name).
                        Replace("+t", time).
                        Replace("+h", to.Hours.ToString()).
                        Replace("+m", to.Minutes.ToString()).
                        Replace("+s", to.Seconds.ToString()));
                }
                else if (to.Minutes > 0)
                {
                    Server.Print(Template.Text(Category.Idle, 2).
                        Replace("+n", client.Name).
                        Replace("+t", time).
                        Replace("+m", to.Minutes.ToString()).
                        Replace("+s", to.Seconds.ToString()));
                }
                else
                {
                    Server.Print(Template.Text(Category.Idle, 1).
                        Replace("+n", client.Name).
                        Replace("+t", time).
                        Replace("+s", to.Seconds.ToString()));
                }
            }
        }

        public void BansAutoCleared()
        {
            Server.Print(Template.Text(Category.Notification, 5), true);
        }

        public void Command(IUser client, String cmd, IUser target, String args)
        {
            if (!client.Registered)
                return;

            if (cmd == "version")
                client.Print("sb0t 5.00");
            else if (cmd.StartsWith("vroom "))
                Eval.Vroom(client, cmd.Substring(6));
            else if (cmd == "id")
                Eval.ID(client);
            else if (cmd.StartsWith("ban "))
                Eval.Ban(client, target, args);
            else if (cmd.StartsWith("unban "))
                Eval.Unban(client, cmd.Substring(6));
            else if (cmd.StartsWith("kick ") || cmd.StartsWith("kill "))
                Eval.Kick(client, target, args);
            else if (cmd.StartsWith("muzzle "))
                Eval.Muzzle(client, target, args);
            else if (cmd.StartsWith("unmuzzle "))
                Eval.Unmuzzle(client, target, args);
            else if (cmd.StartsWith("customname "))
                Eval.CustomName(client, target, args);
            else if (cmd.StartsWith("uncustomname "))
                Eval.UncustomName(client, target, args);
            else if (cmd.StartsWith("addkewltext "))
                Eval.AddKewlText(client, target);
            else if (cmd.StartsWith("remkewltext "))
                Eval.RemKewlText(client, target);
            else if (cmd.StartsWith("lower "))
                Eval.Lower(client, target, args);
            else if (cmd.StartsWith("unlower "))
                Eval.Unlower(client, target, args);
            else if (cmd.StartsWith("kiddy "))
                Eval.Kiddy(client, target, args);
            else if (cmd.StartsWith("unkiddy "))
                Eval.Unkiddy(client, target, args);
            else if (cmd.StartsWith("echo "))
                Eval.Echo(client, target, args);
            else if (cmd.StartsWith("unecho "))
                Eval.Unecho(client, target);
            else if (cmd.StartsWith("paint "))
                Eval.Paint(client, target, args);
            else if (cmd.StartsWith("unpaint "))
                Eval.Unpaint(client, target);
            else if (cmd.StartsWith("rangeban "))
                Eval.RangeBan(client, cmd.Substring(9));
            else if (cmd.StartsWith("rangeunban "))
                Eval.RangeUnban(client, cmd.Substring(11));
            else if (cmd == "listrangebans")
                Eval.ListRangeBans(client);
            else if (cmd == "cbans" || cmd == "clearbans")
                Eval.Cbans(client);
            else if (cmd.StartsWith("pmblock "))
                Eval.PMBlock(client, cmd.Substring(8).Trim());
            else if (cmd.StartsWith("shout "))
                Eval.Shout(client, cmd.Substring(6));
            else if (cmd.StartsWith("adminmsg "))
                Eval.AdminMsg(client, cmd.Substring(9));
            else if (cmd.StartsWith("whisper "))
                Eval.Whisper(client, target, args);
            else if (cmd.StartsWith("link "))
                Eval.Link(client, cmd.Substring(5));
            else if (cmd == "unlink")
                Eval.Unlink(client);
            else if (cmd == "admins")
                Eval.Admins(client);
            else if (cmd.StartsWith("addautologin "))
                Eval.AddAutologin(client, target, args);
            else if (cmd.StartsWith("remautologin "))
                Eval.RemAutologin(client, cmd.Substring(13));
            else if (cmd == "autologins")
                Eval.Autologins(client);
        }

        public void LinkError(ILinkError error)
        {
            Server.Print(Template.Text(Category.Linking, 0).Replace("+e", error.ToString()));

            if (Server.Link.IsLinked)
                if (error == ILinkError.RemoteDisconnect || error == ILinkError.UnableToConnect)
                    Server.Print(Template.Text(Category.Linking, 8));
        }

        public void Linked()
        {
            Server.Print(Template.Text(Category.Linking, 1).Replace("+n", Server.Link.Name));
        }

        public void Unlinked()
        {
            Server.Print(Template.Text(Category.Linking, 2));

            if (Server.Link.IsLinked)
                Server.Print(Template.Text(Category.Linking, 8));
        }

        public void LeafJoined(ILeaf leaf)
        {
            Server.Print(Template.Text(Category.Linking, 3).Replace("+n", leaf.Name));
        }

        public void LeafParted(ILeaf leaf)
        {
            Server.Print(Template.Text(Category.Linking, 4).Replace("+n", leaf.Name));
        }

        public void LinkedAdminDisabled(ILeaf leaf, IUser client)
        {
            if (client != null)
                client.Print(Template.Text(Category.Linking, 7).Replace("+n", leaf.Name));
        }
    }
}
