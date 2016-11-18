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
using System.Data;
using System.Data.SQLite;
using Jurassic;
using Jurassic.Library;

namespace scripting.Instances
{
    class JSSqlInstance : ObjectInstance, IDisposable
    {
        private String m_last_error = String.Empty;

        public JSSqlInstance(ObjectInstance prototype)
            : base(prototype)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Sql"; }
        }

        private String ConnectionString { get; set; }
        private SQLiteConnection Connection { get; set; }
        private SQLiteDataReader Reader { get; set; }

        private static String[] bad_chars = new String[]
        {
            "/",
            "\\",
            " ",
        };

        [JSProperty(Name = "lastError")]
        public String LastError
        {
            get { return this.m_last_error.Replace("SQLite error\r\n", String.Empty); }
            set { }
        }

        [JSProperty(Name = "connected")]
        public bool Connected
        {
            get
            {
                if (this.Connection != null)
                    return this.Connection.State == ConnectionState.Open;

                return false;
            }
            set { }
        }

        [JSProperty(Name = "read")]
        public bool CanRead
        {
            get
            {
                if (this.Reader == null)
                    return false;

                if (!this.Reader.Read())
                    return false;

                return true;
            }
        }

        [JSFunction(Name = "value", IsWritable = false, IsEnumerable = true)]
        public String GetValue(object a)
        {
            this.m_last_error = String.Empty;

            if (a != null)
            {
                if (this.Reader == null)
                {
                    this.m_last_error = "no data available";
                    return null;
                }

                try
                {
                    return this.Reader[a.ToString()].ToString();
                }
                catch (Exception e)
                {
                    this.m_last_error = e.Message;
                    return null;
                }
            }

            this.m_last_error = "invalid item name";
            return null;
        }

        [JSFunction(Name = "query", IsWritable = false, IsEnumerable = true)]
        public bool GetQuery(JSQueryInstance query)
        {
            this.m_last_error = String.Empty;

            try
            {
                if (this.Reader != null)
                {
                    this.Reader.Close();
                    this.Reader.Dispose();
                    this.Reader = null;
                }
            }
            catch { }

            if (!this.Connected)
            {
                this.m_last_error = "connection closed";
                return false;
            }

            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(query.query, this.Connection))
                {
                    for (int i = 0; i < query._params.Count; i++)
                        cmd.Parameters.Add(query._params[i]);

                    this.Reader = cmd.ExecuteReader();
                }
            }
            catch (Exception e)
            {
                this.m_last_error = e.Message;
                return false;
            }

            return true;
        }

        [JSFunction(Name = "open", IsWritable = false, IsEnumerable = true)]
        public bool Open(object a)
        {
            if (a == null)
                return false;

            this.LastError = String.Empty;
            String file = a.ToString();

            if (file.Length > 1)
                if (bad_chars.Count<String>(x => file.Contains(x)) == 0)
                {
                    String path = Path.Combine(Server.DataPath, this.Engine.ScriptName, "sql");

                    try
                    {
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        path = Path.Combine(path, file);
                        this.ConnectionString = "Data Source=\"" + path + "\"";

                        if (!File.Exists(path))
                            SQLiteConnection.CreateFile(path);
                    }
                    catch { return false; }

                    try
                    {
                        if (this.Reader != null)
                        {
                            this.Reader.Close();
                            this.Reader.Dispose();
                            this.Reader = null;
                        }
                    }
                    catch { }

                    try
                    {
                        if (this.Connection == null)
                            this.Connection = new SQLiteConnection(this.ConnectionString);

                        if (this.Connection.State != ConnectionState.Open)
                            this.Connection.Open();

                        return this.Connection.State == ConnectionState.Open;
                    }
                    catch { }
                }

            return false;
        }

        [JSFunction(Name = "close", IsWritable = false, IsEnumerable = true)]
        public bool Close()
        {
            this.m_last_error = String.Empty;
            bool success = true;

            try
            {
                if (this.Connection != null)
                {
                    this.Connection.Close();
                    this.Connection.Dispose();
                    this.Connection = null;
                }
            }
            catch { success = false; }

            try
            {
                if (this.Reader != null)
                {
                    this.Reader.Close();
                    this.Reader.Dispose();
                    this.Reader = null;
                }
            }
            catch { }

            return success;
        }

        public void Dispose()
        {
            try
            {
                if (this.Connection != null)
                {
                    this.Connection.Close();
                    this.Connection.Dispose();
                    this.Connection = null;
                }
            }
            catch { }

            try
            {
                if (this.Reader != null)
                {
                    this.Reader.Close();
                    this.Reader.Dispose();
                    this.Reader = null;
                }
            }
            catch { }
        }
    }
}
