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
            //title
            this.label1.Content = Settings.VERSION;
            //owner
            str = Settings.Get<String>("owner");

            if (str.Length == 0)
                str = this.RandomPassword;

            this.textBox4.Text = str;
            //command levels
            this.AdminCommandSetup();
            //built in admin commands
            this.checkBox10.IsChecked = Settings.Get<bool>("commands");
            this.listView1.IsEnabled = (bool)this.checkBox10.IsChecked;
            //scripting enabled
            this.checkBox11.IsChecked = Settings.Get<bool>("scripting");
            //in room scripting
            this.checkBox12.IsChecked = Settings.Get<bool>("inroom_scripting");
            this.checkBox12.IsEnabled = (bool)this.checkBox11.IsChecked;
            //in room level
            this.comboBox1.IsEnabled = (bool)this.checkBox11.IsChecked;
            byte b = Settings.Get<byte>("inroom_level");
            this.comboBox1.SelectedIndex = b == 0 ? 3 : (b - 1);
            //auto ban clear interval
            int autobanclear = Settings.Get<int>("auto_ban_clear_interval");

            if (autobanclear == 0)
            {
                autobanclear = 1;
                Settings.Set("auto_ban_clear_interval", autobanclear);
            }

            this.numericUpDown1.Value = autobanclear;
            //auto ban clear enabled
            this.checkBox13.IsChecked = Settings.Get<bool>("auto_ban_clear_enabled");
            //captcha mode
            this.comboBox2.SelectedIndex = Settings.Get<int>("captcha_mode");
            this.comboBox2.IsEnabled = (bool)this.checkBox8.IsChecked;
        }

        private String RandomPassword
        {
            get
            {
                String result = String.Empty;
                Random rnd = new Random();
                char[] letters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

                for (int i = 0; i < 20; i++)
                    result += letters[(int)Math.Floor(rnd.NextDouble() * letters.Length)];

                return result;
            }
        }
    }
}
