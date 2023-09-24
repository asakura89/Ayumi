using System.Reflection;

namespace Reflx;

public interface IAssemblyVersionHelper {
    String GetOrDefault(Assembly asm);

    String GetFileVersionOrDefault(Assembly asm);

    String GetOrFileVersionOrDefault(Assembly asm);
}