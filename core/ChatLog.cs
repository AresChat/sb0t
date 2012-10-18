using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace core
{
    class ChatLog
    {
        public static void WriteLine(String text)
        {
            if (Settings.Get<bool>("logging"))
            {
                try
                {
                    DateTime d = DateTime.Now;

                    String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                      "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\ChatLogs";

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    path += ("\\" + d.Day + " " + d.Month + " " + d.Year + ".txt");

                    using (StreamWriter writer = File.Exists(path) ? File.AppendText(path) : File.CreateText(path))
                        writer.WriteLine(DateTime.Now.ToShortTimeString() + " " + text);
                }
                catch { }
            }
        }
    }
}
