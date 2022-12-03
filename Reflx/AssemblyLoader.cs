using System.Reflection;
using System.Runtime.Loader;

namespace Reflx;

public class AssemblyLoader : IAssemblyLoader {
    readonly Object assemblyFileLock = new Object();

    public void LoadFromPath(String path) =>
        LoadFromPath(path, new[] { "*" });

    public void LoadFromPath(String path, IEnumerable<String> assemblyNames) {
        if (String.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException(path);
        if (assemblyNames == null)
            throw new ArgumentNullException(nameof(assemblyNames));
        if (!assemblyNames.Any())
            throw new ArgumentOutOfRangeException(nameof(assemblyNames));

        if (assemblyNames.Any(ns => ns.Equals("*")))
            assemblyNames = Directory
                .GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly)
                .Select(ns => ns.Replace(".dll", String.Empty));

        IList<String> goodNamespaces = assemblyNames
            .Where(ns => !String.IsNullOrEmpty(ns))
            .ToList();

        if (!goodNamespaces.Any())
            throw new ArgumentOutOfRangeException(nameof(assemblyNames));

        IEnumerable<KeyValuePair<String, String>> nonExistents = goodNamespaces
            .Select(ns => Path.Combine(path, ns + ".dll"))
            .Select(ns => new KeyValuePair<String, String>(ns, File.Exists(ns).ToString()))
            .Where(asm => !Convert.ToBoolean(asm.Value));

        if (nonExistents.Any()) {
            String message = $"Below assemblies are nowhere to be found. {Environment.NewLine}{String.Join(Environment.NewLine, nonExistents.Select(item => item.Key))}";
            throw new FileNotFoundException(message);
        }

        String loadContextName = DynamicAssemblyLoadContext.GenerateNameByAssemblyPath(path);
        AssemblyLoadContext loadContext = AssemblyLoadContext.All.FirstOrDefault(ctx => ctx.Name == loadContextName);
        if (loadContext == null)
            loadContext = new DynamicAssemblyLoadContext(path);

        foreach (String ns in goodNamespaces) {
            String file = Path.Combine(path, ns + ".dll");
            loadContext.LoadFromAssemblyPath(file);
        }
    }
}