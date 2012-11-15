using System;
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
        public bool has_related_words { get; set; }
        [DataMember]
        public String result_type { get; set; }
        [DataMember]
        public List<UrbanDictionaryListItem> list { get; set; }
        [DataMember]
        public List<String> sounds { get; set; }
        [DataMember]
        public int total { get; set; }
        [DataMember]
        public int pages { get; set; }
    }

    [DataContract]
    class UrbanDictionaryListItem
    {
        [DataMember]
        public int defid { get; set; }
        [DataMember]
        public String word { get; set; }
        [DataMember]
        public String permalink { get; set; }
        [DataMember]
        public String definition { get; set; }
        [DataMember]
        public String example { get; set; }
        [DataMember]
        public int thumbs_up { get; set; }
        [DataMember]
        public int thumbs_down { get; set; }
        [DataMember]
        public String current_vote { get; set; }
    }
}
