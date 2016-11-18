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

namespace scripting.Instances
{
    class JSTimerInstance : ObjectInstance
    {
        public JSTimerInstance(ObjectInstance prototype)
            : base(prototype)
        {
            this.PopulateFunctions();
            this.ScriptName = this.Engine.ScriptName;
            this.StartTime = 0;

            JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == this.ScriptName);

            if (script != null)
            {
                this.IDENT = TimerList.NextIdent++;
                script.timer_idents.Add(this.IDENT);
            }
        }

        protected override string InternalClassName
        {
            get { return "Timer"; }
        }

        public String ScriptName { get; set; }
        public ulong StartTime { get; set; }

        public ulong IDENT;
        public bool m_enabled = false;
        private int m_interval = 1000;

        [JSFunction(Name = "start", IsWritable = false, IsEnumerable = true)]
        public bool Start()
        {
            return TimerList.AddTimer(this);
        }

        [JSFunction(Name = "stop", IsWritable = false, IsEnumerable = true)]
        public bool Stop()
        {
            return TimerList.RemoveTimer(this);
        }

        [JSProperty(Name = "interval")]
        public int Interval
        {
            get { return this.m_interval; }
            set
            {
                if (value >= 500)
                    this.m_interval = value;
                else
                    value = 500;
            }
        }

        [JSProperty(Name = "oncomplete")]
        public UserDefinedFunction Callback { get; set; }
    }
}
