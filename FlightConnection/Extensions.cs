using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightConnection
{
    public static class Extensions
    {
        public static IEnumerable<T> Cycle<T>(this IEnumerable<T> sequence) {
            while (true) {
                foreach (var element in sequence) {
                    yield return element;
                }
            }
        }

        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> sequence, int count) {
            if (count <= 0) {
                throw new ArgumentException(String.Format("invalid repeat time ({0})", count));
            }

            foreach (int ignored in Enumerable.Range(0, count)) {
                foreach (var element in sequence) {
                    yield return element;
                }
            }
        }

        public static IEnumerable<T> CumulativeSum<T>(this IEnumerable<T> sequence, T init, Func<T, T, T> func) {
            T sum = init;
            foreach (var element in sequence) {
                sum = func(sum, element);
                yield return sum;
            }
        }

        public static int FindIndex<T>(this IReadOnlyList<T> list, int startIndex, Predicate<T> match) {
            if (startIndex < 0 || list.Count <= startIndex) {
                throw new ArgumentException(
                    String.Format("invalid start index ({0}) for list which have {1} elements", startIndex, list.Count));
            }

            foreach (int index in Enumerable.Range(startIndex, list.Count - startIndex)) {
                if (match(list[index])) {
                    return index;
                }
            }

            return -1;
        }

        public static IEnumerable<T> From<T>(this IReadOnlyList<T> list, int startIndex) {
            if (startIndex < 0 || list.Count <= startIndex) {
                throw new ArgumentException(
                    String.Format("invalid start index ({0}) for list which have {1} elements", startIndex, list.Count));
            }

            foreach (int index in Enumerable.Range(startIndex, list.Count - startIndex)) {
                yield return list[index];
            }
        }

        public static IEnumerable<T> Yield<T>(this T item) {
            yield return item;
        }

        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source) {
            return source.Select((item, index) => (item, index));
        }
    }
}
