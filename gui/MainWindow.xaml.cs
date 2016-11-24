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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using core;

namespace gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool SETTING_UP { get; set; }

        private bool do_once = true;
        private ServerCore server { get; set; }

        private OpenFileDialog fd = new OpenFileDialog();
        private System.Windows.Forms.NotifyIcon notify;
        private bool _hidden = false;

        private static Mutex mutex;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public MainWindow()
        {
            bool first;
            mutex = new Mutex(true, AppDomain.CurrentDomain.FriendlyName, out first);

            if (!first)
            {
                Process p = Process.GetCurrentProcess();
                
                var pl = Process.GetProcesses().Where(x =>
                    x.ProcessName == p.ProcessName &&
                    x.MainWindowHandle != p.MainWindowHandle);

                MessageBox.Show("There is already a instance of sb0t running.  To have additional instances, each must have a filename different from "
                    + AppDomain.CurrentDomain.FriendlyName, "sb0t", MessageBoxButton.OK, MessageBoxImage.Error);

                if (pl.Count() > 0)
                    SetForegroundWindow(pl.ToArray()[0].MainWindowHandle);

                Environment.Exit(-1);
            }

            SETTING_UP = true;
            this.InitializeComponent();
            this.server = new ServerCore();
            ServerCore.LogUpdate += this.LogUpdate;
            this.notify = new System.Windows.Forms.NotifyIcon();
            this.notify.Text = "sb0t";
            this.notify.Icon = Resource1.mains;
            this.notify.Click += new EventHandler(this.NotifyIconClicked);
            this.notify.Visible = true;
            GUILabels.Setup(this);
        }

        private void NotifyIconClicked(object sender, EventArgs e)
        {
            if (this._hidden)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
                this._hidden = false;
            }
        }

        private void LogUpdate(object sender, ServerLogEventArgs e)
        {
            String str = "";

            if (!String.IsNullOrEmpty(e.Message))
                str = e.Message;

            if (e.Error != null)
                str += "\r\n" + e.Error.Message + "\r\n" + e.Error.StackTrace;

            try
            {
                String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    + "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName;

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                path += "\\serverlog.txt";

                using (StreamWriter writer = File.Exists(path) ? File.AppendText(path) : File.CreateText(path))
                    writer.WriteLine(DateTime.UtcNow + " " + str);
            }
            catch { }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.do_once)
            {
                this.do_once = false;
                Aero.MakeWindowGlass(this);

                if (!Aero.CanAero)
                {
                    this.Background = new SolidColorBrush(Colors.Gainsboro);
                    this.tabControl1.Margin = new Thickness(5, 5, 5, 5);
                }

                this.SetupValues();

                if ((bool)this.checkBox4.IsChecked)
                    this.WindowState = WindowState.Minimized;

                SETTING_UP = false;

                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.LogUnhandledException);
            }
        }

        private void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogUpdate(sender, new ServerLogEventArgs { Error = (Exception)e.ExceptionObject });
        }

        private void RunMode(bool running)
        {
            this.textBox1.IsEnabled = !running;
            this.textBox2.IsEnabled = !running;
            this.textBox3.IsEnabled = !running;
            this.textBox5.IsEnabled = !running;
            this.textBox6.IsEnabled = !running;
            this.checkBox1.IsEnabled = !running;
            this.checkBox25.IsEnabled = !running;
            this.checkBox2.IsEnabled = !running;
            this.checkBox3.IsEnabled = !running;
            this.checkBox4.IsEnabled = !running;
            this.checkBox5.IsEnabled = !running;
            this.checkBox6.IsEnabled = !running;
            this.checkBox7.IsEnabled = !running;
            this.checkBox8.IsEnabled = !running;
            this.checkBox9.IsEnabled = !running;
            this.checkBox10.IsEnabled = !running;
            this.checkBox11.IsEnabled = !running;
            this.button3.IsEnabled = running;
            this.button4.IsEnabled = running;
            this.comboBox2.IsEnabled = running ? false : (bool)this.checkBox8.IsChecked;
            this.comboBox3.IsEnabled = !running;
            this.comboBox4.IsEnabled = !running;
            this.checkBox20.IsEnabled = !running;
            this.checkBox22.IsEnabled = !running;
            this.checkBox23.IsEnabled = !running;
            this.checkBox30.IsEnabled = !running;
            this.checkBox18.IsEnabled = !running;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            Aero.HideTitleInfo(this, Aero.WTNCA.NODRAWICON | Aero.WTNCA.NODRAWCAPTION);
        }

        private void ServerStartStop(object sender, RoutedEventArgs e)
        {
            switch ((String)this.button1.Content)
            {
                case "Start server":
                    if (this.server.Open())
                    {
                        this.button1.Content = GUILabels.english["button1b"];
                        this.statusLabel.Content = GUILabels.english["statuslabelb"];
                        this.RunMode(true);
                    }
                    else MessageBox.Show(GUILabels.english["mboxa"],
                        "sb0t", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

                case "Stop server":
                    this.server.Close();
                    this.button1.Content = GUILabels.english["button1a"];
                    this.statusLabel.Content = GUILabels.english["statuslabela"];
                    this.RunMode(false);
                    break;

                case "Comenzar el servidor":
                    if (this.server.Open())
                    {
                        this.button1.Content = GUILabels.spanish["button1b"];
                        this.statusLabel.Content = GUILabels.spanish["statuslabelb"];
                        this.RunMode(true);
                    }
                    else MessageBox.Show(GUILabels.spanish["mboxa"],
                        "sb0t", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

                case "Detener el servidor":
                    this.server.Close();
                    this.button1.Content = GUILabels.spanish["button1a"];
                    this.statusLabel.Content = GUILabels.spanish["statuslabela"];
                    this.RunMode(false);
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(GUILabels.IsSpanish ? GUILabels.spanish["mboxb"] : GUILabels.english["mboxb"],
                "sb0t", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (this.server.Running)
                    this.server.Close();

                this.notify.Dispose();
                Environment.Exit(-1);
            }
            else e.Cancel = true;
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb.Name == "textBox1")
            {
                Settings.Set("name", this.textBox1.Text);
                this.SetLinkIdent();
            }
            else if (tb.Name == "textBox2")
            {
                ushort u;

                if (ushort.TryParse(this.textBox2.Text, out u))
                    Settings.Set("port", u);
            }
            else if (tb.Name == "textBox3")
                Settings.Set("bot", this.textBox3.Text);
            else if (tb.Name == "textBox4" && textBox4.Text.Length > 0)
                Settings.Set("owner", this.textBox4.Text);
            else if (tb.Name == "textBox5")
            {
                Uri uri;

                if (Uri.TryCreate(this.textBox5.Text, UriKind.Absolute, out uri))
                    Settings.Set("url", this.textBox5.Text, "web");
            }
            else if (tb.Name == "textBox6")
            {
                IPAddress ip;

                if (IPAddress.TryParse(this.textBox6.Text, out ip))
                    Settings.Set("udp_address", ip.GetAddressBytes());
            }
        }

        private void CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;

            if (cb.Name == "checkBox1")
                Settings.Set("logging", this.checkBox1.IsChecked);
            else if (cb.Name == "checkBox2")
                Settings.Set("can_room_scribble", this.checkBox2.IsChecked);
            else if (cb.Name == "checkBox3")
                Settings.Set("autostart", this.checkBox3.IsChecked);
            else if (cb.Name == "checkBox4")
            {
                Settings.Set("start_min", this.checkBox4.IsChecked);
            }
            else if (cb.Name == "checkBox5")
                Settings.Set("udp", this.checkBox5.IsChecked);
            else if (cb.Name == "checkBox6")
                Settings.Set("voice", this.checkBox6.IsChecked);
            else if (cb.Name == "checkBox7")
                Settings.Set("files", this.checkBox7.IsChecked);
            else if (cb.Name == "checkBox8")
            {
                Settings.Set("captcha", this.checkBox8.IsChecked);
                this.comboBox2.IsEnabled = (bool)this.checkBox8.IsChecked;
            }
            else if (cb.Name == "checkBox9")
                Settings.Set("enabled", this.checkBox9.IsChecked, "web");
            else if (cb.Name == "checkBox10")
            {
                Settings.Set("commands", this.checkBox10.IsChecked);
                this.listView1.IsEnabled = (bool)this.checkBox10.IsChecked;
            }
            else if (cb.Name == "checkBox11")
            {
                Settings.Set("scripting", this.checkBox11.IsChecked);
                this.checkBox12.IsEnabled = (bool)this.checkBox11.IsChecked;
                this.comboBox1.IsEnabled = (bool)this.checkBox11.IsChecked;
            }
            else if (cb.Name == "checkBox12")
                Settings.Set("inroom_scripting", this.checkBox12.IsChecked);
            else if (cb.Name == "checkBox13")
                Settings.Set("auto_ban_clear_enabled", this.checkBox13.IsChecked);
            else if (cb.Name == "checkBox17")
            {
                Settings.Set("age_restrict", this.checkBox17.IsChecked);
                this.numericUpDown2.IsEnabled = (bool)this.checkBox17.IsChecked;
            }
            else if (cb.Name == "checkBox15")
                Settings.Set("reject_male", this.checkBox15.IsChecked);
            else if (cb.Name == "checkBox14")
                Settings.Set("reject_female", this.checkBox14.IsChecked);
            else if (cb.Name == "checkBox16")
                Settings.Set("reject_unknown", this.checkBox16.IsChecked);
            else if (cb.Name == "checkBox18")
                Settings.Set("hide_ips", this.checkBox18.IsChecked);
            else if (cb.Name == "checkBox19")
                Settings.Set("local_host", this.checkBox19.IsChecked);
            else if (cb.Name == "checkBox20")
                Settings.Set("link_reconnect", this.checkBox20.IsChecked);
            else if (cb.Name == "checkBox21")
                Settings.Set("link_admin", this.checkBox21.IsChecked);
            else if (cb.Name == "checkBox22")
                Settings.Set("strict", this.checkBox22.IsChecked);
            else if (cb.Name == "checkBox23")
                Settings.Set("roomsearch", this.checkBox23.IsChecked);
            else if (cb.Name == "checkBox30")
            {
                Settings.Set("script_can_level", this.checkBox30.IsChecked);
                Settings.ScriptCanLevel(Settings.Get<bool>("script_can_level"));
            }
            else if (cb.Name == "checkBox25")
            {
                Settings.Set("fonts_enabled", this.checkBox25.IsChecked);
            }
        }

        private void ScriptLevelSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.Set("inroom_level", (byte)this.comboBox1.SelectedIndex + 1);
        }

        private void numericUpDown1_ValueChanged(object sender, NumericValueChanged e)
        {
            Settings.Set("auto_ban_clear_interval", this.numericUpDown1.Value);
        }

        private void CaptchaModeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.Set("captcha_mode", this.comboBox2.SelectedIndex);
        }

        private void button2_Click(object sender, RoutedEventArgs e) // open data folder
        {
            try
            {
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                    "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName);
            }
            catch { }
        }

        private void button3_Click(object sender, RoutedEventArgs e) // ares join
        {
            try
            {
                String hashlink = core.Hashlink.EncodeHashlink(new core.Room
                {
                    IP = IPAddress.Loopback,
                    Name = textBox1.Text,
                    Port = ushort.Parse(textBox2.Text)
                });

                if (hashlink != null)
                    Process.Start("arlnk://" + hashlink);
            }
            catch { }
        }

        private void button4_Click(object sender, RoutedEventArgs e) // cbot join
        {
            try
            {
                String hashlink = core.Hashlink.EncodeHashlink(new core.Room
                {
                    IP = IPAddress.Loopback,
                    Name = textBox1.Text,
                    Port = ushort.Parse(textBox2.Text)
                });

                if (hashlink != null)
                    Process.Start("cb0t://" + hashlink);
            }
            catch { }
        }

        private void numericUpDown2_ValueChanged(object sender, NumericValueChanged e)
        {
            Settings.Set("age_restrict_value", this.numericUpDown2.Value);
        }

        private void comboBox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.Set("language", this.ComboBoxLanguageToAresLanguage(this.comboBox3.SelectedIndex));
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            this.fd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            this.fd.Filter = "Image files (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png";
            this.fd.Multiselect = false;
            this.fd.FileName = String.Empty;

            if ((bool)this.fd.ShowDialog())
            {
                try
                {
                    RenderTargetBitmap resizedImage = this.FileToSizedImageSource(fd.FileName, 90, 90);
                    this.image1.Source = resizedImage;
                    byte[] data = this.BitmapSourceToArray(resizedImage);
                    String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                      "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\Avatars";

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    File.WriteAllBytes(path + "\\server", data);
                    Avatars.UpdateServerAvatar(data);
                }
                catch { }
            }
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            this.fd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            this.fd.Filter = "Image files (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png";
            this.fd.Multiselect = false;
            this.fd.FileName = String.Empty;

            if ((bool)this.fd.ShowDialog())
            {
                try
                {
                    RenderTargetBitmap resizedImage = this.FileToSizedImageSource(fd.FileName, 90, 90);
                    this.image2.Source = resizedImage;
                    byte[] data = this.BitmapSourceToArray(resizedImage);
                    String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                      "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\Avatars";

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    File.WriteAllBytes(path + "\\default", data);
                    Avatars.UpdateDefaultAvatar(data);
                }
                catch { }
            }
        }

        private void ExtensionGUISelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object obj = this.listBox2.SelectedItem;

            if (obj != null)
            {
                StackPanel item = (StackPanel)obj;

                if (item != null)
                {
                    String extension_name = item.Tag.ToString();

                    for (int i = 0; i < this.gui_host.Children.Count; i++)
                        if (this.gui_host.Children[i] != null)
                            if (this.gui_host.Children[i] is UserControl)
                            {
                                if (((UserControl)this.gui_host.Children[i]).Tag != null)
                                    if (((UserControl)this.gui_host.Children[i]).Tag is String)
                                        if (((UserControl)this.gui_host.Children[i]).Tag.ToString() == extension_name)
                                        {
                                            this.gui_host.Children[i].Visibility = Visibility.Visible;
                                            continue;
                                        }

                                this.gui_host.Children[i].Visibility = Visibility.Hidden;
                            }
                }
            }
        }

        private void UninstallExtension(object sender, RoutedEventArgs e)
        {
            object obj = this.listBox2.SelectedItem;

            if (obj != null)
            {
                StackPanel item = (StackPanel)obj;

                if (item != null)
                {
                    String extension_name = item.Tag.ToString();
                    this.UnloadExtension(extension_name);
                }
            }
        }

        private void RefreshExtensions(object sender, MouseButtonEventArgs e)
        {
            this.listBox1.Items.Clear();

            String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
               "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\Extensions";

            if (Directory.Exists(path))
            {
                DirectoryInfo root = new DirectoryInfo(path);

                foreach (DirectoryInfo folder in root.GetDirectories())
                    foreach (FileInfo file in folder.GetFiles())
                        if (file.Name == "extension.dll")
                        {
                            this.listBox1.Items.Add(folder);
                            break;
                        }
            }
        }

        private void PluginInstallDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (this.listBox1.SelectedIndex > -1)
            {
                String extension_name = this.listBox1.SelectedItem.ToString();
                this.LoadExtension(extension_name);
            }
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            String text = this.textBox8.Text;
            this.textBox8.Text = String.Empty;

            try
            {
                if (text.StartsWith("sblnk://"))
                {
                    text = text.Substring(8);
                    byte[] buffer = Convert.FromBase64String(text);
                    Array.Reverse(buffer);
                    byte len = buffer[0];
                    String name = Encoding.UTF8.GetString(buffer, 1, len);
                    Guid guid = new Guid(buffer.Skip(1 + len).ToArray());
                    this.AddLink(name, guid);
                }
                else throw new Exception();
            }
            catch
            {
                MessageBox.Show(GUILabels.IsSpanish ? GUILabels.spanish["mboxc"] : GUILabels.english["mboxc"],
                    "sb0t", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void comboBox4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.Set("link_mode", this.comboBox4.SelectedIndex);
        }

        private void RemoveTrustedLeaf(object sender, RoutedEventArgs e)
        {
            int i = this.listBox3.SelectedIndex;

            if (i > -1)
            {
                core.LinkHub.TrustedLeafItem item = (core.LinkHub.TrustedLeafItem)this.listBox3.Items[i];
                core.LinkHub.TrustedLeavesManager.RemoveItem(item);
                this.listBox3.Items.RemoveAt(i);
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            this.notify.Visible = false;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Visibility = Visibility.Hidden;
                this._hidden = true;
            }
        }

        private void SpanishClicked(object sender, MouseButtonEventArgs e)
        {
            GUILabels.SetSpanish(this);
        }

        private void EnglishClicked(object sender, MouseButtonEventArgs e)
        {
            GUILabels.SetEnglish(this);
        }
    }
}
