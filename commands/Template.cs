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
using System.IO;

namespace commands
{
    class Template
    {
        public static String Text(Category category, int index)
        {
            return Helpers.SetColors(list.Find(x => x.Category == category && x.Index == index).Text);
        }

        private class Item
        {
            public Category Category { get; set; }
            public String Text { get; set; }
            public int Index { get; set; }
        }

        private static List<Item> list = new List<Item>();

        public static void Load(bool announce_loaded)
        {
            list = new List<Item>();

            list.Add(new Item { Category = Category.Credit, Index = 0, Text = "Default Template" });//97

            list.Add(new Item { Category = Category.AdminAction, Index = 0, Text = "+n was banned by +a" });//90
            list.Add(new Item { Category = Category.AdminAction, Index = 1, Text = "+n was unbanned by +a" });//91
            list.Add(new Item { Category = Category.AdminAction, Index = 2, Text = "+n was kicked by +a" });//89
            list.Add(new Item { Category = Category.AdminAction, Index = 3, Text = "+n was muzzled by +a" });//92
            list.Add(new Item { Category = Category.AdminAction, Index = 4, Text = "+n was unmuzzled by +a" });//93
            list.Add(new Item { Category = Category.AdminAction, Index = 5, Text = "+n's custom name has been set by +a" });//77
            list.Add(new Item { Category = Category.AdminAction, Index = 6, Text = "+n's custom name has been unset by +a" });//78
            list.Add(new Item { Category = Category.AdminAction, Index = 7, Text = "+n has been set kewl text by +a" });//79
            list.Add(new Item { Category = Category.AdminAction, Index = 8, Text = "+n has been unset kewl text by +a" });//80
            list.Add(new Item { Category = Category.AdminAction, Index = 9, Text = "+n has been lowered by +a" });//81
            list.Add(new Item { Category = Category.AdminAction, Index = 10, Text = "+n has been unlowered by +a" });//82
            list.Add(new Item { Category = Category.AdminAction, Index = 11, Text = "+n has been kiddied by +a" });//83
            list.Add(new Item { Category = Category.AdminAction, Index = 12, Text = "+n has been unkiddied by +a" });//84
            list.Add(new Item { Category = Category.AdminAction, Index = 13, Text = "+n has been echoed by +a" });//85
            list.Add(new Item { Category = Category.AdminAction, Index = 14, Text = "+n has been unechoed by +a" });//86
            list.Add(new Item { Category = Category.AdminAction, Index = 15, Text = "+n has been painted by +a" });//87
            list.Add(new Item { Category = Category.AdminAction, Index = 16, Text = "+n has been unpainted by +a" });//88
            list.Add(new Item { Category = Category.AdminAction, Index = 17, Text = "+r has been range banned by +a" });//94
            list.Add(new Item { Category = Category.AdminAction, Index = 18, Text = "+r has been range unbanned by +a" });//95
            list.Add(new Item { Category = Category.AdminAction, Index = 19, Text = "+a has cleared the ban list" });//96
            list.Add(new Item { Category = Category.AdminAction, Index = 20, Text = "+n has been redirected to +r by +a" });//115
            list.Add(new Item { Category = Category.AdminAction, Index = 21, Text = "+n was banned for 10 minutes by +a" });
            list.Add(new Item { Category = Category.AdminAction, Index = 22, Text = "+n was banned for 60 minutes by +a" });
            list.Add(new Item { Category = Category.AdminAction, Index = 23, Text = "+n's avatar was disabled by +a" });
            list.Add(new Item { Category = Category.AdminAction, Index = 24, Text = "+n's personal message was set by +a" });

            list.Add(new Item { Category = Category.Notification, Index = 0, Text = "you are muzzled" });
            list.Add(new Item { Category = Category.Notification, Index = 1, Text = "ban list is empty" });
            list.Add(new Item { Category = Category.Notification, Index = 2, Text = "+n is ignored" });
            list.Add(new Item { Category = Category.Notification, Index = 3, Text = "+n is unignored" });
            list.Add(new Item { Category = Category.Notification, Index = 4, Text = "+n your CAPS have been auto-disabled" });//17
            list.Add(new Item { Category = Category.Notification, Index = 5, Text = "All ban lists have been automatically clear as scheduled" });//132
            list.Add(new Item { Category = Category.Notification, Index = 6, Text = "+n was last seen as +o at +t from +ip" });//103
            list.Add(new Item { Category = Category.Notification, Index = 7, Text = "-=-=-=-=- end of chat history -=-=-=-=-" });//128
            list.Add(new Item { Category = Category.Notification, Index = 8, Text = "MOTD reloaded by +n" });
            list.Add(new Item { Category = Category.Notification, Index = 9, Text = "Admin commands enabled by +n" });
            list.Add(new Item { Category = Category.Notification, Index = 10, Text = "Admin commands disabled by +n" });
            list.Add(new Item { Category = Category.Notification, Index = 11, Text = "+n has cloaked" });
            list.Add(new Item { Category = Category.Notification, Index = 12, Text = "+n has uncloaked" });
            list.Add(new Item { Category = Category.Notification, Index = 13, Text = "+o is now known as +n" });
            list.Add(new Item { Category = Category.Notification, Index = 14, Text = "screen cleared by +n" });
            list.Add(new Item { Category = Category.Notification, Index = 15, Text = "+n was cloned by +a" });
            list.Add(new Item { Category = Category.Notification, Index = 16, Text = "+n was moved to vroom +v by +a" });
            list.Add(new Item { Category = Category.Notification, Index = 17, Text = "+n's name was changed by +a" });
            list.Add(new Item { Category = Category.Notification, Index = 18, Text = "+n's name was restored by +a" });
            list.Add(new Item { Category = Category.Notification, Index = 19, Text = "+a announced" });
            list.Add(new Item { Category = Category.Notification, Index = 20, Text = "+n has been added to vspy" });
            list.Add(new Item { Category = Category.Notification, Index = 21, Text = "+n has been removed from vspy" });
            list.Add(new Item { Category = Category.Notification, Index = 22, Text = "+n has been added to ipsend" });
            list.Add(new Item { Category = Category.Notification, Index = 23, Text = "+n has been removed from ipsend" });
            list.Add(new Item { Category = Category.Notification, Index = 24, Text = "+n has been added to bansend" });
            list.Add(new Item { Category = Category.Notification, Index = 25, Text = "+n has been removed from bansend" });
            list.Add(new Item { Category = Category.Notification, Index = 26, Text = "+n has been added to logsend" });
            list.Add(new Item { Category = Category.Notification, Index = 27, Text = "+n has been removed from logsend" });
            list.Add(new Item { Category = Category.Notification, Index = 28, Text = "+n has flooded (code: +c)" });
            list.Add(new Item { Category = Category.Notification, Index = 29, Text = "your admin level is too low to use this command on +n" });

            list.Add(new Item { Category = Category.Rejected, Index = 0, Text = "The name +n is already in use!" });//1
            list.Add(new Item { Category = Category.Rejected, Index = 1, Text = "+n there are too many clients connected from your IP address!" });//2
            list.Add(new Item { Category = Category.Rejected, Index = 2, Text = "+n please go to control panel and select a username and then rejoin!" });//3
            list.Add(new Item { Category = Category.Rejected, Index = 3, Text = "+n please share some files and then rejoin!" });//4
            list.Add(new Item { Category = Category.Rejected, Index = 4, Text = "+n you rejoined too quick, please wait a short while and try again!" });//5
            list.Add(new Item { Category = Category.Rejected, Index = 5, Text = "+n you are banned from this chatroom!" });//6
            list.Add(new Item { Category = Category.Rejected, Index = 6, Text = "+n your IP address falls into a banned range!" });//7
            list.Add(new Item { Category = Category.Rejected, Index = 7, Text = "+n you were rejected by the Join Filter!" });//8
            list.Add(new Item { Category = Category.Rejected, Index = 8, Text = "+n you must be at least +a years old to enter this room!" });//114
            list.Add(new Item { Category = Category.Rejected, Index = 9, Text = "+n your gender is not allowed in this chatroom!" });

            list.Add(new Item { Category = Category.AdminLogin, Index = 0, Text = "+n has logged in as a level +l admin" });//12
            list.Add(new Item { Category = Category.AdminLogin, Index = 1, Text = "+n attempted to log in with an invalid password!!!" });//13
            list.Add(new Item { Category = Category.AdminLogin, Index = 2, Text = "+n has been banned for too many invalid login attempts!!!" });//14
            list.Add(new Item { Category = Category.AdminLogin, Index = 3, Text = "+n is no longer an admin" });//16
            list.Add(new Item { Category = Category.AdminLogin, Index = 4, Text = "+n has been added to auto login as a level +l admin" });//9
            list.Add(new Item { Category = Category.AdminLogin, Index = 5, Text = "+n has been removed from auto login" });//10
            list.Add(new Item { Category = Category.AdminLogin, Index = 6, Text = "+n your password must include at least one letter and one number!!" });
            list.Add(new Item { Category = Category.AdminLogin, Index = 7, Text = "+n's password and account has been removed" });

            list.Add(new Item { Category = Category.Registration, Index = 0, Text = "Your account registration was successfully created" });
            list.Add(new Item { Category = Category.Registration, Index = 1, Text = "Your login was successful" });
            list.Add(new Item { Category = Category.Registration, Index = 2, Text = "You are no longer registered" });

            list.Add(new Item { Category = Category.Idle, Index = 0, Text = "+n idles at +t" });//18
            list.Add(new Item { Category = Category.Idle, Index = 1, Text = "+n returned at +t - away time [+s seconds]" });
            list.Add(new Item { Category = Category.Idle, Index = 2, Text = "+n returned at +t - away time [+m minutes +s seconds]" });
            list.Add(new Item { Category = Category.Idle, Index = 3, Text = "+n returned at +t - away time [+h hours +m minutes +s seconds]" });
            list.Add(new Item { Category = Category.Idle, Index = 4, Text = "+n returned at +t - away time [+d days +h hours +m minutes +s seconds]" });

            list.Add(new Item { Category = Category.PmBlocking, Index = 0, Text = "PM blocking is now active for screen name: +n" });//20
            list.Add(new Item { Category = Category.PmBlocking, Index = 1, Text = "PM blocking is now inactive for screen name: +n" });//21
            list.Add(new Item { Category = Category.PmBlocking, Index = 2, Text = "Sorry +n, but +t has PM blocking active and was unable to receive your message" });//22

            list.Add(new Item { Category = Category.Messaging, Index = 0, Text = "+n> [SHOUT] +t" });//26
            list.Add(new Item { Category = Category.Messaging, Index = 1, Text = "+n> [ADMIN] +t" });//27
            list.Add(new Item { Category = Category.Messaging, Index = 2, Text = "whisper to +n: +t" });
            list.Add(new Item { Category = Category.Messaging, Index = 3, Text = "whisper from +n: +t" });

            list.Add(new Item { Category = Category.Captcha, Index = 0, Text = "Too many incorrect answers" });//152
            list.Add(new Item { Category = Category.Captcha, Index = 1, Text = "+a was an incorrect answer" });
            list.Add(new Item { Category = Category.Captcha, Index = 2, Text = "Please type this word to allow you to chat" });
            list.Add(new Item { Category = Category.Captcha, Index = 3, Text = "Correct answer.  You may now chat" });//155

            list.Add(new Item { Category = Category.Linking, Index = 0, Text = "\x0002314--- A linking error occurred [+e]" });
            list.Add(new Item { Category = Category.Linking, Index = 1, Text = "\x0002314--- Connection to Link Hub [+n] established :)" });
            list.Add(new Item { Category = Category.Linking, Index = 2, Text = "\x0002314--- Connection to Link Hub was lost." });
            list.Add(new Item { Category = Category.Linking, Index = 3, Text = "\x0002314--- A chatroom [+n] has joined" });
            list.Add(new Item { Category = Category.Linking, Index = 4, Text = "\x0002314--- A chatroom [+n] has parted" });
            list.Add(new Item { Category = Category.Linking, Index = 5, Text = "\x0002314--- Connecting to hub [+n], please wait..." });
            list.Add(new Item { Category = Category.Linking, Index = 6, Text = "\x0002314--- Link session has now been terminated" });
            list.Add(new Item { Category = Category.Linking, Index = 7, Text = "\x0002314--- The chatroom [+n] does not allow linked admins" });
            list.Add(new Item { Category = Category.Linking, Index = 8, Text = "\x0002314--- Reconnection will be attempted in 30 seconds..." });
            list.Add(new Item { Category = Category.Linking, Index = 9, Text = "\x0002314--- This chatroom is currently unlinked" });
            list.Add(new Item { Category = Category.Linking, Index = 10, Text = "\x0002314--- Name: +n" });
            list.Add(new Item { Category = Category.Linking, Index = 11, Text = "\x0002314--- Hashlink: \\\\+h" });

            list.Add(new Item { Category = Category.AdminList, Index = 0, Text = "ADMIN LIST REQUESTED BY [+n]" });//50
            list.Add(new Item { Category = Category.AdminList, Index = 1, Text = "Level +l : +n" });//51
            list.Add(new Item { Category = Category.AdminList, Index = 2, Text = "List Complete" });//52

            list.Add(new Item { Category = Category.RoomSearch, Index = 0, Text = "Room search service is not enabled" });
            list.Add(new Item { Category = Category.RoomSearch, Index = 1, Text = "Channel database is empty, try again later" });
            list.Add(new Item { Category = Category.RoomSearch, Index = 2, Text = "Unable to find any channels containing +n" });
            list.Add(new Item { Category = Category.RoomSearch, Index = 3, Text = "Results for +n as follows:" });
            list.Add(new Item { Category = Category.RoomSearch, Index = 4, Text = "Name: +n" });
            list.Add(new Item { Category = Category.RoomSearch, Index = 5, Text = "Topic: +t" });
            list.Add(new Item { Category = Category.RoomSearch, Index = 6, Text = "Language: +l | Server: +s | Users: +u" });
            list.Add(new Item { Category = Category.RoomSearch, Index = 7, Text = "Hashlink: \\\\+h" });

            list.Add(new Item { Category = Category.Timeouts, Index = 0, Text = "+n has set the muzzle timeout to +i" });//98
            list.Add(new Item { Category = Category.Timeouts, Index = 1, Text = "+n your muzzle timeout has expired" });//99
            list.Add(new Item { Category = Category.Timeouts, Index = 2, Text = "+n's ban timeout has expired" });

            list.Add(new Item { Category = Category.EnableDisable, Index = 0, Text = "+n has enabled File Share monitoring" });//53
            list.Add(new Item { Category = Category.EnableDisable, Index = 1, Text = "+n has disabled File Share monitoring" });//54
            list.Add(new Item { Category = Category.EnableDisable, Index = 2, Text = "+n has enabled Idle Monitoring" });//55
            list.Add(new Item { Category = Category.EnableDisable, Index = 3, Text = "+n has disabled Idle Monitoring" });//56
            list.Add(new Item { Category = Category.EnableDisable, Index = 4, Text = "+n enabled the topic clock" });//57
            list.Add(new Item { Category = Category.EnableDisable, Index = 5, Text = "+n disabled the topic clock" });//58
            list.Add(new Item { Category = Category.EnableDisable, Index = 6, Text = "+n has enabled the greet message" });//59
            list.Add(new Item { Category = Category.EnableDisable, Index = 7, Text = "+n has disabled the greet message" });//60
            list.Add(new Item { Category = Category.EnableDisable, Index = 8, Text = "+n has enabled the PM greet message" });//61
            list.Add(new Item { Category = Category.EnableDisable, Index = 9, Text = "+n has disabled the PM greet message" });//62
            list.Add(new Item { Category = Category.EnableDisable, Index = 10, Text = "+n has enabled CAPS monitoring" });//63
            list.Add(new Item { Category = Category.EnableDisable, Index = 11, Text = "+n has disabled CAPS monitoring" });//64
            list.Add(new Item { Category = Category.EnableDisable, Index = 12, Text = "+n has enabled Anon monitoring" });//65
            list.Add(new Item { Category = Category.EnableDisable, Index = 13, Text = "+n has disabled Anon monitoring" });//66
            list.Add(new Item { Category = Category.EnableDisable, Index = 14, Text = "+n has enabled custom names" });//69
            list.Add(new Item { Category = Category.EnableDisable, Index = 15, Text = "+n has disabled custom names" });//70
            list.Add(new Item { Category = Category.EnableDisable, Index = 16, Text = "+n has enabled general commands" });//71
            list.Add(new Item { Category = Category.EnableDisable, Index = 17, Text = "+n has disabled general commands" });//72
            list.Add(new Item { Category = Category.EnableDisable, Index = 18, Text = "dynamic url tag was enabled by +n" });//116
            list.Add(new Item { Category = Category.EnableDisable, Index = 19, Text = "dynamic url tag was disabled by +n" });//117
            list.Add(new Item { Category = Category.EnableDisable, Index = 20, Text = "+n has enabled Room Information Updates" });//111
            list.Add(new Item { Category = Category.EnableDisable, Index = 21, Text = "+n has disabled Room Information Updates" });//112
            list.Add(new Item { Category = Category.EnableDisable, Index = 22, Text = "+n has enabled Last Seen monitoring" });//101
            list.Add(new Item { Category = Category.EnableDisable, Index = 23, Text = "+n has disabled Last Seen monitoring" });//102
            list.Add(new Item { Category = Category.EnableDisable, Index = 24, Text = "+n has enabled chat history feature" });//126
            list.Add(new Item { Category = Category.EnableDisable, Index = 25, Text = "+n has disabled chat history feature" });//127
            list.Add(new Item { Category = Category.EnableDisable, Index = 26, Text = "+n has enabled stealth mode" });
            list.Add(new Item { Category = Category.EnableDisable, Index = 27, Text = "+n has disabled stealth mode" });
            list.Add(new Item { Category = Category.EnableDisable, Index = 28, Text = "+n has enabled colors" });
            list.Add(new Item { Category = Category.EnableDisable, Index = 29, Text = "+n has disabled colors" });
            list.Add(new Item { Category = Category.EnableDisable, Index = 30, Text = "+n has enabled room filters" });//73
            list.Add(new Item { Category = Category.EnableDisable, Index = 31, Text = "+n has disabled room filters" });//74

            list.Add(new Item { Category = Category.Clock, Index = 0, Text = "[+c] +t [+c]" });//100

            list.Add(new Item { Category = Category.Topics, Index = 0, Text = "+n has updated the topic for vroom +v" });//124
            list.Add(new Item { Category = Category.Topics, Index = 1, Text = "+n has removed the topic for vroom +v" });//125

            list.Add(new Item { Category = Category.Greetings, Index = 0, Text = "new greet message was added by +n" });//129
            list.Add(new Item { Category = Category.Greetings, Index = 1, Text = "greet message was removed by +n" });//130
            list.Add(new Item { Category = Category.Greetings, Index = 2, Text = "+n has updated the pm greeting" });
            list.Add(new Item { Category = Category.Greetings, Index = 3, Text = "greetmsg list is empty" });

            list.Add(new Item { Category = Category.Urls, Index = 0, Text = "url list is empty" });
            list.Add(new Item { Category = Category.Urls, Index = 1, Text = "dynamic url tag was added by +n" });//118
            list.Add(new Item { Category = Category.Urls, Index = 2, Text = "dynamic url tag was removed by +n" });//119

            list.Add(new Item { Category = Category.RoomInfo, Index = 0, Text = "Room Information" });//104
            list.Add(new Item { Category = Category.RoomInfo, Index = 1, Text = "Current hosts: +n" });//105
            list.Add(new Item { Category = Category.RoomInfo, Index = 2, Text = "Current user count: +n" });//106
            list.Add(new Item { Category = Category.RoomInfo, Index = 3, Text = "Current admin count: +n" });//107
            list.Add(new Item { Category = Category.RoomInfo, Index = 4, Text = "Server uptime: +n" });//108
            list.Add(new Item { Category = Category.RoomInfo, Index = 5, Text = "Host status: +n" });//110
            list.Add(new Item { Category = Category.RoomInfo, Index = 6, Text = "+n has updated the host status" });//113

            list.Add(new Item { Category = Category.Info, Index = 0, Text = "\x00027+r" });
            list.Add(new Item { Category = Category.Info, Index = 1, Text = "+n [vroom: +v] [id: +i]" });

            list.Add(new Item { Category = Category.Locate, Index = 0, Text = "\x00027vroom location list" });
            list.Add(new Item { Category = Category.Locate, Index = 1, Text = "+n is currently in vroom +v" });
            list.Add(new Item { Category = Category.Locate, Index = 2, Text = "\x00027end of list" });
            list.Add(new Item { Category = Category.Locate, Index = 3, Text = "location list empty" });

            list.Add(new Item { Category = Category.UrbanDictionary, Index = 0, Text = "unable to find urban definition for: +n" });
            list.Add(new Item { Category = Category.UrbanDictionary, Index = 1, Text = "urban definition for: +n" });
            list.Add(new Item { Category = Category.UrbanDictionary, Index = 2, Text = "+n" });
            list.Add(new Item { Category = Category.UrbanDictionary, Index = 3, Text = "urban example for: +n" });
            list.Add(new Item { Category = Category.UrbanDictionary, Index = 4, Text = "\x00029+n" });

            list.Add(new Item { Category = Category.Define, Index = 0, Text = "unable to find dictionary definition for: +n" });
            list.Add(new Item { Category = Category.Define, Index = 1, Text = "dictionary definition for: +n" });
            list.Add(new Item { Category = Category.Define, Index = 2, Text = "- +n" });

            list.Add(new Item { Category = Category.Trace, Index = 0, Text = "unable to find trace information for +n" });
            list.Add(new Item { Category = Category.Trace, Index = 1, Text = "trace results for +n" });
            list.Add(new Item { Category = Category.Trace, Index = 2, Text = "country: +n" });
            list.Add(new Item { Category = Category.Trace, Index = 3, Text = "region: +n" });
            list.Add(new Item { Category = Category.Trace, Index = 4, Text = "city: +n" });
            list.Add(new Item { Category = Category.Trace, Index = 5, Text = "local time: +n" });
            list.Add(new Item { Category = Category.Trace, Index = 6, Text = "end of trace results" });

            list.Add(new Item { Category = Category.BanStats, Index = 0, Text = "+n [+ip] banned by +a" });

            list.Add(new Item { Category = Category.Vspy, Index = 0, Text = "+n [+ip] has joined" });
            list.Add(new Item { Category = Category.Vspy, Index = 1, Text = "+n [+ip] has parted" });
            list.Add(new Item { Category = Category.Vspy, Index = 2, Text = "+n has moved to vroom +v" });

            list.Add(new Item { Category = Category.Whois, Index = 0, Text = "Name: +n" });
            list.Add(new Item { Category = Category.Whois, Index = 1, Text = "Originally: +n" });
            list.Add(new Item { Category = Category.Whois, Index = 2, Text = "External IP: +n" });
            list.Add(new Item { Category = Category.Whois, Index = 3, Text = "Local IP: +n" });
            list.Add(new Item { Category = Category.Whois, Index = 4, Text = "Data Port: +n" });
            list.Add(new Item { Category = Category.Whois, Index = 5, Text = "Version: +n" });
            list.Add(new Item { Category = Category.Whois, Index = 6, Text = "Vroom: +n" });
            list.Add(new Item { Category = Category.Whois, Index = 7, Text = "ID: +n" });
            list.Add(new Item { Category = Category.Whois, Index = 8, Text = "Linked: +n" });
            list.Add(new Item { Category = Category.Whois, Index = 9, Text = "Registered: +n" });

            list.Add(new Item { Category = Category.Stats, Index = 0, Text = "Stats for +n" });
            list.Add(new Item { Category = Category.Stats, Index = 1, Text = "Language: +n" });
            list.Add(new Item { Category = Category.Stats, Index = 2, Text = "Hashlink: +n" });
            list.Add(new Item { Category = Category.Stats, Index = 3, Text = "Uptime: +n" });
            list.Add(new Item { Category = Category.Stats, Index = 4, Text = "Bytes received: +n" });
            list.Add(new Item { Category = Category.Stats, Index = 5, Text = "Bytes sent: +n" });
            list.Add(new Item { Category = Category.Stats, Index = 6, Text = "Invalid logins: +n" });
            list.Add(new Item { Category = Category.Stats, Index = 7, Text = "Flooded users: +n" });
            list.Add(new Item { Category = Category.Stats, Index = 8, Text = "Rejected users: +n" });
            list.Add(new Item { Category = Category.Stats, Index = 9, Text = "Join count: +n" });
            list.Add(new Item { Category = Category.Stats, Index = 10, Text = "Part count: +n" });
            list.Add(new Item { Category = Category.Stats, Index = 11, Text = "User count: +n" });
            list.Add(new Item { Category = Category.Stats, Index = 12, Text = "Quarantined user count: +n" });
            list.Add(new Item { Category = Category.Stats, Index = 13, Text = "Peak user count: +n" });
            list.Add(new Item { Category = Category.Stats, Index = 14, Text = "Message count: +n" });
            list.Add(new Item { Category = Category.Stats, Index = 15, Text = "PM count: +n" });
            list.Add(new Item { Category = Category.Stats, Index = 16, Text = "Roomsearch size: +n" });

            list.Add(new Item { Category = Category.WhoWas, Index = 0, Text = "whowas: +n +ip +v +t" });
            list.Add(new Item { Category = Category.WhoWas, Index = 1, Text = "no results were found containing +n" });

            list.Add(new Item { Category = Category.BanSend, Index = 0, Text = "+n was rejected for being banned" });
            list.Add(new Item { Category = Category.BanSend, Index = 1, Text = "+n was rejected for name hijacking" });
            list.Add(new Item { Category = Category.BanSend, Index = 2, Text = "+n was rejected for exceeding the client limit" });
            list.Add(new Item { Category = Category.BanSend, Index = 3, Text = "+n was rejected for rejoining too quickly" });
            list.Add(new Item { Category = Category.BanSend, Index = 4, Text = "+n was rejected for unallowed gender" });
            list.Add(new Item { Category = Category.BanSend, Index = 5, Text = "+n was rejected for being too young" });
            list.Add(new Item { Category = Category.BanSend, Index = 6, Text = "+n was rejected for being range banned" });
            list.Add(new Item { Category = Category.BanSend, Index = 7, Text = "+n was rejected for being an anon" });
            list.Add(new Item { Category = Category.BanSend, Index = 8, Text = "+n was rejected for being not sharing any files" });
            list.Add(new Item { Category = Category.BanSend, Index = 9, Text = "+n was rejected by the join filter" });

            list.Add(new Item { Category = Category.Filter, Index = 0, Text = "Join filter added by +n - [trigger: +t] [filter type: +f]" });//40
            list.Add(new Item { Category = Category.Filter, Index = 1, Text = "Join filter [+t] removed by +n" });//41
            list.Add(new Item { Category = Category.Filter, Index = 2, Text = "filter list is empty" });
            list.Add(new Item { Category = Category.Filter, Index = 3, Text = "File filter added by +n - [trigger: +t] [filter type: +f]" });//42
            list.Add(new Item { Category = Category.Filter, Index = 4, Text = "File filter [+t] removed by +n" });//43
            list.Add(new Item { Category = Category.Filter, Index = 5, Text = "+n was killed for sharing [+f]" });//44
            list.Add(new Item { Category = Category.Filter, Index = 6, Text = "+n was banned for sharing [+f]" });//45
            list.Add(new Item { Category = Category.Filter, Index = 7, Text = "Word filter added by +n - [trigger: +t] [filter type: +f]" });//28
            list.Add(new Item { Category = Category.Filter, Index = 8, Text = "Word filter [+t] removed by +n" });//29
            list.Add(new Item { Category = Category.Filter, Index = 9, Text = "+n was muzzled for typing a filtered word" });//33
            list.Add(new Item { Category = Category.Filter, Index = 10, Text = "+n was killed for typing a filtered word" });//35
            list.Add(new Item { Category = Category.Filter, Index = 11, Text = "+n was banned for typing a filtered word" });//36
            list.Add(new Item { Category = Category.Filter, Index = 12, Text = "+n your text was censored for typing a filtered word" });//37
            list.Add(new Item { Category = Category.Filter, Index = 13, Text = "+n was moved for typing a filtered word" });//38
            list.Add(new Item { Category = Category.Filter, Index = 14, Text = "+n was redirected for typing a filtered word" });//39
            list.Add(new Item { Category = Category.Filter, Index = 15, Text = "+n was killed for typing a filtered word in PM" });//48
            list.Add(new Item { Category = Category.Filter, Index = 16, Text = "+n was banned for typing a filtered word in PM" });//49
            list.Add(new Item { Category = Category.Filter, Index = 17, Text = "+n was redirected for typing a filtered word in PM" });
            list.Add(new Item { Category = Category.Filter, Index = 18, Text = "Announce filters set to admin only by +n" });//75
            list.Add(new Item { Category = Category.Filter, Index = 19, Text = "Announce filters set to all users by +n" });//76
            list.Add(new Item { Category = Category.Filter, Index = 20, Text = "Line added to [filter: +t] by +n" });//30
            list.Add(new Item { Category = Category.Filter, Index = 21, Text = "Line removed from [filter: +t] by +n" });//31

            list.Add(new Item { Category = Category.Quarantined, Index = 0, Text = "+n was unquarantined by +a" });
            list.Add(new Item { Category = Category.Quarantined, Index = 1, Text = "there are currently no quarantined users" });

            CheckImport(announce_loaded);
            Load();

            if (announce_loaded)
                Server.Print("Loaded: " + Template.Text(Category.Credit, 0));
        }

