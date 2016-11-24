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
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
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
            //chat logging
            this.checkBox1.IsChecked = Settings.Get<bool>("logging");
            //custom emoticons
            this.checkBox2.IsChecked = Settings.Get<bool>("can_room_scribble");
            //auto load - TO DO
            this.checkBox4.IsChecked = Settings.Get<bool>("start_min");
            //udp
            this.checkBox5.IsChecked = Settings.Get<bool>("udp");
            //voice chat
            this.checkBox6.IsChecked = Settings.Get<bool>("voice");
            //files
            this.checkBox7.IsChecked = Settings.Get<bool>("files");
            //captcha
            this.checkBox8.IsChecked = Settings.Get<bool>("captcha");
            //ib0t
            this.checkBox9.IsChecked = Settings.Get<bool>("enabled", "web");
            //strict
            this.checkBox22.IsChecked = Settings.Get<bool>("strict");
            // script_can_level
            this.checkBox30.IsChecked = Settings.Get<bool>("script_can_level");
            Settings.ScriptCanLevel(Settings.Get<bool>("script_can_level"));
            //title
            this.label1.Content = Settings.VERSION;
            //owner
            str = Settings.Get<String>("owner");

            if (String.IsNullOrEmpty(str))
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
            //hide ips
            this.checkBox18.IsChecked = Settings.Get<bool>("hide_ips");
            //preferred language
            this.comboBox3.SelectedIndex = this.AresLanguageToComboBoxLangauge();
            //local host
            this.checkBox19.IsChecked = Settings.Get<bool>("local_host");
            //udp address
            byte[] udp = Settings.Get<byte[]>("udp_address");
            //room search
            this.checkBox23.IsChecked = Settings.Get<bool>("roomsearch");

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
            //default avatar
            try
            {
                String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                  "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\Avatars\\default";
                if (File.Exists(path))
                {
                    RenderTargetBitmap resizedImage = this.FileToSizedImageSource(path, 90, 90);
                    this.image2.Source = resizedImage;
                    byte[] data = this.BitmapSourceToArray(resizedImage);
                    Avatars.UpdateDefaultAvatar(data);
                }
            }
            catch { }
            // fonts enabled
            this.checkBox25.IsChecked = Settings.Get<bool>("fonts_enabled");
            //extensions
            this.RefreshExtensions(null, null);
            core.Extensions.ExtensionManager.Setup();

            foreach (String ext in ExtAutorun.Items)
                this.LoadExtension(ext);
            //link identifier
            byte[] link_guid = Settings.Get<byte[]>("guid");

            if (link_guid == null)
            {
                link_guid = Guid.NewGuid().ToByteArray();
                Settings.Set("guid", link_guid);
            }
            //link mode
            this.comboBox4.SelectedIndex = Settings.Get<int>("link_mode");
            //trusted leaves
            core.LinkHub.TrustedLeavesManager.Init();

            foreach (core.LinkHub.TrustedLeafItem item in core.LinkHub.TrustedLeavesManager.Items)
                this.listBox3.Items.Add(item);

            //my link ident
            this.SetLinkIdent();
            //auto reconnect links
            this.checkBox20.IsChecked = Settings.Get<bool>("link_reconnect");
            //linked admin
            this.checkBox21.IsChecked = Settings.Get<bool>("link_admin");
            //auto start
            this.checkBox3.IsChecked = Settings.Get<bool>("autostart");

            this.CreateImporter1();
            this.CreateImporter2();

            if ((bool)this.checkBox3.IsChecked)
                this.ServerStartStop(null, null);
        }

        private void CreateImporter1()
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                              "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\TEMPLATE IMPORTER";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, "README.txt");

            if (!File.Exists(path))
            {
                List<String> lines = new List<String>();
                lines.Add("sb0t 5 - TEMPLATE IMPORTER");
                lines.Add(String.Empty);
                lines.Add("You can import template files from old version of sb0t.");
                lines.Add(String.Empty);
                lines.Add("- To do this, simply paste your old template into this folder.");
                lines.Add("- Then start sb0t.");
                lines.Add("- Or type #loadtemplate");
                lines.Add("- sb0t will then attempt to read your old template.txt file.");
                lines.Add("- A file called LOG.txt will be created.");
                lines.Add("- This log file will confirm the success of the import.");
                lines.Add("- sb0t will then delete the old template from this import folder.");
                lines.Add(String.Empty);
                lines.Add("The new template file for this version of sb0t is called strings.txt");
                lines.Add("and is located in the main data folder.");
                lines.Add(String.Empty);

                try { File.WriteAllLines(path, lines.ToArray()); }
                catch { }
            }
        }

        private void CreateImporter2()
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                              "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\FILTER IMPORTER";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, "README.txt");

            if (!File.Exists(path))
            {
                List<String> lines = new List<String>();
                lines.Add("sb0t 5 - FILTER IMPORTER");
                lines.Add(String.Empty);
                lines.Add("You can import filter files from old version of sb0t (4.xx).");
                lines.Add(String.Empty);
                lines.Add("- To do this, simply paste your old filter files (wordfilters.xml, joinfilters.xml,");
                lines.Add("  filefilters.xml, pmfilters.xml) into this folder then start sb0t.");
                lines.Add("- sb0t will then attempt to read your old filter files.");
                lines.Add("- A file called LOG.txt will be created.");
                lines.Add("- This log file will confirm the success of the import.");
                lines.Add("- sb0t will then delete the old filter files from this import folder.");
                lines.Add(String.Empty);
                lines.Add("The new filter files for this version of sb0t will be");
                lines.Add("located in the main data folder.");
                lines.Add(String.Empty);

                try { File.WriteAllLines(path, lines.ToArray()); }
                catch { }
            }
        }

        private void SetLinkIdent()
        {
            List<byte> list = new List<byte>();

            if (Settings.Get<byte[]>("guid") != null)
            {
                byte[] name = Encoding.UTF8.GetBytes(Settings.Get<String>("name"));
                list.Add((byte)name.Length);
                list.AddRange(name);
                list.AddRange(Settings.Get<byte[]>("guid"));
                list.Reverse();
                this.textBox7.Text = "sblnk://" + Convert.ToBase64String(list.ToArray());
            }
        }

        private void AddLink(String name, Guid guid)
        {
            core.LinkHub.TrustedLeafItem item = new core.LinkHub.TrustedLeafItem
            {
                Guid = guid,
                Name = name
            };

            if (!core.LinkHub.TrustedLeavesManager.AddItem(item))
                MessageBox.Show(GUILabels.IsSpanish ? GUILabels.spanish["mboxd"] : GUILabels.english["mboxd"],
                    "sb0t", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else
                this.listBox3.Items.Add(item);
        }

        private void LoadExtension(String name)
        {
            this.UnloadExtension(name);
            core.Extensions.ExtensionFrontEnd fe = core.Extensions.ExtensionManager.LoadPlugin(name);

            if (fe != null)
            {
                StackPanel stack = new StackPanel();
                stack.Orientation = Orientation.Horizontal;
                Image img = new Image();

                if (fe.Icon != null)
                    img.Source = fe.Icon;
                else
                    img.Source = new BitmapImage(new Uri("pack://application:,,/Images/plugin.png"));

                img.Height = 16;
                img.Width = 16;
                img.Margin = new Thickness(10, 0, 0, 0);
                TextBlock tb = new TextBlock();
                tb.Text = name;
                tb.Margin = new Thickness(2, 0, 0, 0);
                tb.VerticalAlignment = VerticalAlignment.Center;
                stack.Children.Add(img);
                stack.Children.Add(tb);
                stack.Tag = name;
                this.listBox2.Items.Add(stack);

                if (fe.GUI != null)
                {
                    fe.GUI.Tag = name;
                    gui_host.Children.Add(fe.GUI);
                    fe.GUI.Height = 343;
                    fe.GUI.Width = 416;
                    fe.GUI.Margin = new Thickness(166, 0, 0, 0);
                }

                this.listBox2.SelectedIndex = this.listBox2.Items.Count - 1;
                ExtAutorun.AddItem(name);
            }
            else MessageBox.Show(GUILabels.IsSpanish ? GUILabels.spanish["mboxe"] : GUILabels.english["mboxe"],
                "sb0t", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void UnloadExtension(String name)
        {
            for (int i = 0; i < this.gui_host.Children.Count; i++)
                if (this.gui_host.Children[i] != null)
                    if (this.gui_host.Children[i] is UserControl)
                        if (((UserControl)this.gui_host.Children[i]).Tag != null)
                            if (((UserControl)this.gui_host.Children[i]).Tag is String)
                                if (((UserControl)this.gui_host.Children[i]).Tag.ToString() == name)
                                {
                                    this.gui_host.Children.RemoveAt(i);
                                    break;
                                }

            for (int i=0;i<this.listBox2.Items.Count;i++)
                if (((StackPanel)this.listBox2.Items[i]).Tag.ToString() == name)
                {
                    this.listBox2.Items.RemoveAt(i);
                    break;
                }

            core.Extensions.ExtensionManager.UnloadPlugin(name);
            ExtAutorun.RemoveItem(name);
        }

        private RenderTargetBitmap FileToSizedImageSource(String file, int width, int height)
        {
            byte[] data = File.ReadAllBytes(file);
            return this.FileToSizedImageSource(data, width, height);
        }

        private RenderTargetBitmap FileToSizedImageSource(byte[] data, int width, int height)
        {
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
