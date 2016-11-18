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
    class JSListInstance : ObjectInstance
    {
        private object[] array { get; set; }

        protected override string InternalClassName
        {
            get { return "List"; }
        }

        public JSListInstance(ObjectInstance prototype, params object[] items)
            : base(prototype)
        {
            this.PopulateFunctions();
            this.array = new object[0];

            if (items.Length > 0)
                this.AddRange(items);
        }

        [JSProperty(Name = "count")]
        public int Count
        {
            get { return this.Length; }
            set { }
        }

        [JSProperty(Name = "length")]
        public int Length
        {
            get { return this.array.Length; }
            set { }
        }

        [JSFunction(Name = "clear", IsEnumerable = true, IsWritable = false)]
        public int Clear()
        {
            for (uint i = 0; i < this.array.Length; i++)
                this.Delete(i, true);

            this.array = new object[0];
            return this.array.Length;
        }

        [JSFunction(Name = "reverse", IsEnumerable = true, IsWritable = false)]
        public bool Reverse()
        {
            if (this.array.Length > 0)
            {
                Array.Reverse(this.array);

                for (uint i = 0; i < this.array.Length; i++)
                    this.SetPropertyValue(i, this.array[i], true);
            }

            return true;
        }

        [JSFunction(Name = "sort", IsEnumerable = true, IsWritable = false)]
        public bool Sort(object a)
        {
            if (this.array.Length < 2)
                return false;

            if (a is Undefined)
            {
                List<object> nocomp = this.array.ToList();
                nocomp.Sort();

                this.array = nocomp.ToArray();

                for (uint i = 0; i < this.array.Length; i++)
                    this.SetPropertyValue(i, this.array[i], true);

                return true;
            }
            
            if (!(a is UserDefinedFunction))
                throw new JavaScriptException(this.Engine, "invalid casting", "expecting comparison function");

            UserDefinedFunction f = (UserDefinedFunction)a;
            List<object> list = this.array.ToList();

            list.Sort((x, y) => TypeConverter.ToInt32(f.Call(this.Engine, x, y)));

            this.array = list.ToArray();

            for (uint i = 0; i < this.array.Length; i++)
                this.SetPropertyValue(i, this.array[i], true);

            return true;
        }

        [JSFunction(Name = "add", IsEnumerable = true, IsWritable = false)]
        public int Add(object a)
        {
            uint counter = (uint)this.array.Length;
            object[] tmp = new object[counter + 1];
            
            tmp[tmp.Length - 1] = a;
            this.SetPropertyValue(counter, a, true);

            Array.Copy(this.array, tmp, this.array.Length);

            this.array = tmp;
            return this.array.Length;
        }

        [JSFunction(Name = "addRange", IsEnumerable = true, IsWritable = false)]
        public int AddRange(params object[] a)
        {
            if (a.Length > 0)
            {
                uint counter = (uint)this.array.Length;
                object[] tmp = new object[counter + a.Length];

                for (int i = 0; i < a.Length; i++)
                {
                    tmp[tmp.Length - (a.Length - i)] = a[i];
                    this.SetPropertyValue(counter++, a[i], true);
                }

                Array.Copy(this.array, tmp, this.array.Length);

                this.array = tmp;
            }

            return this.array.Length;
        }

        [JSFunction(Name = "insert", IsEnumerable = true, IsWritable = false)]
        public int Insert(object ind, object a)
        {
            if (!(ind is int || ind is double))
                throw new JavaScriptException(this.Engine, "invalid casting", "index must be an int or double");

            int index = TypeConverter.ToInt32(ind);

            if (index > this.array.Length || index < 0)
                throw new JavaScriptException(this.Engine, "out of bounds", "index was out of bounds");

            object[] tmp = new object[this.array.Length + 1];
            tmp[index] = a;

            Array.Copy(this.array, 0, tmp, 0, index);
            Array.Copy(this.array, index, tmp, (index + 1), (this.array.Length - index));

            this.array = tmp;

            for (uint i = 0; i < this.array.Length; i++)
                this.SetPropertyValue(i, this.array[i], true);

            return this.array.Length;
        }

        [JSFunction(Name = "insertRange", IsEnumerable = true, IsWritable = false)]
        public int InsertRange(object ind, params object[] a)
        {
            if (!(ind is int || ind is double))
                throw new JavaScriptException(this.Engine, "invalid casting", "index must be an int or double");

            int index = TypeConverter.ToInt32(ind);

            if (index > this.array.Length || index < 0)
                throw new JavaScriptException(this.Engine, "out of bounds", "index was out of bounds");

            object[] tmp = new object[this.array.Length + a.Length];

            for (int i = 0; i < a.Length; i++)
                tmp[index + i] = a[i];

            Array.Copy(this.array, 0, tmp, 0, index);
            Array.Copy(this.array, index, tmp, (index + a.Length), (this.array.Length - index));

            this.array = tmp;

            for (uint i = 0; i < this.array.Length; i++)
                this.SetPropertyValue(i, this.array[i], true);

            return this.array.Length;
        }

        [JSFunction(Name = "remove", IsEnumerable = true, IsWritable = false)]
        public int Remove(object a)
        {
            bool found = false;

            for (uint i = 0; i < this.array.Length; i++)
            {
                if (!found)
                {
                    if (this.array[i].Equals(a))
                    {
                        found = true;

                        if (i < (this.array.Length - 1))
                        {
                            this.array[i] = this.array[i + 1];
                            this.SetPropertyValue(i, this.array[i], true);
                        }
                    }
                }
                else if (i < (this.array.Length - 1))
                {
                    this.array[i] = this.array[i + 1];
                    this.SetPropertyValue(i, this.array[i], true);
                }
                else this.Delete(i, true);
            }

            if (found)
            {
                object[] tmp = new object[this.array.Length - 1];
                Array.Copy(this.array, tmp, tmp.Length);
                this.array = tmp;
            }

            return this.array.Length;
        }

        [JSFunction(Name = "removeRange", IsEnumerable = true, IsWritable = false)]
        public int RemoveRange(object st, object co)
        {
            if (!(st is int || st is double))
                throw new JavaScriptException(this.Engine, "invalid casting", "start must be an int or double");

            int start = TypeConverter.ToInt32(st);

            if (!(co is int || co is double))
                throw new JavaScriptException(this.Engine, "invalid casting", "count must be an int or double");

            int count = TypeConverter.ToInt32(co);

            if (start > this.array.Length || start < 0)
                throw new JavaScriptException(this.Engine, "out of bounds", "start was out of bounds");

            if ((start + count) > this.array.Length || count < start)
                throw new JavaScriptException(this.Engine, "out of bounds", "count was out of bounds");

            object[] tmp = new object[this.array.Length - count];

            for (uint i = 0; i < this.array.Length; i++)
            {
                if (i < start)
                    tmp[i] = this.array[i];
                else if (i >= (start + count))
                {
                    tmp[i - count] = this.array[i];
                    this.SetPropertyValue((uint)(i - count), this.array[i], true);
                }

                if (i >= tmp.Length)
                    this.Delete(i, true);
            }

            this.array = tmp;

            return this.array.Length;
        }

        [JSFunction(Name = "removeAt", IsEnumerable = true, IsWritable = false)]
        public int RemoveAt(object a)
        {
            if (!(a is int || a is double))
                throw new JavaScriptException(this.Engine, "invalid casting", "index must be an int or double");

            int index = TypeConverter.ToInt32(a);

            if (index >= this.array.Length || index < 0)
                throw new JavaScriptException(this.Engine, "out of bounds", "start was out of bounds");

            object[] tmp = new object[this.array.Length - 1];

            for (uint i = 0; i < this.array.Length; i++)
            {
                if (i < index)
                    tmp[i] = this.array[i];
                else if (i > index)
                {
                    tmp[i - 1] = this.array[i];
                    this.SetPropertyValue((i - 1), this.array[i], true);
                }
            }

            this.Delete((uint)(this.array.Length - 1), true);
            this.array = tmp;

            return this.array.Length;
        }

        [JSFunction(Name = "removeAll", IsEnumerable = true, IsWritable = false)]
        public int RemoveAll(object a)
        {
            if (!(a is UserDefinedFunction))
                throw new JavaScriptException(this.Engine, "invalid casting", "expecting comparison function");

            UserDefinedFunction f = (UserDefinedFunction)a;
            List<object> list = new List<object>();

            foreach (object obj in this.array)
                if (!TypeConverter.ToBoolean(f.Call(this.Engine.Global, obj)))
                    list.Add(obj);

            int current = this.array.Length;
            this.array = new object[list.Count];

            for (uint i = 0; i < current; i++)
                if (i < this.array.Length)
                {
                    this.array[i] = list[(int)i];
                    this.SetPropertyValue(i, this.array[i], true);
                }
                else this.Delete(i, true);

            return current - this.array.Length;
        }

        [JSFunction(Name = "getRange", IsEnumerable = true, IsWritable = false)]
        public JSListInstance GetRange(object st, object co)
        {
            if (!(st is int || st is double))
                throw new JavaScriptException(this.Engine, "invalid casting", "start must be an int or double");

            int start = TypeConverter.ToInt32(st);

            if (!(co is int || co is double))
                throw new JavaScriptException(this.Engine, "invalid casting", "count must be an int or double");

            int count = TypeConverter.ToInt32(co);

            if (start > this.array.Length || start < 0)
                throw new JavaScriptException(this.Engine, "out of bounds", "start was out of bounds");

            if ((start + count) > this.array.Length || count < start)
                throw new JavaScriptException(this.Engine, "out of bounds", "count was out of bounds");

            JSListInstance result = new JSListInstance(this.Engine.Object.InstancePrototype);

            for (int i = 0; i < this.array.Length; i++)
                if (i >= start && i <= (start + count))
                    result.Add(this.array[i]);
            
            return result;
        }

        [JSFunction(Name = "indexOf", IsEnumerable = true, IsWritable = false)]
        public int IndexOf(object a)
        {
            for (int i = 0; i < this.array.Length; i++)
                if (this.array[i].Equals(a))
                    return i;

            return -1;
        }

        [JSFunction(Name = "lastIndexOf", IsEnumerable = true, IsWritable = false)]
        public int LastIndexOf(object a)
        {
            for (int i = (this.array.Length - 1); i > -1; i--)
                if (this.array[i].Equals(a))
                    return i;

            return -1;
        }

        [JSFunction(Name = "findAll", IsEnumerable = true, IsWritable = false)]
        public JSListInstance FindAll(object a)
        {
            if (!(a is UserDefinedFunction))
                throw new JavaScriptException(this.Engine, "invalid casting", "expecting comparison function");

            UserDefinedFunction f = (UserDefinedFunction)a;
            List<object> list = new List<object>();

            foreach (object obj in this.array)
                if (TypeConverter.ToBoolean(f.Call(this.Engine.Global, obj)))
                    list.Add(obj);

            JSListInstance result = new JSListInstance(this.Engine.Object.InstancePrototype);

            foreach (object obj in list)
                result.Add(obj);

            return result;
        }

        [JSFunction(Name = "find", IsEnumerable = true, IsWritable = false)]
        public object Find(object a)
        {
            if (!(a is UserDefinedFunction))
                throw new JavaScriptException(this.Engine, "invalid casting", "expecting comparison function");

            UserDefinedFunction f = (UserDefinedFunction)a;

            foreach (object obj in this.array)
                if (TypeConverter.ToBoolean(f.Call(this.Engine.Global, obj)))
                    return obj;

            return null;
        }

        [JSFunction(Name = "findIndex", IsEnumerable = true, IsWritable = false)]
        public int FindIndex(object a)
        {
            if (!(a is UserDefinedFunction))
                throw new JavaScriptException(this.Engine, "invalid casting", "expecting comparison function");

            UserDefinedFunction f = (UserDefinedFunction)a;

            for (int i = 0; i < this.array.Length; i++)
                if (TypeConverter.ToBoolean(f.Call(this.Engine.Global, this.array[i])))
                    return i;

            return -1;
        }

        [JSFunction(Name = "findLastIndex", IsEnumerable = true, IsWritable = false)]
        public int FindLastIndex(object a)
        {
            if (!(a is UserDefinedFunction))
                throw new JavaScriptException(this.Engine, "invalid casting", "expecting comparison function");

            UserDefinedFunction f = (UserDefinedFunction)a;

            for (int i = (this.array.Length - 1); i > -1; i--)
                if (TypeConverter.ToBoolean(f.Call(this.Engine.Global, this.array[i])))
                    return i;

            return -1;
        }

        [JSFunction(Name = "join", IsEnumerable = true, IsWritable = false)]
        public String Join(object a)
        {
            String s = a.ToString();
            List<String> list = new List<String>();

            foreach (object obj in this.array)
                list.Add(TypeConverter.ToString(obj));

            return String.Join(s, list.ToArray());
        }

        public override string ToString()
        {
            List<String> list = new List<String>();

            foreach (object obj in this.array)
                list.Add(TypeConverter.ToString(obj));

            return String.Join(",", list.ToArray());
        }
    }
}
