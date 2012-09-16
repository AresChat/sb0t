using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using core;

namespace gui
{
    partial class MainWindow
    {
        private void SetupValues()
        {
            String str;
            ushort u;
            
            //port
            u = Settings.Get<ushort>("port");

            if (u == 0)
                u = 54321;

            this.textBox2.Text = u.ToString();
            //name
            str = Settings.Get<String>("name");

            if (String.IsNullOrEmpty(str))
                str = "my chatroom";

            this.textBox1.Text = str;
            //bot
            str = Settings.Get<String>("bot");

            if (str.Length < 2)
                str = "sb0t";

            this.textBox3.Text = str;
            //chat logging - TO DO
            this.checkBox1.IsChecked = Settings.Get<bool>("logging");
            //custom emoticons
            this.checkBox2.IsChecked = Settings.Get<bool>("emotes");
            //auto start
            this.checkBox3.IsChecked = Settings.Get<bool>("autostart");

            if ((bool)this.checkBox3.IsChecked)
                this.ServerStartStop(null, null);

            //auto load - TO DO
            this.checkBox4.IsChecked = Settings.Get<bool>("autoload");
            //udp - TO DO
            this.checkBox5.IsChecked = Settings.Get<bool>("udp");
            //voice chat
            this.checkBox6.IsChecked = Settings.Get<bool>("voice");
            //files
            this.checkBox7.IsChecked = Settings.Get<bool>("files");
            //captcha
            this.checkBox8.IsChecked = Settings.Get<bool>("captcha");
            //ib0t
            this.checkBox9.IsChecked = Settings.Get<bool>("enabled", "web");
        }
    }
}
