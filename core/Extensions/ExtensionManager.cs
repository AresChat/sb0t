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

        public static ConcurrentList<ExPlugin> Plugins { get; set; }


        public static void Setup()
        {
            Plugins = new ConcurrentList<ExPlugin>();
            DataPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        public static bool LoadPlugin(String name)
        {
            try
            {
                Assembly asm = Assembly.Load(File.ReadAllBytes(DataPath + name + ".dll"));

                Type type = asm.GetTypes().FirstOrDefault(x =>
                    x.GetInterface("iconnect.IExtension") != null);

                if (type != null)
                {
                    ExPlugin p = new ExPlugin
                    {
                        Name = name,
                        Plugin = (IExtension)Activator.CreateInstance(asm.GetType(type.ToString()), new ExHost(name + ".dll"))
                    };

                    Plugins.RemoveAll(x => x.Name == p.Name);
                    Plugins.Add(p);

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
