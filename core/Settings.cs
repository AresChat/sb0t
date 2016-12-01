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
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;

namespace core
{
    public class Settings
    {
        public const String RELEASE_URL = "https://github.com/AresChat/sb0t/releases";
        public const String VERSION_CHECK_URL = "https://api.github.com/repos/AresChat/sb0t/releases";
        public const String VERSION_NUMBER = "5.36";

        public const String VERSION = "sb0t 5.36";
        public const ushort LINK_PROTO = 500;

        public static bool RUNNING { get; set; }
        public static String WebPath { get; set; }

        public static void Reset()
        {
            externalip = null;
            port = 0;
            name = null;
            language = 0;
            hide_ips = 0;

            WebPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                      "\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\Style";

            if (!Directory.Exists(WebPath))
            {
                Directory.CreateDirectory(WebPath);
                WebPath += "\\";
            }
            else WebPath += "\\";

            DoOnce.Run();
        }

        public static void ScriptCanLevel(bool can)
        {
            Events.ScriptCanLevel(can);
        }

        private static int hide_ips = 0;
        public static bool HideIps
        {
            get
            {
                if (hide_ips == 0)
                    hide_ips = Get<bool>("hide_ips") ? 2 : 1;

                return hide_ips == 2;
            }
        }

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

        private static ushort port { get; set; }
        public static ushort Port
        {
            get
            {
                if (port == 0)
                    port = Get<ushort>("port");

                return port;
            }
        }

        private static String name { get; set; }
        public static String Name
        {
            get
            {
                if (name == null)
                    name = Get<String>("name");

                return name;
            }
        }

        private static byte language { get; set; }
        public static byte Language
        {
            get
            {
                if (language == 0)
                    language = Get<byte>("language");

                return language;
            }
        }

        private static String topic { get; set; }
        public static String Topic
        {
            get
            {
                if (topic == null)
                    topic = Get<String>("topic");

                if (topic.Length < 2)
                {
                    topic = "welcome to my room";
                    Set("topic", topic);
                }

                return topic;
            }
            set
            {
                topic = value;
                Set("topic", topic);
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
