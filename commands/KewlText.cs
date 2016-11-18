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
using iconnect;

namespace commands
{
    class KewlText
    {
        private static List<Guid> list { get; set; }

        public static void Clear()
        {
            list = new List<Guid>();
        }

        public static void Add(IUser client)
        {
            if (list.FindIndex(x => x.Equals(client.Guid)) == -1)
                list.Add(client.Guid);
        }

        public static void Remove(IUser client)
        {
            list.RemoveAll(x => x.Equals(client.Guid));
        }

        public static bool IsKewlText(IUser client)
        {
            return list.FindIndex(x => x.Equals(client.Guid)) > -1;
        }

        public static String UnicodeText(String text)
        {
            String outText = text;

            char[] oldU = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            char[] oldL = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            char[] newU = "ΛβÇĐΞ₣ĢĦÏĴКĿMИΘ₱QЯŠŦЏ√ШЖ¥Ź".ToCharArray();
            char[] newL = "αвс∂εfgнιјκłмησρqяѕтυνωxчz".ToCharArray();

            for (int x = 0; x < 26; x++)
            {
                outText = outText.Replace(oldL[x].ToString(), newL[x].ToString());
                outText = outText.Replace(oldU[x].ToString(), newU[x].ToString());
            }

            return outText;
        }
    }
}
