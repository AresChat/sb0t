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
using System.Linq;
using System.Text;
using iconnect;

namespace commands
{
    public partial class ServerEvents : IExtension
    {
        public void ServerStarted()
        {
            this._second_timer = 0;

            Motd.LoadMOTD();
            Template.Load(false);
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
            Topics.LoadTopics();
            Bans.Load();
            Greets.Load();
            Urls.Load();
            History.Reset();
            AvatarPMManager.Reset();
            BanStats.Reset();
            VSpy.Reset();
            Whowas.Setup();
            IPSend.Reset();
            BanSend.Reset();
            LogSend.Reset();
            JoinFilter.Load();
            FileFilter.Load();
            WordFilter.Load();
            AntiFlood.Reset();
        }

        private uint _second_timer = 0;

        public void CycleTick()
        {
            uint time = Server.Time;

            if (time > this._second_timer)
            {
                this._second_timer = time;
                Muzzles.Tick(time);
                Topics.UpdateClock(time);
                Bans.Tick(time);
                Urls.Tick(time);
                RoomInfo.Tick(time);
            }

            UrbanDictionaryResult urban;

            while (UrbanDictionary.RESULTS.Count > 0)
                if (!UrbanDictionary.RESULTS.TryDequeue(out urban))
                    break;
                else UrbanDictionary.Show(urban);

            DictionaryResultCollection define;

            while (DefineDictionary.RESULTS.Count > 0)
                if (!DefineDictionary.RESULTS.TryDequeue(out define))
                    break;
                else DefineDictionary.Show(define);

            TraceResult trace;

            while (Trace.RESULTS.Count > 0)
                if (!Trace.RESULTS.TryDequeue(out trace))
                    break;
                else Trace.Show(trace);
        }

        public void UnhandledProtocol(IUser client, bool custom, byte msg, byte[] packet) { }

        public bool Joining(IUser client)
        {
            if (RangeBans.IsRangeBanned(client))
            {
                client.Print(Template.Text(Category.Rejected, 6).Replace("+n", client.Name));
                BanSend.Rejected(client, Template.Text(Category.BanSend, 6).Replace("+n", client.Name).Replace("+ip", client.ExternalIP.ToString()));
                return false;
            }

            if (Settings.AnonMonitoring)
                if (client.Name.StartsWith("anon "))
                {
                    client.Print(Template.Text(Category.Rejected, 2).Replace("+n", client.Name));
                    BanSend.Rejected(client, Template.Text(Category.BanSend, 7).Replace("+n", client.Name).Replace("+ip", client.ExternalIP.ToString()));
                    return false;
                }

            if (Settings.ShareFileMonitoring)
                if (client.FileCount == 0)
                {
                    client.Print(Template.Text(Category.Rejected, 3).Replace("+n", client.Name));
                    BanSend.Rejected(client, Template.Text(Category.BanSend, 8).Replace("+n", client.Name).Replace("+ip", client.ExternalIP.ToString()));
                    return false;
                }

            if (Settings.Filtering)
                if (JoinFilter.IsPreFiltered(client))
                {
                    client.Print(Template.Text(Category.Rejected, 7).Replace("+n", client.Name));
                    BanSend.Rejected(client, Template.Text(Category.BanSend, 9).Replace("+n", client.Name).Replace("+ip", client.ExternalIP.ToString()));
                    return false;
                }

            client.Print("\x000500\x000302" + Template.Text(Category.Credit, 0));

            return true;
        }

