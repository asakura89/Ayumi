namespace Reflx;

public interface ITypeHelper {
    TAttribute GetDecorator<TAttribute>(Type type);
    IEnumerable<TAttribute> GetDecorators<TAttribute>(IEnumerable<Type> types);
    Type GetSingleTypeDecoratedBy<TAttribute>(IEnumerable<Type> types, Func<TAttribute, Boolean> predicate);
    Type GetSingleTypeInheritedBy<TAncestor>(IEnumerable<Type> types);
    IEnumerable<Type> GetTypesDecoratedBy<TAttribute>(IEnumerable<Type> types);
    IEnumerable<Type> GetTypesInheritedBy<TAncestor>(IEnumerable<Type> types);
    Boolean IsDecoratedBy<TAttribute>(Type type);
    Boolean IsInheritedBy<TAncestor>(Type type);
}