using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32;

namespace core
{
    class Settings
    {
        public const String VERSION = "sb0t 5.00";

        private static Type[] AcceptableTypes = 
        {
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(String),
            typeof(byte[]),
            typeof(bool)
        };

        public static T Get<T>(String name, params String[] subkeys)
        {
            String subpath = String.Empty;

            foreach (String str in subkeys)
                subpath += "\\" + str;

            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + subpath);

            if (key != null)
            {
                object value = key.GetValue(name);
                key.Close();
                Type type = typeof(T);

                for (int i = 0; i < AcceptableTypes.Length; i++)
                    if (AcceptableTypes[i].Equals(type))
                        if (i <= 4 || i == 6)
                            return (T)Convert.ChangeType(value, typeof(T));
                        else if (i == 5)
                            return (T)Convert.ChangeType(Encoding.UTF8.GetString((byte[])value), typeof(T));
                        else if (i == 7)
                            return (T)Convert.ChangeType(((int)value == 1), typeof(T));
            }

            throw new Exception("Registry value not found or invalid casting");
        }

        public static bool Set(String name, object value, params String[] subkeys)
        {
            String subpath = String.Empty;

            foreach (String str in subkeys)
                subpath += "\\" + str;

            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + subpath, true);

            if (key == null)
            {
                Registry.CurrentUser.CreateSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + subpath);
                key = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + subpath, true);
            }

            Type type = value.GetType();

            for (int i = 0; i < AcceptableTypes.Length; i++)
            {
                if (AcceptableTypes[i].Equals(type))
                {
                    if (i <= 4)
                    {
                        key.SetValue(name, value, RegistryValueKind.DWord);
                        key.Close();
                        return true;
                    }
                    else if (i == 5)
                    {
                        key.SetValue(name, Encoding.UTF8.GetBytes((String)value), RegistryValueKind.Binary);
                        key.Close();
                        return true;
                    }
                    else if (i == 6)
                    {
                        key.SetValue(name, value, RegistryValueKind.Binary);
                        key.Close();
                        return true;
                    }
                    else if (i == 7)
                    {
                        key.SetValue(name, ((bool)value ? (int)1 : (int)0), RegistryValueKind.DWord);
                        key.Close();
                        return true;
                    }
                }
            }

            key.Close();
            return false;
        }
    }
}
