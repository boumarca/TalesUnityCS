using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsEmpty<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return !source.Any();
        }

        public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return !source.Any(predicate);
        }
    }
}