        private static void CheckImport(bool announce_loaded)
        {
            String path = Path.Combine(Server.DataPath, "TEMPLATE IMPORTER", "template.txt");
            List<String> lines = new List<String>();

            try
            {
                if (File.Exists(path))
                {
                    String[] tmp = File.ReadAllLines(path);

                    foreach (String s in tmp)
                        if (s.StartsWith("string="))
                            lines.Add(s.Substring(7));
                }
            }
            catch { }

            int success = 0;

            Alter(ref lines, 97, Category.Credit, 0, ref success);
            Alter(ref lines, 90, Category.AdminAction, 0, ref success);
            Alter(ref lines, 91, Category.AdminAction, 1, ref success);
            Alter(ref lines, 89, Category.AdminAction, 2, ref success);
            Alter(ref lines, 92, Category.AdminAction, 3, ref success);
            Alter(ref lines, 93, Category.AdminAction, 4, ref success);
            Alter(ref lines, 77, Category.AdminAction, 5, ref success);
            Alter(ref lines, 78, Category.AdminAction, 6, ref success);
            Alter(ref lines, 79, Category.AdminAction, 7, ref success);
            Alter(ref lines, 80, Category.AdminAction, 8, ref success);
            Alter(ref lines, 81, Category.AdminAction, 9, ref success);
            Alter(ref lines, 82, Category.AdminAction, 10, ref success);
            Alter(ref lines, 83, Category.AdminAction, 11, ref success);
            Alter(ref lines, 84, Category.AdminAction, 12, ref success);
            Alter(ref lines, 85, Category.AdminAction, 13, ref success);
            Alter(ref lines, 86, Category.AdminAction, 14, ref success);
            Alter(ref lines, 87, Category.AdminAction, 15, ref success);
            Alter(ref lines, 88, Category.AdminAction, 16, ref success);
            Alter(ref lines, 94, Category.AdminAction, 17, ref success);
            Alter(ref lines, 95, Category.AdminAction, 18, ref success);
            Alter(ref lines, 96, Category.AdminAction, 19, ref success);
            Alter(ref lines, 115, Category.AdminAction, 20, ref success);
            Alter(ref lines, 17, Category.Notification, 4, ref success);
            Alter(ref lines, 132, Category.Notification, 5, ref success);
            Alter(ref lines, 103, Category.Notification, 6, ref success);
            Alter(ref lines, 128, Category.Notification, 7, ref success);
            Alter(ref lines, 1, Category.Rejected, 0, ref success);
            Alter(ref lines, 2, Category.Rejected, 1, ref success);
            Alter(ref lines, 3, Category.Rejected, 2, ref success);
            Alter(ref lines, 4, Category.Rejected, 3, ref success);
            Alter(ref lines, 5, Category.Rejected, 4, ref success);
            Alter(ref lines, 6, Category.Rejected, 5, ref success);
            Alter(ref lines, 7, Category.Rejected, 6, ref success);
            Alter(ref lines, 8, Category.Rejected, 7, ref success);
            Alter(ref lines, 114, Category.Rejected, 8, ref success);
            Alter(ref lines, 12, Category.AdminLogin, 0, ref success);
            Alter(ref lines, 13, Category.AdminLogin, 1, ref success);
            Alter(ref lines, 14, Category.AdminLogin, 2, ref success);
            Alter(ref lines, 16, Category.AdminLogin, 3, ref success);
            Alter(ref lines, 9, Category.AdminLogin, 4, ref success);
            Alter(ref lines, 10, Category.AdminLogin, 5, ref success);
            Alter(ref lines, 18, Category.Idle, 0, ref success);
            Alter(ref lines, 20, Category.PmBlocking, 0, ref success);
            Alter(ref lines, 21, Category.PmBlocking, 1, ref success);
            Alter(ref lines, 22, Category.PmBlocking, 2, ref success);
            Alter(ref lines, 26, Category.Messaging, 0, ref success);
            Alter(ref lines, 27, Category.Messaging, 1, ref success);
            Alter(ref lines, 152, Category.Captcha, 0, ref success);
            Alter(ref lines, 155, Category.Captcha, 3, ref success);
            Alter(ref lines, 50, Category.AdminList, 0, ref success);
            Alter(ref lines, 51, Category.AdminList, 1, ref success);
            Alter(ref lines, 52, Category.AdminList, 2, ref success);
            Alter(ref lines, 98, Category.Timeouts, 0, ref success);
            Alter(ref lines, 99, Category.Timeouts, 1, ref success);
            Alter(ref lines, 53, Category.EnableDisable, 0, ref success);
            Alter(ref lines, 54, Category.EnableDisable, 1, ref success);
            Alter(ref lines, 55, Category.EnableDisable, 2, ref success);
            Alter(ref lines, 56, Category.EnableDisable, 3, ref success);
            Alter(ref lines, 57, Category.EnableDisable, 4, ref success);
            Alter(ref lines, 58, Category.EnableDisable, 5, ref success);
            Alter(ref lines, 59, Category.EnableDisable, 6, ref success);
            Alter(ref lines, 60, Category.EnableDisable, 7, ref success);
            Alter(ref lines, 61, Category.EnableDisable, 8, ref success);
            Alter(ref lines, 62, Category.EnableDisable, 9, ref success);
            Alter(ref lines, 63, Category.EnableDisable, 10, ref success);
            Alter(ref lines, 64, Category.EnableDisable, 11, ref success);
            Alter(ref lines, 65, Category.EnableDisable, 12, ref success);
            Alter(ref lines, 66, Category.EnableDisable, 13, ref success);
            Alter(ref lines, 69, Category.EnableDisable, 14, ref success);
            Alter(ref lines, 70, Category.EnableDisable, 15, ref success);
            Alter(ref lines, 71, Category.EnableDisable, 16, ref success);
            Alter(ref lines, 72, Category.EnableDisable, 17, ref success);
            Alter(ref lines, 116, Category.EnableDisable, 18, ref success);
            Alter(ref lines, 117, Category.EnableDisable, 19, ref success);
            Alter(ref lines, 111, Category.EnableDisable, 20, ref success);
            Alter(ref lines, 112, Category.EnableDisable, 21, ref success);
            Alter(ref lines, 101, Category.EnableDisable, 22, ref success);
            Alter(ref lines, 102, Category.EnableDisable, 23, ref success);
            Alter(ref lines, 126, Category.EnableDisable, 24, ref success);
            Alter(ref lines, 127, Category.EnableDisable, 25, ref success);
            Alter(ref lines, 73, Category.EnableDisable, 30, ref success);
            Alter(ref lines, 74, Category.EnableDisable, 31, ref success);
            Alter(ref lines, 100, Category.Clock, 0, ref success);
            Alter(ref lines, 124, Category.Topics, 0, ref success);
            Alter(ref lines, 125, Category.Topics, 1, ref success);
            Alter(ref lines, 129, Category.Greetings, 0, ref success);
            Alter(ref lines, 130, Category.Greetings, 1, ref success);
            Alter(ref lines, 118, Category.Urls, 1, ref success);
            Alter(ref lines, 119, Category.Urls, 2, ref success);
            Alter(ref lines, 104, Category.RoomInfo, 0, ref success);
            Alter(ref lines, 105, Category.RoomInfo, 1, ref success);
            Alter(ref lines, 106, Category.RoomInfo, 2, ref success);
            Alter(ref lines, 107, Category.RoomInfo, 3, ref success);
            Alter(ref lines, 108, Category.RoomInfo, 4, ref success);
            Alter(ref lines, 110, Category.RoomInfo, 5, ref success);
            Alter(ref lines, 113, Category.RoomInfo, 6, ref success);
            Alter(ref lines, 40, Category.Filter, 0, ref success);
            Alter(ref lines, 41, Category.Filter, 1, ref success);
            Alter(ref lines, 42, Category.Filter, 3, ref success);
            Alter(ref lines, 43, Category.Filter, 4, ref success);
            Alter(ref lines, 44, Category.Filter, 5, ref success);
            Alter(ref lines, 45, Category.Filter, 6, ref success);
            Alter(ref lines, 28, Category.Filter, 7, ref success);
            Alter(ref lines, 29, Category.Filter, 8, ref success);
            Alter(ref lines, 33, Category.Filter, 9, ref success);
            Alter(ref lines, 35, Category.Filter, 10, ref success);
            Alter(ref lines, 36, Category.Filter, 11, ref success);
            Alter(ref lines, 37, Category.Filter, 12, ref success);
            Alter(ref lines, 38, Category.Filter, 13, ref success);
            Alter(ref lines, 39, Category.Filter, 14, ref success);
            Alter(ref lines, 48, Category.Filter, 15, ref success);
            Alter(ref lines, 49, Category.Filter, 16, ref success);
            Alter(ref lines, 75, Category.Filter, 18, ref success);
            Alter(ref lines, 76, Category.Filter, 19, ref success);
            Alter(ref lines, 30, Category.Filter, 20, ref success);
            Alter(ref lines, 31, Category.Filter, 21, ref success);

            if (success > 1)
            {
                try { File.Delete(path); }
                catch { }

                if (announce_loaded)
                    Server.Print("Successfully imported " + success + " template items");

                path = Path.Combine(Server.DataPath, "TEMPLATE IMPORTER", "LOG.txt");

                try { File.WriteAllText(path, "Successfully imported " + success + " template items at " + DateTime.Now); }
                catch { }

                Save();
            }
        }

