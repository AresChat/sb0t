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

namespace core
{
    static class Prototypes
    {
        private static Random rnd = new Random();

        public static void ForEachWhere<T>(this List<T> list, Action<T> @foreach, Predicate<T> @where)
        {
            for (int i = 0; i < list.Count; i++)
                if (@where(list[i]))
                    @foreach(list[i]);
        }

        public static void Randomize<T>(this List<T> list)
        {
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static int NextEndLine(this List<byte> list)
        {
            for (int i = 0; i < list.Count; i++)
                if (i < (list.Count - 1))
                    if (list[i] == 13)
                        if (list[i + 1] == 10)
                            return i;

            return -1;
        }

        public static bool CanTakeLine(this List<byte> list)
        {
            return list.NextEndLine() > -1;
        }

        public static String TakeLine(this List<byte> list)
        {
            int index = list.NextEndLine();

            if (index > -1)
            {
                String str = Encoding.Default.GetString(list.ToArray(), 0, index);
                list.RemoveRange(0, (index + 2));
                return str;
            }

            return String.Empty;
        }
    }
}
