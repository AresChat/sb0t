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
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net;
using System.Threading;

namespace commands
{
    class UrbanDictionary
    {
        public static ConcurrentQueue<UrbanDictionaryResult> RESULTS = new ConcurrentQueue<UrbanDictionaryResult>();

        public static void Lookup(String text)
        {
            new Thread(new ThreadStart(() =>
            {
                String url = "http://www.urbandictionary.com/iphone/search/define?term=" + Uri.EscapeDataString(text);
                WebRequest request = WebRequest.Create(url);

                try
                {
                    using (WebResponse response = request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    {
                        DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(UrbanDictionaryResult));
                        UrbanDictionaryResult result = (UrbanDictionaryResult)json.ReadObject(stream);
                        result.org_text = text;
                        RESULTS.Enqueue(result);
                    }
                }
                catch
                {
                    RESULTS.Enqueue(new UrbanDictionaryResult { result_type = String.Empty, org_text = text });
                }
            })).Start();
        }

        public static void Show(UrbanDictionaryResult urban)
        {
            if (urban.result_type != "exact" || urban.list.Count == 0)
                Server.Print(Template.Text(Category.UrbanDictionary, 0).Replace("+n", urban.org_text));
            else
            {
                Server.Print(Template.Text(Category.UrbanDictionary, 1).Replace("+n", urban.org_text));
                Server.Print(String.Empty);
                String[] def = urban.list[0].definition.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (String str in def)
                    Server.Print(Template.Text(Category.UrbanDictionary, 2).Replace("+n", str));

                String[] eg = urban.list[0].example.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (eg.Length > 0)
                {
                    Server.Print(String.Empty);
                    Server.Print(Template.Text(Category.UrbanDictionary, 3).Replace("+n", urban.org_text));
                    Server.Print(String.Empty);

                    foreach (String str in eg)
                        Server.Print(Template.Text(Category.UrbanDictionary, 4).Replace("+n", str));
                }
            }
        }
    }

    [DataContract]
    class UrbanDictionaryResult
    {
        [IgnoreDataMember]
        public String org_text { get; set; }
        [DataMember]
        public String result_type { get; set; }
        [DataMember]
        public List<UrbanDictionaryListItem> list { get; set; }
    }

    [DataContract]
    class UrbanDictionaryListItem
    {
        [DataMember]
        public String definition { get; set; }
        [DataMember]
        public String example { get; set; }
    }
}
