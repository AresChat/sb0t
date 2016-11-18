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
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net;
using System.Threading;
using iconnect;

namespace commands
{
    class Trace
    {
        public static ConcurrentQueue<TraceResult> RESULTS = new ConcurrentQueue<TraceResult>();

        public static void Lookup(String text)
        {
            new Thread(new ThreadStart(() =>
            {
                String url = "http://api.ipinfodb.com/v3/ip-city/?key=a91bf4749409447a6ba31d6f68b22140b1c87825c72f2c3ff337f9a5dd52f917&ip="
                    + Uri.EscapeDataString(text) + "&format=json";

                WebRequest request = WebRequest.Create(url);

                try
                {
                    using (WebResponse response = request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    {
                        DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(TraceResult));
                        TraceResult result = (TraceResult)json.ReadObject(stream);
                        result.owner = text;
                        RESULTS.Enqueue(result);
                    }
                }
                catch
                {
                    RESULTS.Enqueue(new TraceResult { owner = text });
                }
            })).Start();
        }

        public static void Lookup(IUser client)
        {
            new Thread(new ThreadStart(() =>
            {
               /* if (client.Encrypted)
                {
                    RESULTS.Enqueue(new TraceResult { owner = client.Name });
                    return;
                } */

                String url = "http://api.ipinfodb.com/v3/ip-city/?key=a91bf4749409447a6ba31d6f68b22140b1c87825c72f2c3ff337f9a5dd52f917&ip="
                    + Uri.EscapeDataString(client.ExternalIP.ToString()) + "&format=json";

                WebRequest request = WebRequest.Create(url);

                try
                {
                    using (WebResponse response = request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    {
                        DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(TraceResult));
                        TraceResult result = (TraceResult)json.ReadObject(stream);
                        result.owner = client.Name;
                        RESULTS.Enqueue(result);
                    }
                }
                catch
                {
                    RESULTS.Enqueue(new TraceResult { owner = client.Name });
                }
            })).Start();
        }

        public static void Show(TraceResult trace)
        {
            bool got_it = false;

            if (!String.IsNullOrEmpty(trace.countryName))
                if (trace.countryName.Length > 1)
                    got_it = true;

            if (!got_it)
                Server.Print(Template.Text(Category.Trace, 0).Replace("+n", trace.owner), true);
            else
            {
                Server.Print(Template.Text(Category.Trace, 1).Replace("+n", trace.owner), true);
                Server.Print(String.Empty, true);
                Server.Print(Template.Text(Category.Trace, 2).Replace("+n", format(trace.countryName)), true);

                if (!String.IsNullOrEmpty(trace.regionName))
                    if (trace.regionName.Length > 1)
                        Server.Print(Template.Text(Category.Trace, 3).Replace("+n", format(trace.regionName)), true);

                if (!String.IsNullOrEmpty(trace.cityName))
                    if (trace.cityName.Length > 1)
                        Server.Print(Template.Text(Category.Trace, 4).Replace("+n", format(trace.cityName)), true);

                if (!String.IsNullOrEmpty(trace.timeZone))
                    if (trace.timeZone.Length > 1)
                    {
                        String tz = trace.timeZone;
                        bool minus = false;

                        if (tz.StartsWith("-"))
                        {
                            tz = tz.Substring(1);
                            minus = true;
                        }

                        String[] sp = tz.Split(new String[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

                        if (sp.Length == 2)
                        {
                            int mins = 0;
                            int i;

                            if (int.TryParse(sp[0], out i))
                            {
                                mins += (i * 60);

                                if (int.TryParse(sp[1], out i))
                                {
                                    mins += i;

                                    if (minus)
                                        mins = (mins * -1);

                                    DateTime dt = DateTime.UtcNow.AddMinutes(mins);
                                    Server.Print(Template.Text(Category.Trace, 5).Replace("+n", dt.ToShortTimeString()), true);
                                }
                            }
                        }
                    }

                Server.Print(String.Empty, true);
                Server.Print(Template.Text(Category.Trace, 6), true);
            }
        }

        private static String format(String str)
        {
            String[] words = str.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; i++)
                words[i] = words[i].Substring(0, 1).ToUpper() + words[i].Substring(1).ToLower();

            return String.Join(" ", words);
        }
    }

    [DataContract]
    class TraceResult
    {
        [IgnoreDataMember]
        public String owner { get; set; }
        [DataMember]
        public String countryName { get; set; }
        [DataMember]
        public String regionName { get; set; }
        [DataMember]
        public String cityName { get; set; }
        [DataMember]
        public String timeZone { get; set; }
    }
}
