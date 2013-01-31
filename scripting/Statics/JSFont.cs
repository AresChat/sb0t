using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace scripting.Statics
{
    [JSEmbed(Name = "Font")]
    class JSFont : ObjectInstance
    {
        public JSFont(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Font"; }
        }

        [JSProperty(Name = "enabled")]
        public static bool Enabled
        {
            get { return Server.Chatroom.Font.Enabled; }
            set { Server.Chatroom.Font.Enabled = value; }
        }

        [JSProperty(Name = "nameColor")]
        public static String NameColor
        {
            get { return Server.Chatroom.Font.NameColor; }
            set { Server.Chatroom.Font.NameColor = value; }
        }

        [JSProperty(Name = "textColor")]
        public static String TextColor
        {
            get { return Server.Chatroom.Font.TextColor; }
            set { Server.Chatroom.Font.TextColor = value; }
        }

        [JSProperty(Name = "size")]
        public static int Size
        {
            get { return Server.Chatroom.Font.Size; }
            set
            {
                if (value >= 8 && value <= 18)
                    Server.Chatroom.Font.Size = value;
            }
        }

        [JSProperty(Name = "family")]
        public static String Name
        {
            get { return Server.Chatroom.Font.FontName; }
            set { Server.Chatroom.Font.FontName = value; }
        }
    }
}
