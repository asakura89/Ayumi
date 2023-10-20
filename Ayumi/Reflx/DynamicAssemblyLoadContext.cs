using System.Reflection;
using System.Runtime.Loader;

namespace Reflx;

public class DynamicAssemblyLoadContext : AssemblyLoadContext {
    readonly AssemblyDependencyResolver resolver;

    public DynamicAssemblyLoadContext(String asmPath) : this(GenerateNameByAssemblyPath(asmPath), asmPath) { }

    public DynamicAssemblyLoadContext(String loadContextName, String asmPath) : base(loadContextName) {
        resolver = new AssemblyDependencyResolver(asmPath);
    }

    public static String GenerateNameByAssemblyPath(String asmPath) => new DirectoryInfo(asmPath).Name;

    protected override Assembly? Load(AssemblyName assemblyName) {
        System.Diagnostics.Debug.WriteLine($"Context: {Name}, AssemblyName: {assemblyName}");

        try {
            // This is usually the plugin interface
            Assembly defaultContextAssembly = AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
            if (defaultContextAssembly != null)
                return defaultContextAssembly;

            System.Diagnostics.Debug.WriteLine($"DefaultContextAssembly is null {defaultContextAssembly != null}");
        }
        catch {
            String assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath == null)
                return null;

            return LoadFromAssemblyPath(assemblyPath);
        }

        return null;
    }

    protected override IntPtr LoadUnmanagedDll(String unmanagedDllName) {
        String unmanagedDllPath = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (unmanagedDllPath == null)
            return IntPtr.Zero;

        return LoadUnmanagedDllFromPath(unmanagedDllPath);
    }
}