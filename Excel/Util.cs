using DbgToolkit.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DbgToolkit.Casting;

namespace DbgToolkit.Excel {
    public static class Util {
        /// <summary>
        /// Get range as objects, assuming the objects' properties are declared in the order
        /// they appear in the range. Properties must be annotated with the "Order" attribute to guarantee
        /// proper functioning of this method.
        /// </summary>
        public static List<T> ArrayToObjectListUsingDeclaredOrder<T>(object[,] values) where T : new() {
            var firstRow = values.GetLowerBound(0); var lastRow = values.GetUpperBound(0);
            var firstCol = values.GetLowerBound(1); var lastCol = values.GetUpperBound(1);
            var colCount = values.GetLength(1);
            var objects = new List<T>();
            var type = typeof(T);
            var props = type.GetPropertiesInDeclaredOrder().ToList();
            if (colCount != props.Count)
                throw new Exception($"{type.AssemblyQualifiedName} doesn't declare as many properties as present in the Excel range.");
            for (int r = firstRow; r <= lastRow; r++) {
                var o = new T(); objects.Add(o);
                var propEnumerator = props.GetEnumerator();
                for (int c = firstCol; c <= lastCol; c++) {
                    propEnumerator.MoveNext();
                    var prop = propEnumerator.Current;
                    var value = values[r, c];
                    if (value == null) {
                        if (!prop.PropertyType.IsValueType)
                            prop.SetValue(o, null);
                    }
                    else if (prop.PropertyType == typeof(Int32)) {
                        prop.SetValue(o, Convert.ToInt32(value));
                    }
                    else if (prop.PropertyType == typeof(double)) {
                        prop.SetValue(o, value);
                    }
                    else if (prop.PropertyType == typeof(DateTime)) {
                        prop.SetValue(o, value);
                    }
                    else if (prop.PropertyType == typeof(string)) {
                        prop.SetValue(o, Convert.ToString(value));
                    }
                    else if (prop.PropertyType == typeof(decimal)) {
                        var valueType = value.GetType();
                        if (valueType != typeof(double))
                            throw new Exception($"{type.AssemblyQualifiedName}.{prop.Name} is decimal, while value from Range is {valueType}.");
                        prop.SetValue(o, Convert.ToDecimal(value));
                    }
                    else if (prop.PropertyType.IsEnum) {
                        if (value is int intValue) {
                            prop.SetValue(o, Enum.ToObject(prop.PropertyType, intValue));
                        }
                        else {
                            throw new Exception($"{type.AssemblyQualifiedName}.{prop.Name} is an enum, but the value from Range is not an integer.");
                        }
                    }
                    else {
                        prop.SetValue(o, value);
                    }
                }
            }
            return objects;
        }

        public static List<T> ArrayToScalarList<T>(object[,] values) {
            var type = typeof(T);
            if (!type.IsSimpleType())
                throw new NotImplementedException("Complex struct aren't supported.");
            var results = new List<T>();
            var dim1_LowerBound = values.GetLowerBound(0);
            var dim1_higherBound = values.GetUpperBound(0);
            var dim2_lowerBound2 = values.GetLowerBound(1);
            for (int i = dim1_LowerBound; i <= dim1_higherBound; i++) {
                results.Add((T)values.GetValue(i, dim2_lowerBound2));
            }
            return results;
        }

        public static List<string> ArrayToStringList(object[,] values) {
            var results = new List<string>();
            var dim1_LowerBound = values.GetLowerBound(0);
            var dim1_higherBound = values.GetUpperBound(0);
            var dim2_lowerBound2 = values.GetLowerBound(1);
            for (int i = dim1_LowerBound; i <= dim1_higherBound; i++) {
                results.Add(values.GetValue(i, dim2_lowerBound2)?.ToString() ?? "");
            }
            return results;
        }


        public static bool IsSimpleType(this Type type) {
            return type.IsPrimitive
                || type.IsEnum
                || type.Equals(typeof(string))
                || type.Equals(typeof(decimal));
        }
    }
}
