using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using Microsoft.Win32;

namespace core
{
    public class Settings
    {
        public const String VERSION = "sb0t 5.00";

        public static bool RUNNING { get; set; }

        private static IPAddress externalip { get; set; }

        public static IPAddress ExternalIP
        {
            get
            {
                if (externalip != null)
                    return externalip;

                byte[] buffer = Get<byte[]>("ip");

                if (buffer != null)
                    externalip = new IPAddress(buffer);
                else
                    externalip = IPAddress.Loopback;

                return externalip;
            }
            set
            {
                externalip = value;
                Set("ip", externalip.GetAddressBytes());
            }
        }

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

            if (key == null)
            {
                Registry.CurrentUser.CreateSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + subpath);
                key = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + subpath);
            }

            if (key != null)
            {
                object value = key.GetValue(name);
                key.Close();
                Type type = typeof(T);

                for (int i = 0; i < AcceptableTypes.Length; i++)
                    if (AcceptableTypes[i].Equals(type))
                        if (i <= 4)
                        {
                            if (value != null)
                                return (T)Convert.ChangeType(value, typeof(T));
                            else
                                return (T)Convert.ChangeType(0, typeof(T));
                        }
                        else if (i == 6)
                        {
                            if (value != null)
                                return (T)Convert.ChangeType(value, typeof(T));
                            else
                                return default(T);
                        }
                        else if (i == 5)
                        {
                            if (value != null)
                                return (T)Convert.ChangeType(Encoding.UTF8.GetString((byte[])value), typeof(T));
                            else
                                return (T)Convert.ChangeType(String.Empty, typeof(T));
                        }
                        else if (i == 7)
                        {
                            if (value != null)
                                return (T)Convert.ChangeType(((int)value == 1), typeof(T));
                            else
                                return (T)Convert.ChangeType(false, typeof(T));
                        }
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

        public static IPAddress LocalIP
        {
            get
            {
                foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        return ip;

                return IPAddress.Loopback;
            }
        }
    }
}
