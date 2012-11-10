﻿using System;
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

            list.Add(new Item { Category = Category.Notification, Index = 0, Text = "you are muzzled" });
            list.Add(new Item { Category = Category.Notification, Index = 1, Text = "ban list is empty" });
            list.Add(new Item { Category = Category.Notification, Index = 2, Text = "+n is ignored" });
            list.Add(new Item { Category = Category.Notification, Index = 3, Text = "+n is unignored" });
            list.Add(new Item { Category = Category.Notification, Index = 4, Text = "+n your CAPS have been auto-disabled" });//17
            list.Add(new Item { Category = Category.Notification, Index = 5, Text = "All ban lists have been automatically clear as scheduled" });//132

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
        Urls = 18
    }
}