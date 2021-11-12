using System.Reflection;

namespace Reflx;

public class AssemblyLoader : IAssemblyLoader {
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

        // https://stackoverflow.com/questions/9315716/side-effects-of-calling-assembly-load-multiple-times
        foreach (String ns in goodNamespaces) {
            try {
                String file = Path.Combine(path, ns + ".dll");
                var asmName = AssemblyName.GetAssemblyName(file);
                Assembly asm = AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .FirstOrDefault(a => a.FullName == asmName.FullName);

                if (asm == null)
                    Assembly.Load(File.ReadAllBytes(file));
            }
            catch {
                //
            }
        }
    }
}