using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace core
{
    class Helpers
    {
        public static void FormatUsername(AresClient client)
        {
            if (client.OrgName == Settings.Get<String>("bot"))
                client.OrgName = String.Empty;

            client.OrgName = Regex.Replace(client.OrgName, Regex.Escape("_"), " ", RegexOptions.IgnoreCase);
            client.OrgName = Regex.Replace(client.OrgName, Regex.Escape("\""), String.Empty, RegexOptions.IgnoreCase);
            client.OrgName = Regex.Replace(client.OrgName, Regex.Escape("/"), String.Empty, RegexOptions.IgnoreCase);
            client.OrgName = Regex.Replace(client.OrgName, Regex.Escape("\\"), String.Empty, RegexOptions.IgnoreCase);
            client.OrgName = Regex.Replace(client.OrgName, Regex.Escape("www."), String.Empty, RegexOptions.IgnoreCase);

            while (Encoding.UTF8.GetByteCount(client.OrgName) > 20)
                client.OrgName = client.OrgName.Substring(0, client.OrgName.Length - 1);

            if (client.OrgName.Length < 2)
                client.OrgName = "anon " + client.Cookie;
        }

        public static void PopulateCommand(Command cmd)
        {

        }
    }

    class Command
    {
        public String Text { get; set; }
        public AresClient Target { get; set; }
        public String Args { get; set; }
    }
}
