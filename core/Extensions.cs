using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    static class Extensions
    {
        private static Random rnd = new Random();

        public static void ForEachWhere<T>(this List<T> list, Action<T> @foreach, Predicate<T> @where)
        {
            for (int i = 0; i < list.Count; i++)
                if (@where(list[i]))
                    @foreach(list[i]);
        }

        public static void Randomize<T>(this List<T> list)
        {
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}
