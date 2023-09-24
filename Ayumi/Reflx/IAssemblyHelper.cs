using System.Reflection;

namespace Reflx;

public interface IAssemblyHelper {
    TAttribute GetDecorator<TAttribute>(Assembly assembly);
    IEnumerable<TAttribute> GetDecorators<TAttribute>(IEnumerable<Assembly> assemblies);
    Type GetSingleTypeDecoratedBy<TAttribute>(IEnumerable<Assembly> assemblies, Func<TAttribute, Boolean> predicate);
    Type GetSingleTypeInheritedBy<TAncestor>(IEnumerable<Assembly> assemblies);
    Type GetType(IEnumerable<Assembly> assemblies, Func<Type, Boolean> predicate);
    IEnumerable<Type> GetTypes(IEnumerable<Assembly> assemblies);
    IEnumerable<Type> GetTypes(IEnumerable<Assembly> assemblies, Func<Type, Boolean> predicate);
    IEnumerable<Type> GetTypesDecoratedBy<TAttribute>(IEnumerable<Assembly> assemblies);
    IEnumerable<Type> GetTypesInheritedBy<TAncestor>(IEnumerable<Assembly> assemblies);
}