using System;
using System.Collections;
using System.Collections.Generic;

namespace MatthiWare.CommandLine.Core
{
    internal class ReadOnlyCollectionWrapper<TKey, TValue> : IReadOnlyList<TValue>
    {
        private readonly Dictionary<TKey, TValue>.ValueCollection values;

        public ReadOnlyCollectionWrapper(Dictionary<TKey, TValue>.ValueCollection values)
        {
            this.values = values;
        }

        public TValue this[int index] => FindFirstIndex(GetEnumerator(), index);

        public int Count => values.Count;

        public IEnumerator<TValue> GetEnumerator() => values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private TValue FindFirstIndex(IEnumerator<TValue> enumerator, int index)
        {
            int i = 0;
            while (enumerator.MoveNext())
            {
                if (i == index)
                    return enumerator.Current;

                i++;
            }

            throw new IndexOutOfRangeException();
        }
    }
}
