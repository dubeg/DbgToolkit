using DbgToolkit.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DbgToolkit.Excel {
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelColumnAttribute : Attribute, IOrderAttribute {
        public string Name { get; set; }
        public string Format { get; set; }
        public int Order { get; }
        public WrapTextType Wrap { get; }
        public int Width { get; } // 0: undefined

        public ExcelColumnAttribute(
            [CallerMemberName]string name = null,
            string format = null,
            [CallerLineNumber]int order = 0,
            WrapTextType wrap = WrapTextType.Undefined,
            int width = 0
            ) {
            Name = name;
            Format = format;
            Order = order;
            Wrap = wrap;
            Width = width;
        }
    }

    public enum WrapTextType { 
        Undefined,
        Wrap,
        NoWrap
    }
}
