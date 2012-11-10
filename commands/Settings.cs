using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace commands
{
    class Settings
    {
        public static bool Url
        {
            get { return Get<bool>("url"); }
            set { Set("url", value); }
        }

        public static byte MuzzleTimeout
        {
            get { return Get<byte>("mtimeout"); }
            set { Set("mtimeout", value); }
        }

        public static bool Clock
        {
            get { return Get<bool>("clock"); }
            set { Set("clock", value); }
        }

        public static bool AnonMonitoring
        {
            get { return Get<bool>("anonmonitoring"); }
            set { Set("anonmonitoring", value); }
        }

        public static bool ShareFileMonitoring
        {
            get { return Get<bool>("sharemonitoring"); }
            set { Set("sharemonitoring", value); }
        }

        public static bool Filtering
        {
            get { return Get<bool>("filtering"); }
            set { Set("filtering", value); }
        }

        public static bool CapsMonitoring
        {
            get { return Get<bool>("capsmonitoring"); }
            set { Set("capsmonitoring", value); }
        }

        public static bool IdleMonitoring
        {
            get { return Get<bool>("idlemonitoring"); }
            set { Set("idlemonitoring", value); }
        }

        public static bool General
        {
            get { return Get<bool>("general"); }
            set { Set("general", value); }
        }

        public static bool GreetMsg
        {
            get { return Get<bool>("greetmsg"); }
            set { Set("greetmsg", value); }
        }

        public static bool PMGreetMsg
        {
            get { return Get<bool>("pmgreetmsg"); }
            set { Set("pmgreetmsg", value); }
        }

        public static String PMGreetMsgText
        {
            get { return Get<String>("pmgreetmsgtext"); }
            set { Set("pmgreetmsgtext", value); }
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

        public static T Get<T>(String name)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\settings");

            if (key == null)
            {
                Registry.CurrentUser.CreateSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\settings");
                key = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\settings");
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

        public static bool Set(String name, object value)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\settings", true);

            if (key == null)
            {
                Registry.CurrentUser.CreateSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\settings");
                key = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\settings", true);
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
