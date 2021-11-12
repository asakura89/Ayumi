using System.Reflection;

namespace Reflx;

public class MemberHelper : IMemberHelper {
    public TAttribute GetDecorator<TAttribute>(MemberInfo member) =>
        member == null ? default(TAttribute) : member
            .GetCustomAttributes(typeof(TAttribute), false)
            .Cast<TAttribute>()
            .SingleOrDefault();
}