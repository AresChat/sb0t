using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;
using iconnect;

namespace scripting.Statics
{
    class JSGlobal
    {
        public JSScript parent { get; set; }

        public JSGlobal(JSScript script)
        {
            this.parent = script;
        }

        public int ByteLength(object a)
        {
            if (a is Undefined)
                return -1;

            return Encoding.UTF8.GetByteCount(a.ToString());
        }

        public String ClrName(object a)
        {
            if (a is Undefined)
                return null;

            return a.GetType().ToString();
        }

        public Objects.JSUser User(object a) // name/id
        {
            if (a is Null)
                return null;

            Objects.JSUser result = null;

            if (a is String || a is ConcatenatedString)
            {
                if (a.ToString().Length < 2)
                    return null;

                result = this.parent.local_users.Find(x => x.Name == ((String)a));

                if (result == null)
                    result = this.parent.local_users.Find(x => x.Name.StartsWith((String)a));
            }
            else if (a is int || a is double)
            {
                int _i;

                if (int.TryParse(a.ToString(), out _i))
                    result = this.parent.local_users.Find(x => x.Id == _i);
            }

            return result;
        }

        public void Print(object a, object b)
        {
            if (b is Undefined) // to all local users
            {
                String result = null;

                if (!(a is Null || a is Undefined))
                    if (a is bool)
                        result = ((bool)a).ToString().ToLower();
                    else if (a != null)
                        result = a.ToString();

                if (result != null)
                    Server.Print(result);
            }
            else if (a is int || a is double) // to vroom
            {
                int _i;

                if (int.TryParse(a.ToString(), out _i))
                {
                    if (_i >= 0 && _i <= 65535)
                    {
                        String result = null;

                        if (!(b is Null || b is Undefined))
                            if (b is bool)
                                result = ((bool)b).ToString().ToLower();
                            else if (b != null)
                                result = b.ToString();

                        if (result != null)
                            Server.Print((ushort)_i, result);
                    }
                }
            }
            else if (a is Objects.JSUser) // to user
            {
                Objects.JSUser u = (Objects.JSUser)a;

                if (u != null)
                {
                    String result = null;

                    if (!(b is Null || b is Undefined))
                        if (b is bool)
                            result = ((bool)b).ToString().ToLower();
                        else if (b != null)
                            result = b.ToString();

                    if (result != null)
                        u.parent.Print(result);
                }
            }
        }

        public void SendPM(object a, object b, object c)
        {
            if (!(a is Objects.JSUser))
                return;

            if (b is Null || c is Null)
                return;

            if (b != null && c != null)
            {
                String sender = b.ToString();
                String text = c.ToString();

                if (String.IsNullOrEmpty(sender) || String.IsNullOrEmpty(text))
                    return;

                ((Objects.JSUser)a).parent.PM(sender, text);
            }
        }
    }
}
