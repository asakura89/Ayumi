namespace Reflx;

public interface IAssemblyLoader {
    void LoadFromPath(System.String path);
    void LoadFromPath(System.String path, IEnumerable<System.String> assemblyNames);
}