using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;
using System.Reflection;
using iconnect;

namespace core.Extensions
{
    class ExtensionManager
    {
        private static String DataPath { get; set; }
        private static ConcurrentDictionary<String, ExPlugin> list { get; set; }

        public static List<ExPlugin> Plugins
        {
            get { return list.Values.ToList(); }
        }

        public static void Setup()
        {
            list = new ConcurrentDictionary<String, ExPlugin>();
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
                        Plugin = (IExtension)Activator.CreateInstance(
                                 asm.GetType(type.ToString()),
                                 new ExHost(name + ".dll"))
                    };

                    UnloadPlugin(p.Name);
                    list[p.Name] = p;                    

                    return true;
                }
            }
            catch { }

            return false;
        }

        public static ExPlugin UnloadPlugin(String name)
        {
            if (!list.ContainsKey(name))
                return null;

            ExPlugin p;

            while (!list.TryRemove(name, out p))
                continue;

            return p;
        }

    }
}
