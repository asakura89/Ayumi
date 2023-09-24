using System.Reflection;

namespace Reflx;

public class ParameterHelper : IParameterHelper {
    public TAttribute GetDecorator<TAttribute>(ParameterInfo parameter) =>
        parameter == null ? default : parameter
            .GetCustomAttributes(typeof(TAttribute), false)
            .Cast<TAttribute>()
            .SingleOrDefault();
}