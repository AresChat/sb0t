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
