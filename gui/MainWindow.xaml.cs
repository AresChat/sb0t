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

namespace gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool do_once = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
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
            }
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            Aero.HideTitleInfo(this, Aero.WTNCA.NODRAWICON | Aero.WTNCA.NODRAWCAPTION);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
