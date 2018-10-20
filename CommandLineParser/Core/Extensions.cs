using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.CommandLine.Core
{
    internal static class Extensions
    {
        public static T GetAndRemove<T>(this IList<T> src, int index)
        {
            if (index < 0 || index > src.Count) return default(T);

            T item = src[index];

            src.RemoveAt(index);

            return item;
        }

        public static ReadOnlySpan<char> Substring(this ReadOnlySpan<char> src, int start, int len)
        {
            if (src.IsEmpty) return src;

            return src.Slice(start, len);
        }

    }
}
