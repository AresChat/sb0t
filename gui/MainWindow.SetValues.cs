using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
            {
                u = 54321;
                Settings.Set("port", u);
            }

            this.textBox2.Text = u.ToString();
            //name
            str = Settings.Get<String>("name");

            if (String.IsNullOrEmpty(str))
            {
                str = "my chatroom";
                Settings.Set("name", str);
            }

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
            //enable age restriction
            this.checkBox17.IsChecked = Settings.Get<bool>("age_restrict");
            //age restriction
            int age = Settings.Get<int>("age_restrict_value");

            if (age == 0)
            {
                age = 18;
                Settings.Set("age_restrict_value", age);
            }

            this.numericUpDown2.Value = age;
            this.numericUpDown2.IsEnabled = (bool)this.checkBox17.IsChecked;
            //male users
            this.checkBox15.IsChecked = Settings.Get<bool>("reject_male");
            //female users
            this.checkBox14.IsChecked = Settings.Get<bool>("reject_female");
            //unknown users
            this.checkBox16.IsChecked = Settings.Get<bool>("reject_unknown");
            //ib0t push
            String ib = Settings.Get<String>("url", "web");

            if (String.IsNullOrEmpty(ib))
            {
                ib = "http://chatrooms.marsproject.net/ibot.aspx";
                Settings.Set("url", "http://chatrooms.marsproject.net/ibot.aspx", "web");
            }

            this.textBox5.Text = ib;
            //full scribble
            this.checkBox18.IsChecked = Settings.Get<bool>("full_scribble");
            //preferred language
            this.comboBox3.SelectedIndex = this.AresLanguageToComboBoxLangauge();
            //local host - TO DO
            this.checkBox19.IsChecked = Settings.Get<bool>("local_host");
            //udp address - TO DO
            byte[] udp = Settings.Get<byte[]>("udp_address");

            if (udp == null)
            {
                udp = IPAddress.Any.GetAddressBytes();
                Settings.Set("udp_address", udp);
            }

            this.textBox6.Text = new IPAddress(udp).ToString();
            //server avatar
            try
            {
                String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                  "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\Avatars\\server";

                if (File.Exists(path))
                {
                    RenderTargetBitmap resizedImage = this.FileToSizedImageSource(path, 90, 90);
                    this.image1.Source = resizedImage;
                    byte[] data = this.BitmapSourceToArray(resizedImage);
                    Avatars.UpdateServerAvatar(data);
                }
            }
            catch { }
            //auto start
            this.checkBox3.IsChecked = Settings.Get<bool>("autostart");

            if ((bool)this.checkBox3.IsChecked)
                this.ServerStartStop(null, null);
        }

        private RenderTargetBitmap FileToSizedImageSource(String file, int width, int height)
        {
            byte[] data = File.ReadAllBytes(file);
            RenderTargetBitmap resizedImage = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);

            using (MemoryStream ms = new MemoryStream(data))
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = ms;
                img.EndInit();
                Rect rect = new Rect(0, 0, width, height);
                DrawingVisual drawingVisual = new DrawingVisual();

                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                    drawingContext.DrawImage(img, rect);

                resizedImage.Render(drawingVisual);
            }

            return resizedImage;
        }

        private byte[] BitmapSourceToArray(BitmapSource img)
        {
            byte[] result;

            using (MemoryStream ms = new MemoryStream())
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(img));
                encoder.Save(ms);
                result = ms.ToArray();
            }

            return result;
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

        private byte ComboBoxLanguageToAresLanguage(int index)
        {
            switch (index)
            {
                case 0: return 11;
                case 1: return 12;
                case 2: return 14;
                case 3: return 15;
                case 4: return 16;
                case 5: return 10;
                case 6: return 27;
                case 7: return 28;
                case 8: return 29;
                case 9: return 30;
                case 10: return 17;
                case 11: return 19;
                case 12: return 20;
                case 13: return 21;
                case 14: return 31;
                case 15: return 22;
                case 16: return 23;
                case 17: return 25;
                case 18: return 26;
                default: return 10;
            }
        }

        private int AresLanguageToComboBoxLangauge()
        {
            byte lang = Settings.Get<byte>("language");

            if (lang == 0)
            {
                Settings.Set("language", (byte)10);
                lang = 10;
            }

            switch (lang)
            {
                case 11: return 0;
                case 12: return 1;
                case 14: return 2;
                case 15: return 3;
                case 16: return 4;
                case 10: return 5;
                case 27: return 6;
                case 28: return 7;
                case 29: return 8;
                case 30: return 9;
                case 17: return 10;
                case 19: return 11;
                case 20: return 12;
                case 21: return 13;
                case 31: return 14;
                case 22: return 15;
                case 23: return 16;
                case 25: return 17;
                case 26: return 18;
                default: return 5;
            }
        }
    }
}