        public void Joined(IUser client)
        {
            KewlText.Remove(client);

            if (!client.Link.IsLinked)
            {
                if (client.Vroom == 0)
                {
                    if (Settings.Clock)
                        Topics.ClockTo(client);
                }
                else
                {
                    String topic = Topics.GetTopic(client.Vroom);

                    if (!String.IsNullOrEmpty(topic))
                        client.Topic(topic);
                }

                if (!client.FastPing)
                {
                    if (!client.WebClient)
                    {
                        Motd.ViewMOTD(client);

                        if (Settings.PMGreetMsg)
                            client.PM(Server.Chatroom.BotName, Greets.GetPM(client));

                        if (Settings.GreetMsg)
                            Server.Print(client.Vroom, Greets.GetGreet(client), true);
                    }

                    if (Settings.History)
                        History.Show(client);
                }

                if (Muzzles.IsMuzzled(client))
                    client.Muzzled = true;

                CustomNames.Set(client);
                ILevel level = AutoLogin.GetLevel(client);

                if (level > ILevel.Regular)
                    client.SetLevel(level);

                if (Settings.LastSeen)
                {
                    LastSeenResult lsr = Whowas.Last(client);

                    if (lsr != null)
                    {
                        String lastseen = Template.Text(Category.Notification, 6);
                        lastseen = lastseen.Replace("+n", client.Name);
                        lastseen = lastseen.Replace("+ip", client.ExternalIP.ToString());
                        lastseen = lastseen.Replace("+o", lsr.Name);
                        lastseen = lastseen.Replace("+t", Helpers.UnixTimeToString((uint)lsr.Time));
                        Server.Print(lastseen, true);
                    }
                }

                if (Settings.Filtering)
                    JoinFilter.DoPostFilter(client);
            }

            String forced_pm = AvatarPMManager.GetPM(client);

            if (forced_pm != null)
                client.PersonalMessage = forced_pm;

            VSpy.Join(client);
            Whowas.Add(client);
            IPSend.Join(client);
        }

        public void Rejected(IUser client, RejectedMsg msg)
        {
            switch (msg)
            {
                case RejectedMsg.Banned:
                    client.Print(Template.Text(Category.Rejected, 5).Replace("+n", client.Name));
                    BanSend.Rejected(client, Template.Text(Category.BanSend, 0).Replace("+n", client.Name).Replace("+ip", client.ExternalIP.ToString()));
                    break;

                case RejectedMsg.NameInUse:
                    client.Print(Template.Text(Category.Rejected, 0).Replace("+n", client.Name));
                    BanSend.Rejected(client, Template.Text(Category.BanSend, 1).Replace("+n", client.Name).Replace("+ip", client.ExternalIP.ToString()));
                    break;

                case RejectedMsg.TooManyClients:
                    client.Print(Template.Text(Category.Rejected, 1).Replace("+n", client.Name));
                    BanSend.Rejected(client, Template.Text(Category.BanSend, 2).Replace("+n", client.Name).Replace("+ip", client.ExternalIP.ToString()));
                    break;

                case RejectedMsg.TooSoon:
                    client.Print(Template.Text(Category.Rejected, 4).Replace("+n", client.Name));
                    BanSend.Rejected(client, Template.Text(Category.BanSend, 3).Replace("+n", client.Name).Replace("+ip", client.ExternalIP.ToString()));
                    break;

                case RejectedMsg.UnacceptableGender:
                    client.Print(Template.Text(Category.Rejected, 9).Replace("+n", client.Name));
                    BanSend.Rejected(client, Template.Text(Category.BanSend, 4).Replace("+n", client.Name).Replace("+ip", client.ExternalIP.ToString()));
                    break;

                case RejectedMsg.UnderAge:
                    client.Print(Template.Text(Category.Rejected, 8).Replace("+n", client.Name).Replace("+a", Server.Chatroom.MinimumAge.ToString()));
                    BanSend.Rejected(client, Template.Text(Category.BanSend, 5).Replace("+n", client.Name).Replace("+ip", client.ExternalIP.ToString()));
                    break;
            }
        }

        public void Parting(IUser client)
        {
            if (!client.Link.IsLinked)
            {
                VSpy.Remove(client);
                IPSend.Remove(client);
                BanSend.Remove(client);
                LogSend.Remove(client);
                AntiFlood.Remove(client);
            }
        }

        public void Parted(IUser client)
        {
            VSpy.Part(client);
        }

        public bool AvatarReceived(IUser client)
        {
            return AvatarPMManager.CanAvatar(client);
        }

        public bool PersonalMessageReceived(IUser client, String text)
        {
            String forced_pm = AvatarPMManager.GetPM(client);

            if (forced_pm != null)
                if (forced_pm != text)
                    return false;

            return true;
        }

        public void TextReceived(IUser client, String text)
        {
            if (client.Muzzled)
            {
                String str = "\x000314[muzzled] " + client.Name + "> " + Helpers.StripColors(text);
                Server.Print(ILevel.Host, str);
            }
        }

