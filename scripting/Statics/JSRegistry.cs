using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Jurassic;
using Jurassic.Library;

namespace scripting.Statics
{
    [JSEmbed(Name = "Registry")]
    class JSRegistry : ObjectInstance
    {
        public JSRegistry(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Registry"; }
        }

        [JSFunction(Name = "exists", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool Exists(ScriptEngine eng, String key)
        {
            String script = eng.ScriptName;
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\scripting", false);

            if (rk == null)
                return false;

            rk.Close();
            rk = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\scripting\\" + script, false);

            if (rk == null)
                return false;

            object value = rk.GetValue(key);
            rk.Close();

            if (value == null)
                return false;

            return true;
        }

        [JSFunction(Name = "getValue", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static String GetValue(ScriptEngine eng, String key)
        {
            String script = eng.ScriptName;
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\scripting", false);

            if (rk == null)
                return null;

            rk.Close();
            rk = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\scripting\\" + script, false);

            if (rk == null)
                return null;

            object value = rk.GetValue(key);
            rk.Close();

            if (value == null)
                return null;

            String str = (String)value;

            if (str.Length > 1)
                return str.Substring(2);

            return null;
        }

        [JSFunction(Name = "getKeys", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static Objects.JSRegistryKeyCollection GeyKeys(ScriptEngine eng)
        {
            List<String> results = new List<String>();
            String script = eng.ScriptName;
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\scripting", false);

            if (rk == null)
                return new Objects.JSRegistryKeyCollection(eng.Object.InstancePrototype, results.ToArray(), eng.ScriptName);

            rk.Close();
            rk = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\scripting\\" + script, false);

            if (rk == null)
                return new Objects.JSRegistryKeyCollection(eng.Object.InstancePrototype, results.ToArray(), eng.ScriptName);

            foreach (String str in rk.GetValueNames())
                results.Add(str);

            return new Objects.JSRegistryKeyCollection(eng.Object.InstancePrototype, results.ToArray(), eng.ScriptName);
        }

        [JSFunction(Name = "setValue", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool SetValue(ScriptEngine eng, String key, object value)
        {
            String script = eng.ScriptName;
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\scripting", false);

            if (rk == null)
                Registry.CurrentUser.CreateSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\scripting");
            else
                rk.Close();

            rk = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\scripting\\" + script, true);

            if (rk == null)
            {
                Registry.CurrentUser.CreateSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\scripting\\" + script);
                rk = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\scripting\\" + script, true);
            }

            if (value is int || value is String || value is double || value is ConcatenatedString)
            {
                rk.SetValue(key, (String)("ST" + value.ToString()));
                rk.Close();
                return true;
            }

            rk.Close();
            return false;
        }

        [JSFunction(Name = "deleteValue", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool DeleteValue(ScriptEngine eng, String key)
        {
            String script = eng.ScriptName;
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\scripting", false);

            if (rk == null)
                return false;
            else
                rk.Close();

            rk = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\scripting\\" + script, true);

            if (rk == null)
                return false;

            try
            {
                rk.DeleteValue(key, true);
            }
            catch  // value doesn't exist
            {
                rk.Close();
                return false;
            }

            rk.Close();
            return true;
        }

        [JSFunction(Name = "clear", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static bool Clear(ScriptEngine eng)
        {
            String script = eng.ScriptName;
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software\\sb0t\\" + AppDomain.CurrentDomain.FriendlyName + "\\scripting", true);

            if (rk == null)
                return false;

            try
            {
                rk.DeleteSubKey(script, true);
            }
            catch // script subkey doesn't exist
            {
                rk.Close();
                return false;
            }

            rk.Close();
            return true;
        }
    }
}
