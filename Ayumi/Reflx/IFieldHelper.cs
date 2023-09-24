using System.Reflection;

namespace Reflx;

public interface IFieldHelper {
    TAttribute GetDecorator<TAttribute>(FieldInfo field);
}