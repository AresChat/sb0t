using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.Extensions
{
    class ConcurrentList<T>
    {
        private List<T> Items { get; set; }
        private object Locker { get; set; }

        public ConcurrentList()
        {
            this.Items = new List<T>();
            this.Locker = new object();
        }

        public void Add(T obj)
        {
            lock (this.Locker)
                this.Items.Add(obj);
        }

        public void RemoveAll(Predicate<T> predicate)
        {
            lock (this.Locker)
                this.Items.RemoveAll(predicate);
        }

        public void ForEach(Action<T> action)
        {
            T[] copy;

            lock (this.Locker)
                copy = this.Items.ToArray();

            foreach (T obj in copy)
                action(obj);
        }
    }
}
