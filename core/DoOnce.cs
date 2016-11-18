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
using System.IO;

namespace core
{
    class DoOnce
    {
        public static void Run()
        {
            uint target = 1;
            uint i = Settings.Get<uint>("do_once");

            if (i != target)
                Settings.Set("do_once", target);

            if (i == 0 || !File.Exists(Settings.WebPath + "template.htm"))
            {
                FixTemplate();
            }
        }

        private static void FixTemplate()
        {
            if (File.Exists(Settings.WebPath + "template.htm"))
                try
                {
                    File.WriteAllBytes(Settings.WebPath + "template.broken", File.ReadAllBytes(Settings.WebPath + "template.htm"));
                }
                catch { }

            try
            {
                File.WriteAllBytes(Settings.WebPath + "template.htm", Resource1.template);
            }
            catch { }
        }
    }
}
