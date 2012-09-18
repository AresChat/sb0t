using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using iconnect;

namespace core
{
    class PrivateMsg : IPrivateMsg
    {
        internal String Text { get; private set; }

        public PrivateMsg(String text)
        {
            this.Text = text;
        }

        public bool Contains(String text)
        {
            return this.Text.ToUpper().Contains(text.ToUpper());
        }

        public void Replace(String x, String y)
        {
            this.Text = Regex.Replace(this.Text, Regex.Escape(x), y, RegexOptions.IgnoreCase);
        }

        public void Remove(String text)
        {
            this.Text = Regex.Replace(this.Text, Regex.Escape(text), String.Empty, RegexOptions.IgnoreCase);
        }
    }
}
