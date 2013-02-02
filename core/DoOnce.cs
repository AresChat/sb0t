using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace core
{
    class DoOnce
    {
        public static void Run()
        {
            uint target = 1;
            uint i = Settings.Get<uint>("do_once");

            if (i != target)
                Settings.Set("do_once", target);

            if (i == 0 || !File.Exists(Settings.WebPath + "template.htm"))
            {
                FixTemplate();
            }
        }

        private static void FixTemplate()
        {
            if (File.Exists(Settings.WebPath + "template.htm"))
                try
                {
                    File.WriteAllBytes(Settings.WebPath + "template.broken", File.ReadAllBytes(Settings.WebPath + "template.htm"));
                }
                catch { }

            try
            {
                File.WriteAllBytes(Settings.WebPath + "template.htm", Resource1.template);
            }
            catch { }
        }
    }
}
