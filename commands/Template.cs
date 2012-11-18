using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static void Load()
        {
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

            list.Add(new Item { Category = Category.Linking, Index = 0, Text = "\x000314--- An linking error occurred [+e]" });
            list.Add(new Item { Category = Category.Linking, Index = 1, Text = "\x000314--- Connection to Link Hub [+n] established :)" });
            list.Add(new Item { Category = Category.Linking, Index = 2, Text = "\x000314--- Connection to Link Hub was lost." });
            list.Add(new Item { Category = Category.Linking, Index = 3, Text = "\x000314--- A chatroom [+n] has joined" });
            list.Add(new Item { Category = Category.Linking, Index = 4, Text = "\x000314--- A chatroom [+n] has parted" });
            list.Add(new Item { Category = Category.Linking, Index = 5, Text = "\x000314--- Connecting to hub [+n], please wait..." });
            list.Add(new Item { Category = Category.Linking, Index = 6, Text = "\x000314--- Link session has now been terminated" });
            list.Add(new Item { Category = Category.Linking, Index = 7, Text = "\x000314--- The chatroom [+n] does not allow linked admins" });
            list.Add(new Item { Category = Category.Linking, Index = 8, Text = "\x000314--- Reconnection will be attempted in 30 seconds..." });
            list.Add(new Item { Category = Category.Linking, Index = 9, Text = "\x000314--- This chatroom is currently unlinked" });
            list.Add(new Item { Category = Category.Linking, Index = 10, Text = "\x000314--- Name: +n" });
            list.Add(new Item { Category = Category.Linking, Index = 11, Text = "\x000314--- Hashlink: \\\\+h" });

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
            list.Add(new Item { Category = Category.Define, Index = 1, Text = "urban definition for: +n" });
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
        BanSend = 30
    }
}
