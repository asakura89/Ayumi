using System.Reflection;

namespace Reflx;

public interface IDefaultAssemblyResolver {
    Assembly Resolve(Object sender, ResolveEventArgs args);
}