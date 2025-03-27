using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbgToolkit.Annotation {
    /// <summary>
    /// Interface specifying the custom attribute has an Order field
    /// whose value is set by the CallerLineNumberAttribute.
    /// </summary>
    public interface IOrderAttribute {
        int Order { get; }
    }
}
