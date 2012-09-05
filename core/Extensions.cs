using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    static class Extensions
    {
        public static void ForEachWhere<T>(this List<T> list, Action<T> @foreach, Predicate<T> @where)
        {
            for (int i = 0; i < list.Count; i++)
                if (@where(list[i]))
                    @foreach(list[i]);
        }
    }
}
