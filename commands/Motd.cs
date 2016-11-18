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
using System.IO;
using iconnect;

namespace commands
{
    class Motd
    {
        private static List<String> lines = new List<String>();

        private static String yt = "<div style=\"margin-left: 2px;\"><object width=\"420\" height=\"315\">" +
                                   "<param name=\"movie\" value=\"https://www.youtube.com/v/LINK?version=3&autoplay=0\"></param>" +
                                   "<param name=\"allowScriptAccess\" value=\"always\"></param>" +
                                   "<embed src=\"https://www.youtube.com/v/LINK?version=3&autoplay=0\" " +
                                   "type=\"application/x-shockwave-flash\" " +
                                   "allowscriptaccess=\"always\" " +
                                   "wmode=\"opaque\" " +
                                   "width=\"420\" height=\"315\"></embed>" +
                                   "</object></div>";

        public static void LoadMOTD()
        {
            try
            {
                if (File.Exists(Server.DataPath + "motd.txt"))
                    lines = new List<String>(File.ReadAllLines(Server.DataPath + "motd.txt"));
                else
                {
                    lines.Add("304Welcome to my room +n :-)");
                    lines.Add("303Have fun!");
                    File.WriteAllLines(Server.DataPath + "motd.txt", lines.ToArray(), Encoding.UTF8);
                }
            }
            catch { }
        }

        public static void ViewMOTD(IUser client)
        {
            if (client.SupportsHTML)
                client.SendHTML("<!--MOTDSTART-->");

            String _str = Helpers.CountryCodeToString(client.Country);
            List<String> list = new List<String>();
            Server.Users.Ares(x => list.Add(x.Name));
            Server.Users.Web(x => list.Add(x.Name));
            list.RemoveAll(x => x == client.Name);
            String rnd_user = client.Name;

            if (list.Count > 0)
            {
                int index = (int)Math.Floor(new Random().NextDouble() * list.Count);
                rnd_user = list[index];
            }

            if (_str != "?")
                if (client.Region.Length > 0)
                    _str = client.Region + ", " + _str;

            if (_str == "?")
                if (client.Region.Length > 0)
                    _str = client.Region;

            if (_str == "?")
                _str = "unknown";

            foreach (String str in lines)
            {
                String html = str.Trim();

                if (html.StartsWith("[youtube=") && html.EndsWith("]") && client.SupportsHTML)
                {
                    html = html.Substring(9, html.Length - 10);
                    client.SendHTML("<!--EMBEDYOUTUBE:" + html + "-->");
                    html = yt.Replace("LINK", html);
                    client.SendHTML(html);
                }
                else if (html.StartsWith("[image=") && html.EndsWith("]") && client.SupportsHTML)
                {
                    html = html.Substring(7, html.Length - 8);
                    html = "<img src=\"" + html + "\" style=\"max-width: 420px; max-height: 420px;\" alt=\"\" />";
                    client.SendHTML(html);
                }
                else if (html.StartsWith("[poster=") && html.EndsWith("]") && client.SupportsHTML)
                {
                    html = html.Substring(8, html.Length - 9);
                    html = "<img src=\"" + html + "\" style=\"max-width: 75%; display: block; margin-left: auto; margin-right: auto;\" alt=\"\" />";
                    client.SendHTML(html);
                }
                else if (html.StartsWith("[audio=") && html.EndsWith("]") && client.SupportsHTML)
                {
                    html = html.Substring(7, html.Length - 8);
                    html = "<audio src=\"" + html + "\" autoplay />";
                    client.SendHTML(html);
                }
                else if (html.StartsWith("[video=") && html.EndsWith("]") && client.SupportsHTML)
                {
                    html = html.Substring(7, html.Length - 8);
                    html = "<video src=\"" + html + "\" width=\"420\" height=\"315\" controls />";
                    client.SendHTML(html);
                }
                else
                {
                    String s = str;
                    s = s.Replace("+n", client.Name);
                    s = s.Replace("+ip", client.ExternalIP.ToString());
                    s = s.Replace("+id", client.ID.ToString());
                    s = s.Replace("+f", client.FileCount.ToString());
                    s = s.Replace("+v", client.Version);
                    s = s.Replace("+p", client.DataPort.ToString());
                    s = s.Replace("+uc", Server.Stats.CurrentUserCount.ToString());
                    s = s.Replace("+rn", Server.Chatroom.Name);
                    s = s.Replace("+ut", Helpers.GetUptime);
                    s = s.Replace("+ru", rnd_user);
                    s = s.Replace("+l", _str);

                    if (s.Length == 0)
                        s = " ";

                    client.Print(Helpers.SetColors(s));
                }
            }

            if (client.SupportsHTML)
                client.SendHTML("<!--MOTDEND-->");
        }

        public static int MotdSize
        {
            get
            {
                return Encoding.UTF8.GetByteCount(String.Join(String.Empty, lines.ToArray()));
            }
        }
    }
}
