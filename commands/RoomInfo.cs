using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace commands
{
    class RoomInfo
    {
        private static uint last = 0;

        private static void Show()
        {
            Server.Print(Template.Text(Category.RoomInfo, 0));
            Server.Print(String.Empty);
            int counter = 0;
            Server.Users.Ares(x => { if (x.Level == ILevel.Host) counter++; });
            Server.Users.Web(x => { if (x.Level == ILevel.Host) counter++; });
            Server.Print(Template.Text(Category.RoomInfo, 1).Replace("+n", counter.ToString()));
            counter = 1;
            Server.Users.Ares(x => { if (!x.Quarantined) counter++; });
            Server.Users.Web(x => { if (!x.Quarantined) counter++; });
            Server.Print(Template.Text(Category.RoomInfo, 2).Replace("+n", counter.ToString()));
            counter = 0;
            Server.Users.Ares(x => { if (x.Level > ILevel.Regular) counter++; });
            Server.Users.Web(x => { if (x.Level > ILevel.Regular) counter++; });
            Server.Print(Template.Text(Category.RoomInfo, 3).Replace("+n", counter.ToString()));
            Server.Print(Template.Text(Category.RoomInfo, 4).Replace("+n", Helpers.GetUptime));
            String status = Settings.Status;

            if (String.IsNullOrEmpty(status))
                status = String.Empty;

            Server.Print(Template.Text(Category.RoomInfo, 5).Replace("+n", status));
            Server.Print(String.Empty);
            Server.Print(Template.Text(Category.RoomInfo, 0));
        }

        public static void ForceUpdate()
        {
            last = 1201;
        }

        public static void Tick(uint time)
        {
            if (time > (last + 1200)) // 20 minutes
            {
                last = time;

                if (Settings.RoomInfo)
                    Show();
            }
        }
    }
}
