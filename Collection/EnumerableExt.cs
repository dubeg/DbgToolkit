using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbgToolkit.Collection {
    public static class EnumerableExt {
        ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate) {
            if (items == null) throw new ArgumentNullException("items");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int retVal = 0;
            foreach (var item in items) {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }

        ///<summary>Finds the one-based index of the first item matching an expression in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The one-based index of the first matching item, or 0 if no items match.</returns>
        public static int FindIndexOneBased<T>(this IEnumerable<T> items, Func<T, bool> predicate) {
            var result = FindIndex(items, predicate);
            return result + 1;
        }

        ///<summary>Finds the index of the first occurrence of an item in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int IndexOf<T>(this IEnumerable<T> items, T item) =>
            items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));

        /// <summary>
        /// Wraps this object instance into an IEnumerable&lt;T&gt;
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="item"> The instance that will be wrapped. </param>
        /// <returns> An IEnumerable&lt;T&gt; consisting of a single item. </returns>
        public static IEnumerable<T> Yield<T>(this T item) {
            yield return item;
        }

        /// <summary>
        /// Process enumerable in chunks
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize) {
            var pos = 0;
            while (source.Skip(pos).Any()) {
                yield return source.Skip(pos).Take(chunksize);
                pos += chunksize;
            }
        }
    }
}
