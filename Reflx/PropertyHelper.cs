using System.Reflection;

namespace Reflx;

public class PropertyHelper : IPropertyHelper {
    public TAttribute GetDecorator<TAttribute>(PropertyInfo property) =>
        property == null ? default : property
            .GetCustomAttributes(typeof(TAttribute), false)
            .Cast<TAttribute>()
            .SingleOrDefault();
}