using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using iconnect;

namespace gui
{
    class CommandManager
    {
        public static void UpdateCommand(String name, String level)
        {
            SetLevel(name, StringToLevel(level));
        }

        public static String LevelToString(ILevel level)
        {
            switch (level)
            {
                case ILevel.Administrator:
                    return "Administrator";

                case ILevel.Host:
                    return "Host";

                case ILevel.Moderator:
                    return "Moderator";

                case (ILevel)254:
                    return "Disabled";

                default:
                    return "Regular";
            }
        }

        public static ILevel StringToLevel(String level)
        {
            switch (level)
            {
                case "Administrator":
                    return ILevel.Administrator;

                case "Host":
                    return ILevel.Host;

                case "Moderator":
                    return ILevel.Moderator;

                case "Disabled":
                    return (ILevel)254;

                case "Regular":
                    return ILevel.Regular;

                default:
                    return (ILevel)255; // not found
            }
        }

        public static ILevel GetLevel(String command)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\commands");

            if (key != null)
            {
                object value = key.GetValue(command);
                key.Close();

                if (value != null)
                    return (ILevel)(byte)(int)value;
            }

            return (ILevel)255; // not found
        }

        public static void SetLevel(String name, ILevel level)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\commands", true);

            if (key == null)
            {
                Registry.CurrentUser.CreateSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\commands");
                key = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\commands", true);
            }

            key.SetValue(name, (int)(byte)level, RegistryValueKind.DWord);
            key.Close();
        }
    }
}