        public String TextSending(IUser client, String text)
        {
            if (client.Muzzled)
                client.Print(Template.Text(Category.Notification, 0));
            else
            {
                if (text.StartsWith("#host") || text.StartsWith("#whisper") ||
                    text.StartsWith("#clone") || text.StartsWith("#cloak"))
                    return String.Empty;

                if (client.Level > ILevel.Regular && text.StartsWith("#"))
                    if (Settings.Stealth)
                        return String.Empty;

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

                if (client.Level == ILevel.Regular && !Settings.Colors)
                    text = Helpers.StripColors(text);

                if (Settings.Filtering)
                    text = WordFilter.FilterBefore(client, text);
            }

            return text;
        }

        public void TextSent(IUser client, String text)
        {
            String echo = Echo.IsEcho(client);

            if (echo != null)
                client.SendText(echo);

            History.Add(client.Name, text, false);
            VSpy.Text(client, text);

            if (Settings.Filtering)
                WordFilter.FilterAfter(client, text);
        }

        public void EmoteReceived(IUser client, String text)
        {
            if (client.Muzzled)
            {
                String str = "\x000314[muzzled] * " + client.Name + " " + Helpers.StripColors(text);
                Server.Print(ILevel.Host, str);
            }
        }

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

                if (Settings.Filtering)
                    text = WordFilter.FilterBefore(client, text);
            }

