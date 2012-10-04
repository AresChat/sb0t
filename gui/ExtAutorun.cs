using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace gui
{
    class ExtAutorun
    {
        public static void AddItem(String name)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\extautorun", true);

            if (key == null)
            {
                Registry.CurrentUser.CreateSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\extautorun");
                key = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\extautorun", true);
            }

            if (key != null)
            {
                key.SetValue(name, 0);
                key.Close();
            }
        }

        public static void RemoveItem(String name)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\extautorun", true);

            if (key == null)
            {
                Registry.CurrentUser.CreateSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\extautorun");
                key = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\extautorun", true);
            }

            if (key != null)
            {
                key.DeleteValue(name, false);
                key.Close();
            }
        }

        public static String[] Items
        {
            get
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\extautorun");

                if (key != null)
                {
                    String[] results = key.GetValueNames();
                    key.Close();

                    if (results == null)
                        return new String[] { };
                    else
                        return results;
                }

                return new String[] { };
            }
        }
    }
}