        private static void Save()
        {
            List<String> lines = new List<String>();
            int current = -1;

            foreach (Item i in list)
            {
                String x = ((int)i.Category).ToString();

                if ((int)i.Category != current)
                {
                    current = (int)i.Category;
                    lines.Add(String.Empty);
                    lines.Add("[" + i.Category + "]");
                }

                while (x.Length < 3)
                    x = "0" + x;

                String y = i.Index.ToString();

                while (y.Length < 3)
                    y = "0" + y;

                lines.Add(x + "|" + y + "|" + i.Text);
            }

            lines.RemoveAt(0);
            String path = Path.Combine(Server.DataPath, "strings.txt");

            try { File.WriteAllLines(path, lines.ToArray(), Encoding.UTF8); }
            catch { }
        }

        private static void Load()
        {
            List<String> lines = new List<String>();
            String path = Path.Combine(Server.DataPath, "strings.txt");

            try { lines.AddRange(File.ReadAllLines(path, Encoding.UTF8)); }
            catch { }

            if (lines.Count > 0)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    String str = lines[i];
                    int id = str.IndexOf("|");

                    if (id > -1)
                    {
                        String x = str.Substring(0, id);
                        str = str.Substring(id + 1);
                        id = str.IndexOf("|");

                        if (id > -1)
                        {
                            String y = str.Substring(0, id);
                            str = str.Substring(id + 1);
                            int ix, iy;

                            if (int.TryParse(x, out ix))
                                if (int.TryParse(y, out iy))
                                {
                                    Item item = list.Find(q => (int)q.Category == ix && q.Index == iy);

                                    if (item != null)
                                        item.Text = str;
                                }
                        }
                    }
                }
            }

