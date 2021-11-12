using System.Reflection;

namespace Reflx;

public class AssemblyHelper : IAssemblyHelper {
    readonly ITypeHelper typeHelper;

    public AssemblyHelper(ITypeHelper typeHelper) {
        this.typeHelper = typeHelper ?? throw new ArgumentException(nameof(typeHelper));
    }

    public TAttribute GetDecorator<TAttribute>(Assembly assembly) =>
        assembly == null ? default : assembly
            .GetCustomAttributes(typeof(TAttribute), false)
            .Cast<TAttribute>()
            .SingleOrDefault();

    public IEnumerable<TAttribute> GetDecorators<TAttribute>(IEnumerable<Assembly> assemblies) =>
        GetTypes(assemblies)
            .Select(typeHelper.GetDecorator<TAttribute>);

    public IEnumerable<Type> GetTypesDecoratedBy<TAttribute>(IEnumerable<Assembly> assemblies) =>
        typeHelper
            .GetTypesDecoratedBy<TAttribute>(
                GetTypes(assemblies));

    public Type GetSingleTypeDecoratedBy<TAttribute>(IEnumerable<Assembly> assemblies, Func<TAttribute, Boolean> predicate) =>
        typeHelper
            .GetSingleTypeDecoratedBy(
                GetTypes(assemblies), predicate);

    public IEnumerable<Type> GetTypesInheritedBy<TAncestor>(IEnumerable<Assembly> assemblies) =>
        GetTypes(assemblies, typeHelper
            .IsInheritedBy<TAncestor>);

    public Type GetSingleTypeInheritedBy<TAncestor>(IEnumerable<Assembly> assemblies) =>
        GetType(assemblies, typeHelper
            .IsInheritedBy<TAncestor>);
    public IEnumerable<Type> GetTypes(IEnumerable<Assembly> assemblies) =>
       assemblies?
           .SelectMany(asm => asm.GetTypes());

    public IEnumerable<Type> GetTypes(IEnumerable<Assembly> assemblies, Func<Type, Boolean> predicate) =>
        GetTypes(assemblies)
            .Where(predicate);

    public Type GetType(IEnumerable<Assembly> assemblies, Func<Type, Boolean> predicate) =>
        assemblies?
            .SelectMany(asm => asm.GetTypes())
            .FirstOrDefault(predicate);
}