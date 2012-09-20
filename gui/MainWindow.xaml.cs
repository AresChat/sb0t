using System;
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

        public MainWindow()
        {
            SETTING_UP = true;
            this.InitializeComponent();
            this.server = new ServerCore();
            ServerCore.LogUpdate += this.LogUpdate;
        }

        private void LogUpdate(object sender, ServerLogEventArgs e)
        {
         /*   if (!String.IsNullOrEmpty(e.Message))
                MessageBox.Show(DateTime.Now + " log: " + e.Message);

            if (e.Error != null)
                MessageBox.Show(DateTime.Now + " error: " + e.Error.Message + "\n" + e.Error.StackTrace); */
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
                SETTING_UP = false;
            }
        }

        private void RunMode(bool running)
        {
            this.textBox1.IsEnabled = !running;
            this.textBox2.IsEnabled = !running;
            this.textBox3.IsEnabled = !running;
            this.checkBox1.IsEnabled = !running;
            this.checkBox2.IsEnabled = !running;
            this.checkBox3.IsEnabled = !running;
            this.checkBox4.IsEnabled = !running;
            this.checkBox5.IsEnabled = !running;
            this.checkBox6.IsEnabled = !running;
            this.checkBox7.IsEnabled = !running;
            this.checkBox8.IsEnabled = !running;
            this.checkBox9.IsEnabled = !running;
            this.checkBox10.IsEnabled = !running;
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
                        this.button1.Content = "Stop server";
                        this.statusLabel.Content = "Status: Server running.";
                        this.RunMode(true);
                    }
                    else MessageBox.Show("Unable to start server - please check your settings",
                        "sb0t", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

                case "Stop server":
                    this.server.Close();
                    this.button1.Content = "Start server";
                    this.statusLabel.Content = "Status: Server stopped.";
                    this.RunMode(false);
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to quit sb0t?",
                "sb0t", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (this.server.Running)
                    this.server.Close();
            }
            else e.Cancel = true;
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb.Name == "textBox1")
                Settings.Set("name", this.textBox1.Text);
            else if (tb.Name == "textBox2")
            {
                ushort u;

                if (ushort.TryParse(this.textBox3.Text, out u))
                    Settings.Set("port", u);
            }
            else if (tb.Name == "textBox3")
                Settings.Set("bot", this.textBox3.Text);
            else if (tb.Name == "textBox4" && textBox4.Text.Length > 0)
                Settings.Set("owner", this.textBox4.Text);
        }

        private void CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;

            if (cb.Name == "checkBox1")
                Settings.Set("logging", this.checkBox1.IsChecked);
            else if (cb.Name == "checkBox2")
                Settings.Set("emotes", this.checkBox2.IsChecked);
            else if (cb.Name == "checkBox3")
                Settings.Set("autostart", this.checkBox3.IsChecked);
            else if (cb.Name == "checkBox4")
            {
                Settings.Set("autoload", this.checkBox4.IsChecked);
            }
            else if (cb.Name == "checkBox5")
                Settings.Set("udp", this.checkBox5.IsChecked);
            else if (cb.Name == "checkBox6")
                Settings.Set("voice", this.checkBox6.IsChecked);
            else if (cb.Name == "checkBox7")
                Settings.Set("files", this.checkBox7.IsChecked);
            else if (cb.Name == "checkBox8")
                Settings.Set("captcha", this.checkBox8.IsChecked);
            else if (cb.Name == "checkBox9")
                Settings.Set("enabled", this.checkBox9.IsChecked, "web");
            else if (cb.Name == "checkBox10")
            {
                Settings.Set("commands", this.checkBox10.IsChecked);
                this.listView1.IsEnabled = (bool)this.checkBox10.IsChecked;
            }
        }
    }
}
