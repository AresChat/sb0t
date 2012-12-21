using System;
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
