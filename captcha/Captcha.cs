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

namespace captcha
{
    public class Captcha
    {
        private static List<String> words;
        private static Random rnd;

        public static void Initialize()
        {
            words = new List<String>(Resource1._4words.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries));
            words.RemoveAll(x => x.Length != 4);
            rnd = new Random();
        }

        public static CaptchaItem Create()
        {
            CaptchaItem item = new CaptchaItem();
            item.Word = words[(int)Math.Floor(rnd.NextDouble() * words.Count)];
            item.Lines = new String[5];
            char[] letters = item.Word.ToCharArray();

            for (int i = 0; i < 5; i++)
            {
                item.Lines[i] = "\x000500   ";

                foreach (char l in letters)
                    item.Lines[i] += Letters.GetCaptcha(l, i) + "   ";
            }

            return item;
        }
    }

    public class CaptchaItem
    {
        public String Word { get; set; }
        public String[] Lines { get; set; }
    }
}
