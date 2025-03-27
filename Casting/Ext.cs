using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbgToolkit.Casting {
    public static class Ext {
        public static T CastTo<T>(this object o) => (T)o;
        public static T As<T>(this object o) where T : class => o as T;
    }
}
