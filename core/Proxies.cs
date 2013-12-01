using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace core
{
    class Proxies
    {
        private static String[] bad_ranges = new String[]
        {
            "46.16", // hotspot sheild
            "64.55.", // hotspot sheild
            "69.22.", // hotspot sheild
            "69.162.", // hotspot sheild
            "69.167.", // hotspot sheild
           // "74.115.", // hotspot sheild
            "128.241.", // hotspot sheild
            "140.174.", // hotspot sheild
            "204.2.", // hotspot sheild
            "206.14.", // hotspot sheild
            "209.107.", // hotspot sheild
            "199.255.", // hotspot sheild
            "31.210.102.38", // cyber ghost
            "46.4.62.16", // cyber ghost
            "67.221.255.12", // cyber ghost
            "74.82.218.250", // cyber ghost
            "77.245.78.10", // cyber ghost
            "79.168.9.111", // cyber ghost
            "83.142.226.101", // cyber ghost
            "85.195.76.6", // cyber ghost
            "89.217.238.", // cyber ghost
            "95.141.", // cyber ghost
            "95.142.", // cyber ghost
            "95.143.", // cyber ghost
            "95.215.", // cyber ghost
            "109.74.3.24", // cyber ghost
            "124.248.202.119", // cyber ghost
            "174.36.47.170", // cyber ghost
            "176.227.194.146", // cyber ghost
            "188.227.180.2", // cyber ghost
            "209.239.120.120", // cyber ghost
            "216.185.105.34", // cyber ghost
            "108.61.74.99", // cyber ghost
            "79.141.165.21", // cyber ghost
            "24.99.89.166", // cyber ghost
        };

        private static String[] bad_dns = new String[]
        {
            "webfusion",
            "cyberghost",
            "redstation",
            "center",
            "centre",
            "softlayer",
            "choopa",
            "lstn.net",
            "kryptote",
            "system",
            "vpn",
            "pptp",
            "cloud",
            "onlinehome",
            "hide"
        };

        public static bool Check(IClient client)
        {
            foreach (String ip in bad_ranges)
                if (client.ExternalIP.ToString().StartsWith(ip))
                {
                    return true;
                }

            foreach (String dns in bad_dns)
                if (client.DNS.ToLower().Contains(dns))
                {
                    return true;
                }

            bool result = false;

            lock (list)
            {
                int i = list.FindIndex(x => x.Equals(client.ExternalIP));
                result = i > -1;
            }

            return result;
        }



        /* TOR BEGINS */


        private static uint last { get; set; }
        private static List<IPAddress> list { get; set; }
        private static uint offset { get; set; }

        public static void Start(uint time)
        {
            list = new List<IPAddress>();
            last = 0;
            offset = (uint)new Random().Next(20000, 43200);

            uint tmp = Settings.Get<uint>("tor_time");

            if (tmp != 0)
            {
                last = tmp;

                String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                  "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\tor.dat";

                if (File.Exists(path))
                {
                    try
                    {
                        using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            byte[] buf = new byte[4];

                            lock (list)
                                while (stream.Read(buf, 0, 4) == 4)
                                    list.Add(new IPAddress(buf));
                        }
                    }
                    catch { }

                    lock (list)
                        ServerCore.Log("Loaded " + list.Count + " tor addresses from local collection");
                }
                else last = 0;
            }
            else
            {
                Settings.Set("tor_time", time);
                DownloadTor();
            }
        }

        private static void DownloadTor()
        {
            new Thread(new ThreadStart(() =>
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.dan.me.uk/torlist/");
                    request.Host = "www.dan.me.uk";
                    request.UserAgent = String.Empty;

                    using (WebResponse response = request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        String page = reader.ReadToEnd();
                        String[] split = page.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                        lock (list)
                        {
                            list.Clear();
                            IPAddress ip;

                            for (int i = 0; i < split.Length; i++)
                                if (IPAddress.TryParse(split[i], out ip))
                                    list.Add(ip);
                        }

                        String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                          "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\tor.dat";

                        try
                        {
                            lock (list)
                            {
                                List<byte> buf = new List<byte>();

                                foreach (IPAddress ip in list)
                                    buf.AddRange(ip.GetAddressBytes());

                                File.WriteAllBytes(path, buf.ToArray());
                                ServerCore.Log("Downloaded " + list.Count + " tor addresses from remote collection");
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            })).Start();
        }

        public static void Updater(uint time)
        {
            if (time > (last + offset))
            {
                Settings.Set("tor_time", time);
                last = time;
                DownloadTor();
            }
        }

    }
}
