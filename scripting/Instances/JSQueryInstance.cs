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
using System.Data.SQLite;
using Jurassic;
using Jurassic.Library;

namespace scripting.Instances
{
    class JSQueryInstance : ObjectInstance
    {
        internal String query { get; private set; }
        internal List<SQLiteParameter> _params = new List<SQLiteParameter>();

        public JSQueryInstance(ObjectInstance prototype)
            : base(prototype)
        {
            this.PopulateFunctions();
            this.query = String.Empty;
        }

        protected override string InternalClassName
        {
            get { return "Query"; }
        }

        public JSQueryInstance(ObjectInstance prototype, String q, params object[] a)
            : base(prototype)
        {
            this.query = q;

            int counter = 0;

            for (int i = 0; i < a.Length; i++)
            {
                this.query = this.query.Replace("{" + counter + "}", "@var" + counter);

                if (a[i] is String || a[i] is ConcatenatedString)
                    this._params.Add(new SQLiteParameter(("@var" + (counter++)), a[i].ToString()));
                else if (a[i] is int || a[i] is double)
                {
                    int _i;

                    if (int.TryParse(a[i].ToString(), out _i))
                        this._params.Add(new SQLiteParameter(("@var" + (counter++)), _i));
                    else
                        this._params.Add(new SQLiteParameter(("@var" + (counter++)), double.Parse(a[i].ToString())));
                }
            }
        }
    }
}
