using System.Reflection;

namespace Reflx;

public class FieldHelper : IFieldHelper {
    public TAttribute GetDecorator<TAttribute>(FieldInfo field) =>
        field == null ? default : field
            .GetCustomAttributes(typeof(TAttribute), false)
            .Cast<TAttribute>()
            .SingleOrDefault();
}