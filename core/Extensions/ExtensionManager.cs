/*
    sb0t ares chat server
    Copyright (C) 2016  AresChat

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using iconnect;

namespace core.Extensions
{
    public class ExtensionManager
    {
        private static String DataPath { get; set; }
        private static ConcurrentDictionary<String, ExPlugin> list { get; set; }

        internal static List<ExPlugin> Plugins
        {
            get { return list.Values.ToList(); }
        }

        public static void Setup()
        {
            list = new ConcurrentDictionary<String, ExPlugin>();

            DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
               "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\Extensions";

            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);

            DataPath += "\\";
        }

        public static ExtensionFrontEnd LoadPlugin(String name)
        {
            try
            {
                Assembly asm = Assembly.Load(File.ReadAllBytes(DataPath + name + "\\extension.dll"));
                Type type = asm.GetTypes().FirstOrDefault(x => x.GetInterface("iconnect.IExtension") != null);

                if (type != null)
                {
                    ExPlugin p = new ExPlugin
                    {
                        Name = name,
                        Plugin = (IExtension)Activator.CreateInstance(
                                 asm.GetType(type.ToString()),
                                 new ExHost(name))
                    };

                    UnloadPlugin(p.Name);
                    list[p.Name] = p;

                    try
                    {
                        list[p.Name].Plugin.Load();
                    }
                    catch { }

                    return new ExtensionFrontEnd
                    {
                        GUI = list[p.Name].Plugin.GUI,
                        Icon = list[p.Name].Plugin.Icon,
                        Name = list[p.Name].Name
                    };
                }
            }
            catch { }

            return null;
        }

        public static void UnloadPlugin(String name)
        {
            if (!list.ContainsKey(name))
                return;

            try
            {
                list[name].Plugin.Dispose();
            }
            catch { }

            ExPlugin p;

            while (!list.TryRemove(name, out p))
                continue;
        }

    }

    public class ExtensionFrontEnd
    {
        public UserControl GUI { get; set; }
        public BitmapSource Icon { get; set; }
        public String Name { get; set; }
    }
}
