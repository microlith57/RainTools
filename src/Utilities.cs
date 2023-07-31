using System;
using System.Collections.Generic;

namespace Celeste.Mod.RainTools {
    public static class ListExt {
        // from https://stackoverflow.com/a/22801345
        public static void AddSorted<T>(this List<T> self, T item, IComparer<T> comparer) {
            if (self.Count == 0) {
                self.Add(item);
                return;
            }
            if (comparer.Compare(self[self.Count - 1], item) <= 0) {
                self.Add(item);
                return;
            }
            if (comparer.Compare(self[0], item) >= 0) {
                self.Insert(0, item);
                return;
            }
            int index = self.BinarySearch(item, comparer);
            if (index < 0)
                index = ~index;
            self.Insert(index, item);
        }
    }

    public class KVPComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>> where TKey : IComparable<TKey> {
        public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y) {
            return x.Key.CompareTo(y.Key);
        }
    }
}
