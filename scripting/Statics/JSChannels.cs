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
using Jurassic;
using Jurassic.Library;
using iconnect;

namespace scripting.Statics
{
    [JSEmbed(Name = "Channels")]
    class JSChannels : ObjectInstance
    {
        public JSChannels(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Channels"; }
        }

        [JSProperty(Name = "available")]
        public static bool Available
        {
            get { return Server.Channels.Available; }
            set { }
        }

        [JSProperty(Name = "enabled")]
        public static bool Enabled
        {
            get { return Server.Channels.Enabled; }
            set { }
        }

        [JSFunction(Name = "search", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static Objects.JSChannelCollection Search(ScriptEngine eng, Object a)
        {
            List<Objects.JSChannel> results = new List<Objects.JSChannel>();

            if (!(a is Undefined))
            {
                String str = a.ToString().ToUpper();
                List<IChannelItem> matches = new List<IChannelItem>();

                Server.Channels.ForEach(x =>
                {
                    if (x.Name.ToUpper().Contains(str))
                        if (x.Users < 200)
                            matches.Add(x);
                });

                matches.Sort((y, x) => x.Users.CompareTo(y.Users));

                if (matches.Count > 10)
                    matches = matches.GetRange(0, 10);

                foreach (IChannelItem m in matches)
                    results.Add(new Objects.JSChannel(eng.Object.InstancePrototype, m));
            }

            return new Objects.JSChannelCollection(eng.Object.InstancePrototype, results.ToArray(), eng.ScriptName);
        }
    }
}
