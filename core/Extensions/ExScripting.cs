using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iconnect;

namespace core.Extensions
{
    class ExScripting : IScripting
    {
        public bool ScriptEnabled
        {
            get { return Settings.Get<bool>("scripting"); }
        }

        public bool ScriptInRoom
        {
            get { return Settings.Get<bool>("inroom_scripting"); }
        }

        public byte ScriptLevel
        {
            get { return Settings.Get<byte>("inroom_level"); }
        }
    }
}
