using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbgToolkit;
public static class ObjectExtensions {
    public static bool In<T>(this T obj, params T[] args) {
        return args.Contains(obj);
    }
}
