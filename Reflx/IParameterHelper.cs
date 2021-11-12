using System.Reflection;

namespace Reflx;

public interface IParameterHelper {
    TAttribute GetDecorator<TAttribute>(ParameterInfo parameter);
}