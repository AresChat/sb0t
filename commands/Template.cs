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

            list.Add(new Item { Category = Category.Notification, Index = 0, Text = "you are muzzled" });
            list.Add(new Item { Category = Category.Notification, Index = 1, Text = "ban list is empty" });

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
        }
    }

    enum Category : int
    {
        Credit = 0,
        AdminAction = 1,
        Notification = 2,
        Rejected = 3,
    }
}
