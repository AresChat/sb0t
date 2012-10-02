using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            "74.115.", // hotspot sheild
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
            "95.215.63.29", // cyber ghost
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
            "server",
            "choopa",
            "lstn.net",
            "kryptotel",
            "system1",
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
                    return true;

            foreach (String dns in bad_dns)
                if (client.DNS.ToLower().Contains(dns))
                    return true;

            return false;
        }
    }
}
