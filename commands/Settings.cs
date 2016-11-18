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
using Microsoft.Win32;

namespace commands
{
    class Settings
    {
        public static bool DisableAdmins { get; set; }

        public static bool AdminAnnounce
        {
            get { return Get<bool>("adminannounce"); }
            set { Set("adminannounce", value); }
        }

        public static bool Colors
        {
            get { return Get<bool>("colors"); }
            set { Set("colors", value); }
        }

        public static bool Stealth
        {
            get { return Get<bool>("stealth"); }
            set { Set("stealth", value); }
        }

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

        public static bool RoomInfo
        {
            get { return Get<bool>("roominfo"); }
            set { Set("roominfo", value); }
        }

        public static bool LastSeen
        {
            get { return Get<bool>("lastseen"); }
            set { Set("lastseen", value); }
        }

        public static bool History
        {
            get { return Get<bool>("history"); }
            set { Set("history", value); }
        }

        public static String PMGreetMsgText
        {
            get { return Get<String>("pmgreetmsgtext"); }
            set { Set("pmgreetmsgtext", value); }
        }

        public static String Status
        {
            get { return Get<String>("status"); }
            set { Set("status", value); }
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
