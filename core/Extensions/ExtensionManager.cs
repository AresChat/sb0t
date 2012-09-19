using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using iconnect;

namespace core.Extensions
{
    class ExtensionManager
    {
        private static String DataPath { get; set; }
        private static List<ExPlugin> Plugins { get; set; }


        public static void Setup()
        {
            Plugins = new List<ExPlugin>();

            DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
               "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName;

            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);

            DataPath += "\\";
        }

        public static bool LoadPlugin(String name)
        {
            try
            {
                Assembly asm = Assembly.Load(File.ReadAllBytes(DataPath + name + ".dll"));
                
                Type type = asm.GetTypes().FirstOrDefault(x =>
                    x.GetInterface("iconnect.IExtension", true) != null);

                if (type != null)
                {
                    Plugins.Add(new ExPlugin
                    {
                        Name = name,
                        Plugin = (IExtension)Activator.CreateInstance(typeof(IExtension), new ExHost())
                    });

                    return true;
                }
            }
            catch { }

            return false;
        }

        public static void UnloadPlugin(String name)
        {
            Plugins.RemoveAll(x => x.Name == name);
        }

    }
}
