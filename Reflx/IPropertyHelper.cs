using System.Reflection;

namespace Reflx;

public interface IPropertyHelper {
    TAttribute GetDecorator<TAttribute>(PropertyInfo property);
}