using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using DbgToolkit.Annotation;

namespace DbgToolkit.Reflection;

public static class Extensions {
    public static string GetResource(this Assembly assembly, string name) {
        // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
        var resourcePath = assembly.GetManifestResourceNames()
            .Single(str => str.EndsWith(name));
        using (var stream = assembly.GetManifestResourceStream(resourcePath))
        using (var reader = new StreamReader(stream)) {
            return reader.ReadToEnd();
        }
    }

    public static IEnumerable<PropertyInfo> GetPropertiesInDeclaredOrder(this Type type) {
        return
            from property in type.GetProperties()
            where property.IsDefined(typeof(IOrderAttribute), false)
            orderby (
                (IOrderAttribute)property
                .GetCustomAttributes(typeof(IOrderAttribute), false)
                .Single()
            ).Order
            select property
        ;
    }

    /// <summary>
    /// Requires that the type has its properties annotated using the "Order" attribute 
    /// from this library.
    /// </summary>
    public static IEnumerable<string> GetPropertyNamesInDeclaredOrder(this Type type) {
        return type.GetPropertiesInDeclaredOrder().Select(x => x.Name);
    }

    /// <summary>
    /// Requires that the type has its properties annotated using the "Order" attribute 
    /// from this library.
    /// </summary>
    public static IEnumerable<object> GetPropertyValuesInDeclaredOrder(this Type type, object obj) {
        return type.GetPropertiesInDeclaredOrder().Select(x => x.GetValue(obj));
    }

    //public static IEnumerable<T> GetCustomPropertyAttributesInDeclaredOrder<T>(this Type type) {
    //    return
    //        from property in type.GetPropertiesInDeclaredOrder()
    //        where Attribute.IsDefined(property, typeof(T))
    //        select (T)property
    //            .GetCustomAttributes(typeof(T), false)
    //            .Single()
    //    ;
    //}
}
