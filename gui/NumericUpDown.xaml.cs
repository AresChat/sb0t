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

namespace gui
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        public event EventHandler<NumericValueChanged> ValueChanged;

        public NumericUpDown()
        {
            this.InitializeComponent();
            this.Value = 0;
        }

        private void button1_Click(object sender, RoutedEventArgs e) // up
        {
            if (this.Value < this.MaxValue)
            {
                this.Value++;

                if (this.ValueChanged != null)
                    this.ValueChanged(this, new NumericValueChanged(true));
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e) // down
        {
            if (this.Value > this.MinValue)
            {
                this.Value--;

                if (this.ValueChanged != null)
                    this.ValueChanged(this, new NumericValueChanged(false));
            }
        }

        private int _value = 0;
        private int _max = 10;
        private int _min = 0;

        public int MaxValue
        {
            get { return this._max; }
            set
            {
                if (this.Value > value)
                    this.Value = value;

                this._max = value;
            }
        }

        public int MinValue
        {
            get { return this._min; }
            set
            {
                if (this.Value < value)
                    this.Value = value;

                this._min = value;
            }
        }

        public int Value
        {
            get { return this._value; }
            set
            {
                this._value = value;
                this.textBox1.Text = value.ToString();
            }
        }
    }

    public class NumericValueChanged : EventArgs
    {
        public bool Up { get; private set; }

        public NumericValueChanged(bool up)
        {
            this.Up = up;
        }
    }
}
