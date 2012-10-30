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
            }

            return text;
        }

        public void EmoteSent(IUser client, String text)
        {
            String echo = Echo.IsEcho(client);

            if (echo != null)
                client.SendEmote(echo);
        }

        public void PrivateSending(IUser client, IUser target, IPrivateMsg msg) { }

        public void PrivateSent(IUser client, IUser target) { }

        public void BotPrivateSent(IUser client, String text) { }

        public bool Nick(IUser client, String name) { return true; }

        public void Help(IUser client) { }

        public void FileReceived(IUser client, String filename, String title, MimeType type) { }

        public bool Ignoring(IUser client, IUser target) { return true; }

        public void IgnoredStateChanged(IUser client, IUser target, bool ignored) { }

        public void InvalidLoginAttempt(IUser client) { }

        public void LoginGranted(IUser client) { }

        public void AdminLevelChanged(IUser client) { }

        public bool Registering(IUser client) { return true; }

        public void Registered(IUser client) { }

        public void Unregistered(IUser client) { }

        public void CaptchaSending(IUser client) { }

        public void CaptchaReply(IUser client, String reply) { }

        public bool VroomChanging(IUser client, ushort vroom) { return true; }

        public void VroomChanged(IUser client) { }

        public bool Flooding(IUser client, byte msg) { return true; }

        public void Flooded(IUser client) { }

        public bool ProxyDetected(IUser client) { return true; }

        public void Logout(IUser client) { }

        public void Idled(IUser client) { }

        public void Unidled(IUser client, uint seconds_away) { }

        public void BansAutoCleared() { }

        public void Command(IUser client, String cmd, IUser target, String args)
        {
            if (!client.Registered)
                return;

            if (cmd == "version")
                client.Print("sb0t 5.00");
            if (cmd.StartsWith("vroom "))
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
        }

        public void LinkError(ILinkError error) { }

        public void Linked() { }

        public void Unlinked() { }

        public void LeafJoined(ILeaf leaf) { }

        public void LeafParted(ILeaf leaf) { }

        public void LinkedAdminDisabled(ILeaf leaf, IUser client) { }
    }
}
