using System.Reflection;

namespace Reflx;

public interface IMemberHelper {
    TAttribute GetDecorator<TAttribute>(MemberInfo member);
}