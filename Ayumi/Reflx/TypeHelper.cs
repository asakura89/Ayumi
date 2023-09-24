namespace Reflx;

public class TypeHelper : ITypeHelper {
    public TAttribute GetDecorator<TAttribute>(Type type) =>
        type == null ? default : type
            .GetCustomAttributes(typeof(TAttribute), false)
            .Cast<TAttribute>()
            .SingleOrDefault();

    public IEnumerable<TAttribute> GetDecorators<TAttribute>(IEnumerable<Type> types) =>
        types?
            .SelectMany(type => type.GetCustomAttributes(false))
            .Cast<TAttribute>();

    public Boolean IsDecoratedBy<TAttribute>(Type type) =>
        type
            .GetCustomAttributes(typeof(TAttribute), false)
            .Any();

    public IEnumerable<Type> GetTypesDecoratedBy<TAttribute>(IEnumerable<Type> types) =>
        types?.Where(IsDecoratedBy<TAttribute>);

    public Type GetSingleTypeDecoratedBy<TAttribute>(IEnumerable<Type> types, Func<TAttribute, Boolean> predicate) =>
        types?.FirstOrDefault(type => predicate(GetDecorator<TAttribute>(type)));

    public Boolean IsInheritedBy<TAncestor>(Type type) =>
        type != null && typeof(TAncestor).IsAssignableFrom(type);

    public IEnumerable<Type> GetTypesInheritedBy<TAncestor>(IEnumerable<Type> types) =>
        types?.Where(IsInheritedBy<TAncestor>);

    public Type GetSingleTypeInheritedBy<TAncestor>(IEnumerable<Type> types) =>
        types?.FirstOrDefault(IsInheritedBy<TAncestor>);
}