            return text;
        }

        public void EmoteSent(IUser client, String text)
        {
            String echo = Echo.IsEcho(client);

            if (echo != null)
                client.SendEmote(echo);

            History.Add(client.Name, text, true);
            VSpy.Text(client, text);

            if (Settings.Filtering)
                WordFilter.FilterAfter(client, text);
        }

        public bool CanPrivateMessage(IUser client, IUser target)
        {
            return !PMBlocking.IsBlocking(target);
        }

        public void PrivateSending(IUser client, IUser target, IPrivateMsg msg)
        {
            if (PMBlocking.IsBlocking(target))
                if (target.Level > client.Level || client.Level == ILevel.Regular)
                {
                    msg.Cancel = true;
                    client.PM(target.Name, Template.Text(Category.PmBlocking, 2).Replace("+n", client.Name).Replace("+t", target.Name));
                    return;
                }

            if (Settings.Filtering)
                WordFilter.FilterPM(client, msg);
        }

        public void PrivateSent(IUser client, IUser target) { }

        public void BotPrivateSent(IUser client, String text) { }

        public bool Nick(IUser client, String name)
        {
            Server.Print(Template.Text(Category.Notification, 13).Replace("+o", client.Name).Replace("+n", name), true);
            return true;
        }

        public void Help(IUser admin)
        {
            if (Settings.DisableAdmins && admin.Level < ILevel.Host)
                return;

            admin.Print("/id");
            admin.Print("/info");

            if (admin.Level > ILevel.Regular || Settings.General)
            {
                admin.Print("/vroom <number>");
                admin.Print("/customname <user> <custom name>");
                admin.Print("/uncustomname <user> <custom name>");
                admin.Print("/pmblock <on or off>");
                admin.Print("/shout <message>");
                admin.Print("/whisper <user> <message>");
                admin.Print("/locate");
                admin.Print("/viewmotd");
            }

            if (admin.Level >= Server.GetLevel("ban"))
                admin.Print("/ban <user> [<message>]");
            if (admin.Level >= Server.GetLevel("ban10"))
                admin.Print("/ban10 <user> [<message>]");
            if (admin.Level >= Server.GetLevel("ban60"))
                admin.Print("/ban60 <user> [<message>]");
            if (admin.Level >= Server.GetLevel("unban"))
                admin.Print("/unban <user>");
            if (admin.Level >= Server.GetLevel("listbans"))
                admin.Print("/listbans");
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
            if (admin.Owner)
                admin.Print("/listpasswords");
            if (admin.Owner)
                admin.Print("/rempassword <id>");
            if (admin.Level >= Server.GetLevel("roomsearch"))
                admin.Print("/roomsearch <name>");
            if (admin.Level >= Server.GetLevel("mtimeout"))
                admin.Print("/mtimeout <minutes>");
            if (admin.Level >= Server.GetLevel("redirect"))
                admin.Print("/redirect <user> <hashlink>");
            if (admin.Level >= Server.GetLevel("sharefiles"))
                admin.Print("/sharefiles <on or off>");
            if (admin.Level >= Server.GetLevel("idle"))
                admin.Print("/idle <on or off>");
            if (admin.Level >= Server.GetLevel("clock"))
                admin.Print("/clock <on or off>");
            if (admin.Level >= Server.GetLevel("addtopic"))
                admin.Print("/addtopic <text>");
            if (admin.Level >= Server.GetLevel("remtopic"))
                admin.Print("/remtopic");
            if (admin.Level >= Server.GetLevel("greetmsg"))
                admin.Print("/greetmsg <on or off>");
            if (admin.Level >= Server.GetLevel("addgreetmsg"))
                admin.Print("/addgreetmsg <message>");
            if (admin.Level >= Server.GetLevel("remgreetmsg"))
                admin.Print("/remgreetmsg <id>");
            if (admin.Level >= Server.GetLevel("listgreetmsg"))
                admin.Print("/listgreetmsg");
            if (admin.Level >= Server.GetLevel("pmgreetmsg"))
                admin.Print("/pmgreetmsg <on or off>");
            if (admin.Level >= Server.GetLevel("pmgreetmsg"))
                admin.Print("/pmgreetmsg <message>");
            if (admin.Level >= Server.GetLevel("caps"))
                admin.Print("/caps <on or off>");
            if (admin.Level >= Server.GetLevel("anon"))
                admin.Print("/anon <on or off>");
            if (admin.Level >= Server.GetLevel("customnames"))
                admin.Print("/customnames <on or off>");
            if (admin.Level >= Server.GetLevel("general"))
                admin.Print("/general <on or off>");
            if (admin.Level >= Server.GetLevel("url"))
                admin.Print("/url <on or off>");
            if (admin.Level >= Server.GetLevel("addurl"))
                admin.Print("/addurl <address> <text>");
            if (admin.Level >= Server.GetLevel("remurl"))
                admin.Print("/remurl <id>");
            if (admin.Level >= Server.GetLevel("listurls"))
                admin.Print("/listurls");
            if (admin.Level >= Server.GetLevel("roominfo"))
                admin.Print("/roominfo <on or off>");
            if (admin.Level >= Server.GetLevel("status"))
                admin.Print("/status <host status>");
            if (admin.Level >= Server.GetLevel("lastseen"))
                admin.Print("/lastseen <on or off>");
            if (admin.Level >= Server.GetLevel("history"))
                admin.Print("/history <on or off>");
            if (admin.Level >= Server.GetLevel("pmroom"))
                admin.Print("/pmroom <text>");
            if (admin.Level == ILevel.Host)
                admin.Print("/hostkill <user>");
            if (admin.Level == ILevel.Host)
                admin.Print("/hostban <user>");
            if (admin.Level == ILevel.Host)
                admin.Print("/hostunban <user>");
            if (admin.Level == ILevel.Host)
                admin.Print("/hostmuzzle <user>");
            if (admin.Level == ILevel.Host)
                admin.Print("/hostunmuzzle <user>");
            if (admin.Level == ILevel.Host)
                admin.Print("/hostcbans");
            if (admin.Level == ILevel.Host)
                admin.Print("/hostclone <user> <text>");
            if (admin.Level >= Server.GetLevel("loadmotd"))
                admin.Print("/loadmotd");
            if (admin.Level >= Server.GetLevel("disableadmins"))
                admin.Print("/enableadmins");
            if (admin.Level >= Server.GetLevel("disableadmins"))
                admin.Print("/disableadmins");
            if (admin.Level >= Server.GetLevel("stealth"))
                admin.Print("/stealth <on or off>");
            if (admin.Level >= Server.GetLevel("cloak"))
                admin.Print("/cloak <on or off>");
            if (admin.Level >= Server.GetLevel("disableavatar"))
                admin.Print("/disableavatar <user>");
            if (admin.Level >= Server.GetLevel("changemessage"))
                admin.Print("/changemessage <user> <message>");
            if (admin.Level >= Server.GetLevel("clearscreen"))
                admin.Print("/clearscreen");
            if (admin.Level >= Server.GetLevel("urban"))
                admin.Print("/urban <text>");
            if (admin.Level >= Server.GetLevel("define"))
                admin.Print("/define <text>");
            if (admin.Level >= Server.GetLevel("trace"))
                admin.Print("/trace <user or ip>");
            if (admin.Level >= Server.GetLevel("clone"))
                admin.Print("/clone <user> <text>");
            if (admin.Level >= Server.GetLevel("move"))
                admin.Print("/move <user> <vroom>");
            if (admin.Level >= Server.GetLevel("changename"))
                admin.Print("/changename <user> <name>");
            if (admin.Level >= Server.GetLevel("oldname"))
                admin.Print("/oldname <user>");
            if (admin.Level >= Server.GetLevel("announce"))
                admin.Print("/announce <text>");
            if (admin.Level >= Server.GetLevel("colors"))
                admin.Print("/colors <on or off>");
            if (admin.Level >= Server.GetLevel("banstats"))
                admin.Print("/banstats");
            if (admin.Level >= Server.GetLevel("vspy"))
                admin.Print("/vspy <on or off>");
            if (admin.Level >= Server.GetLevel("whois"))
                admin.Print("/whois <user>");
            if (admin.Level >= Server.GetLevel("stats"))
                admin.Print("/stats");
            if (admin.Level >= Server.GetLevel("whowas"))
                admin.Print("/whowas <query>");
            if (admin.Level >= Server.GetLevel("ipsend"))
                admin.Print("/ipsend <on or off>");
            if (admin.Level >= Server.GetLevel("bansend"))
                admin.Print("/bansend <on or off>");
            if (admin.Level >= Server.GetLevel("logsend"))
                admin.Print("/logsend <on or off>");
            if (admin.Level >= Server.GetLevel("adminannounce"))
                admin.Print("/adminannounce <on or off>");
            if (admin.Level >= Server.GetLevel("filter"))
            {
                admin.Print("/filter <on or off>");
                admin.Print("/addjoinfilter <trigger>, <type>[, <args>]");
                admin.Print("/remjoinfilter <ident>");
                admin.Print("/joinfilters");
                admin.Print("/addfilefilter <trigger>, <type>[, <args>]");
                admin.Print("/remfilefilter <ident>");
                admin.Print("/filefilters");
                admin.Print("/addwordfilter <trigger>, <type>[, <args>]");
                admin.Print("/remwordfilter <ident>");
                admin.Print("/wordfilters");
                admin.Print("/addline <ident>, <text>");
                admin.Print("/remline <ident>, <line ident>");
                admin.Print("/viewfilter <ident>");
            }
            if (admin.Level >= Server.GetLevel("loadtemplate"))
                admin.Print("/loadtemplate");
            if (admin.Level >= Server.GetLevel("listquarantined"))
                admin.Print("/listquarantined");
            if (admin.Level >= Server.GetLevel("unquarantine"))
                admin.Print("/unquarantine <id>");
        }

        public void FileReceived(IUser client, String filename, String title, MimeType type)
        {
            if (client.Level < ILevel.Administrator)
                if (type == MimeType.ARES_MIME_IMAGE ||
                    type == MimeType.ARES_MIME_OTHER ||
                    type == MimeType.ARES_MIME_SOFTWARE ||
                    type == MimeType.ARES_MIME_VIDEO)
                    if (Settings.Filtering)
                        FileFilter.DoFilter(client, filename);
        }

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

        public void InvalidRegistration(IUser client)
        {
            client.Print(Template.Text(Category.AdminLogin, 6).Replace("+n", client.Name));
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

        public void VroomChanged(IUser client)
        {
            if (!client.Link.IsLinked)
                if (client.Vroom == 0)
                {
                    if (Settings.Clock)
                        Topics.ClockTo(client);
                    else
                        client.Topic(Server.Chatroom.Topic);
                }
                else
                {
                    String topic = Topics.GetTopic(client.Vroom);

                    if (!String.IsNullOrEmpty(topic))
                        client.Topic(topic);
                }

            VSpy.VroomChanged(client);
        }

        private byte last_flood { get; set; }

        public bool Flooding(IUser client, byte msg)
        {
            this.last_flood = msg;
            return !AntiFlood.CanFlood(client);
        }

        public void Flooded(IUser client)
        {
            String text = Template.Text(Category.Notification, 28).Replace("+n", client.Name).Replace("+c", this.last_flood.ToString());

            if (client.Captcha)
                Server.Print(ILevel.Moderator, text, true);
        }

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
            Muzzles.Clear();
            Bans.Clear();
        }

        public void Command(IUser client, String cmd, IUser target, String args)
        {
            if (Settings.DisableAdmins && client.Level < ILevel.Host)
                return;

            if (client.Level > ILevel.Regular)
                if (!cmd.StartsWith("whisper"))
                    if (!cmd.StartsWith("host"))
                        if (!cmd.StartsWith("jsmsg"))
                            LogSend.Log(client, cmd);

            if (cmd == "version")
                client.Print(Server.Chatroom.Version);
            else if (cmd.StartsWith("vroom "))
                Eval.Vroom(client, cmd.Substring(6));
            else if (cmd == "id")
                Eval.ID(client);
            else if (cmd == "info")
                Eval.Info(client);
            else if (cmd.StartsWith("ban "))
                Eval.Ban(client, target, args);
            else if (cmd.StartsWith("ban10 "))
                Eval.Ban10(client, target, args);
            else if (cmd.StartsWith("ban60 "))
                Eval.Ban60(client, target, args);
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
            else if (cmd.StartsWith("roomsearch "))
                Eval.RoomSearch(client, cmd.Substring(11));
            else if (cmd.StartsWith("mtimeout "))
                Eval.MTimeout(client, cmd.Substring(9));
            else if (cmd.StartsWith("redirect "))
                Eval.Redirect(client, target, args);
            else if (cmd.StartsWith("sharefiles "))
                Eval.ShareFiles(client, cmd.Substring(11));
            else if (cmd.StartsWith("idle "))
                Eval.IdleMonitoring(client, cmd.Substring(5));
            else if (cmd.StartsWith("clock "))
                Eval.Clock(client, cmd.Substring(6));
            else if (cmd.StartsWith("addtopic "))
                Eval.AddTopic(client, cmd.Substring(9));
            else if (cmd == "remtopic")
                Eval.RemTopic(client);
            else if (cmd == "listbans")
                Eval.ListBans(client);
            else if (cmd.StartsWith("listgreetmsg"))
                Eval.ListGreetMsg(client);
            else if (cmd.StartsWith("greetmsg "))
                Eval.GreetMsg(client, cmd.Substring(9));
            else if (cmd.StartsWith("addgreetmsg "))
                Eval.AddGreetMsg(client, cmd.Substring(12));
            else if (cmd.StartsWith("remgreetmsg "))
                Eval.RemGreetMsg(client, cmd.Substring(12));
            else if (cmd.StartsWith("pmgreetmsg "))
                Eval.PMGreetMsg(client, cmd.Substring(11));
            else if (cmd.StartsWith("caps "))
                Eval.Caps(client, cmd.Substring(5));
            else if (cmd.StartsWith("anon "))
                Eval.Anon(client, cmd.Substring(5));
            else if (cmd.StartsWith("customnames"))
                Eval.CustomNames(client, cmd.Substring(11));
            else if (cmd.StartsWith("general "))
                Eval.General(client, cmd.Substring(8));
            else if (cmd.StartsWith("url "))
                Eval.Url(client, cmd.Substring(4));
            else if (cmd.StartsWith("addurl "))
                Eval.AddUrl(client, cmd.Substring(7));
            else if (cmd.StartsWith("remurl "))
                Eval.RemUrl(client, cmd.Substring(7));
            else if (cmd == "listurl" || cmd == "listurls")
                Eval.ListUrls(client);
            else if (cmd.StartsWith("roominfo "))
                Eval.RoomInfo(client, cmd.Substring(9));
            else if (cmd.StartsWith("status "))
                Eval.Status(client, cmd.Substring(7));
            else if (cmd.StartsWith("lastseen "))
                Eval.LastSeen(client, cmd.Substring(9));
            else if (cmd.StartsWith("history "))
                Eval.History(client, cmd.Substring(8));
            else if (cmd == "locate")
                Eval.Locate(client);
            else if (cmd == "viewmotd")
                Eval.ViewMotd(client);
            else if (cmd.StartsWith("pmroom "))
                Eval.PMRoom(client, cmd.Substring(7));
            else if (cmd.StartsWith("hostkill ") || cmd.StartsWith("hostkick "))
                Eval.HostKill(client, target);
            else if (cmd.StartsWith("hostban "))
                Eval.HostBan(client, target);
            else if (cmd.StartsWith("hostunban "))
                Eval.HostUnban(client, cmd.Substring(10));
            else if (cmd.StartsWith("hostmuzzle "))
                Eval.HostMuzzle(client, target);
            else if (cmd.StartsWith("hostunmuzzle "))
                Eval.HostUnmuzzle(client, target);
            else if (cmd.StartsWith("hostcban"))
                Eval.HostCBans(client);
            else if (cmd.StartsWith("hostclone "))
                Eval.HostClone(client, target, args);
            else if (cmd == "loadmotd")
                Eval.LoadMotd(client);
            else if (cmd == "enableadmins")
                Eval.EnableAdmins(client);
            else if (cmd == "disableadmins")
                Eval.DisableAdmins(client);
            else if (cmd.StartsWith("stealth "))
                Eval.Stealth(client, cmd.Substring(8));
            else if (cmd.StartsWith("cloak "))
                Eval.Cloak(client, cmd.Substring(6));
            else if (cmd.StartsWith("disableavatar "))
                Eval.DisableAvatar(client, target);
            else if (cmd.StartsWith("changemessage "))
                Eval.ChangeMessage(client, target, args);
            else if (cmd == "clearscreen")
                Eval.ClearScreen(client);
            else if (cmd.StartsWith("urban "))
                Eval.Urban(client, cmd.Substring(6));
            else if (cmd.StartsWith("define "))
                Eval.Define(client, cmd.Substring(7));
            else if (cmd.StartsWith("trace "))
                Eval.Trace(client, target, cmd.Substring(6));
            else if (cmd.StartsWith("clone "))
                Eval.Clone(client, target, args);
            else if (cmd.StartsWith("move "))
                Eval.Move(client, target, args);
            else if (cmd.StartsWith("changename "))
                Eval.ChangeName(client, target, args);
            else if (cmd.StartsWith("oldname "))
                Eval.OldName(client, target);
            else if (cmd.StartsWith("announce "))
                Eval.Announce(client, cmd.Substring(9));
            else if (cmd.StartsWith("colors "))
                Eval.Colors(client, cmd.Substring(7));
            else if (cmd == "banstats")
                Eval.BanStats(client);
            else if (cmd.StartsWith("vspy "))
                Eval.Vspy(client, cmd.Substring(5));
            else if (cmd.StartsWith("whois ")) //93
                Eval.Whois(client, target);
            else if (cmd == "stats")
                Eval.Stats(client);
            else if (cmd.StartsWith("whowas "))
                Eval.Whowas(client, cmd.Substring(7));
            else if (cmd.StartsWith("ipsend "))
                Eval.IPSend(client, cmd.Substring(7));
            else if (cmd.StartsWith("bansend "))
                Eval.BanSend(client, cmd.Substring(8));
            else if (cmd.StartsWith("logsend "))
                Eval.LogSend(client, cmd.Substring(8));
            else if (cmd.StartsWith("addjoinfilter "))
                Eval.AddJoinFilter(client, cmd.Substring(14));
            else if (cmd.StartsWith("remjoinfilter "))
                Eval.RemJoinFilter(client, cmd.Substring(14));
            else if (cmd == "joinfilters")
                Eval.JoinFilters(client);
            else if (cmd.StartsWith("filter "))
                Eval.Filter(client, cmd.Substring(7));
            else if (cmd.StartsWith("addfilefilter "))
                Eval.AddFileFilter(client, cmd.Substring(14));
            else if (cmd.StartsWith("remfilefilter "))
                Eval.RemFileFilter(client, cmd.Substring(14));
            else if (cmd == "filefilters")
                Eval.FileFilters(client);
            else if (cmd.StartsWith("addwordfilter "))
                Eval.AddWordFilter(client, cmd.Substring(14));
            else if (cmd.StartsWith("remwordfilter "))
                Eval.RemWordFilter(client, cmd.Substring(14));
            else if (cmd == "wordfilters")
                Eval.WordFilters(client);
            else if (cmd.StartsWith("adminannounce "))
                Eval.AdminAnnounce(client, cmd.Substring(14));
            else if (cmd.StartsWith("addline "))
                Eval.AddLine(client, cmd.Substring(8));
            else if (cmd.StartsWith("remline "))
                Eval.RemLine(client, cmd.Substring(8));
            else if (cmd.StartsWith("viewfilter "))
                Eval.ViewFilter(client, cmd.Substring(11));
            else if (cmd == "loadtemplate")
                Eval.LoadTemplate(client);
            else if (cmd == "listpasswords")
                Eval.ListPasswords(client);
            else if (cmd.StartsWith("rempassword "))
                Eval.RemovePassword(client, cmd.Substring(12));
            else if (cmd == "listquarantined")
                Eval.ListQuarantined(client);
            else if (cmd.StartsWith("unquarantine "))
                Eval.Unquarantine(client, cmd.Substring(13));
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