            Save();
        }

        private static void Alter(ref List<String> l, int src_index, Category c, int dest_index, ref int success)
        {
            int ud = (src_index - 1);
            
            if (l.Count > 0 && ud < l.Count)
            {
                String text = l[ud];
                Item i = list.Find(x => x.Category == c && x.Index == dest_index);

                if (i != null)
                {
                    i.Text = text;
                    success++;
                }
            }
        }


    }

    enum Category : int
    {
        Credit = 0,
        AdminAction = 1,
        Notification = 2,
        Rejected = 3,
        AdminLogin = 4,
        Registration = 5,
        Idle = 6,
        PmBlocking = 7,
        Messaging = 8,
        Captcha = 9,
        Linking = 10,
        AdminList = 11,
        RoomSearch = 12,
        Timeouts = 13,
        EnableDisable = 14,
        Clock = 15,
        Topics = 16,
        Greetings = 17,
        Urls = 18,
        RoomInfo = 19,
        Info = 20,
        Locate = 21,
        UrbanDictionary = 22,
        Define = 23,
        Trace = 24,
        BanStats = 25,
        Vspy = 26,
        Whois = 27,
        Stats = 28,
        WhoWas = 29,
        BanSend = 30,
        Filter = 31,
        Quarantined = 32
    }
}
