namespace Reflx;

public interface IAssemblyLoader {
    void LoadFromPath(String path);
    void LoadFromPath(String path, IEnumerable<String> assemblyNames);
}