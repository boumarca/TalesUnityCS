using System;
using System.Collections.Generic;

namespace Framework.Extensions
{
    public static class ReadOnlyListExtensions
    {
        public static bool IsIndexInRange<TSource>(this IReadOnlyList<TSource> source, int index)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return index >= 0 && index < source.Count;
        }

        public static int IndexOf<TSource>(this IReadOnlyList<TSource> source, TSource item)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            for (int i = 0; i < source.Count; i++) 
            {
                if (EqualityComparer<TSource>.Default.Equals(source[i], item))
                    return i;
            }
            return -1;
        }
    }
}
