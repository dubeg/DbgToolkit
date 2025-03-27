using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DbgToolkit.Annotation {
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class OrderAttribute : Attribute, IOrderAttribute {
        public int Order { get; }
        public OrderAttribute([CallerLineNumber]int order = 0) {
            Order = order;
        }
    }
}
