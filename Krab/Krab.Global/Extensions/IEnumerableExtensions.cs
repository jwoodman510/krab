using System;
using System.Collections.Generic;
using System.Linq;

namespace Krab.Global.Extensions
{
    public static class EnumerableExtensions
    {
        public static IList<T> ToSafeList<T>(this IEnumerable<T> source)
        {
            return source?.ToList() ?? new List<T>();
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
                return;

            foreach (var item in source)
                action?.Invoke(item);
        }
    }
}
