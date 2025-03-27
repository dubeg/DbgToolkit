using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbgToolkit.Reflection;

namespace DbgToolkit.Collection {
    public static class ListExt {
        /// <summary>
        /// Transform list to array[items.Count, 1], 
        /// where each second dimension contains an item.
        /// </summary>
        public static object[,] ScalarListTo2DArray<T>(this List<T> items) {
            var arr = new object[items.Count, 1];
            for (int i = 0; i < items.Count; i++) {
                arr[i, 0] = items[i];
            }
            return arr;
        }

        /// <summary>
        /// Transform list to array[items.Count, typeof(item).Properties.Count],
        /// where each second dimension contains the properties values of an item.
        /// </summary>
        /// <remarks>
        /// Requires that the type has its properties annotated using the "Order" attribute 
        /// from this library.
        /// </remarks>
        public static object[,] ListTo2DArrayUsingDeclaredOrder<T>(this List<T> items) where T : class {
            var props = typeof(T).GetPropertiesInDeclaredOrder().ToArray();
            var arr = new object[items.Count, props.Length];
            for (int i = 0; i < items.Count; i++) {
                var item = items[i];
                for (int j = 0; j < props.Length; j++) {
                    arr[i, j] = props[j].GetValue(item);
                }
            }
            return arr;
        }
    }
}
