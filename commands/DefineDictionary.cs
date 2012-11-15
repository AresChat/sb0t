using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net;
using System.Threading;

namespace commands
{
    class DefineDictionary
    {
        public static ConcurrentQueue<DictionaryResultCollection> RESULTS = new ConcurrentQueue<DictionaryResultCollection>();

        public static void Lookup(String text)
        {
            new Thread(new ThreadStart(() =>
            {
                String url = "http://api.wordnik.com//v4/word.json/" + Uri.EscapeDataString(text.ToLower()) +
                    "/definitions?includeRelated=false&includeTags=false&limit=5&sourceDictionaries=all&useCanonical=false";

                WebRequest request = WebRequest.Create(url);
                request.Headers.Add("api_key", "0f69e2f981991cfe0e1351afd6a2d39da10077112d21165be");

                try
                {
                    using (WebResponse response = request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    {
                        DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(DictionaryResultCollection));
                        DictionaryResultCollection result = (DictionaryResultCollection)json.ReadObject(stream);
                        result.org_text = text;
                        result.result_type = "exact";
                        RESULTS.Enqueue(result);
                    }
                }
                catch
                {
                    RESULTS.Enqueue(new DictionaryResultCollection { result_type = String.Empty, org_text = text });
                }
            })).Start();
        }

        public static void Show(DictionaryResultCollection dict)
        {
            if (dict.result_type != "exact" || dict.Count == 0)
                Server.Print(Template.Text(Category.Define, 0).Replace("+n", dict.org_text));
            else
            {
                Server.Print(Template.Text(Category.Define, 1).Replace("+n", dict.org_text));
                Server.Print(String.Empty);

                foreach (DictionaryResult dr in dict)
                    Server.Print(Template.Text(Category.Define, 2).Replace("+n", dr.text));
            }
        }
    }

    [DataContract]
    class DictionaryResult
    {
        [DataMember]
        public String text { get; set; }
    }

    class DictionaryResultCollection : List<DictionaryResult>
    {
        [IgnoreDataMember]
        public String org_text { get; set; }
        [IgnoreDataMember]
        public String result_type { get; set; }
    }
}
