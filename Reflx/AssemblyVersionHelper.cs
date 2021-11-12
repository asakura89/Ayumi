using System.Reflection;

namespace Reflx;

public class AssemblyVersionHelper : IAssemblyVersionHelper {
    AssemblyVersionAttribute GetAsmVersion(Assembly asm) => asm
        .GetCustomAttributes(typeof(AssemblyVersionAttribute), false)
        .Cast<AssemblyVersionAttribute>()
        .SingleOrDefault();

    AssemblyFileVersionAttribute GetAsmFileVersion(Assembly asm) => asm
        .GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)
        .Cast<AssemblyFileVersionAttribute>()
        .SingleOrDefault();

    public String GetOrDefault(Assembly asm) {
        AssemblyVersionAttribute asmVersion = GetAsmVersion(asm);
        if (asmVersion != null)
            return asmVersion.Version;

        return "1.0.0.0";
    }

    public String GetFileVersionOrDefault(Assembly asm) {
        AssemblyFileVersionAttribute asmFileVersion = GetAsmFileVersion(asm);
        if (asmFileVersion != null)
            return asmFileVersion.Version;

        return "1.0.0.0";
    }

    public String GetOrFileVersionOrDefault(Assembly asm) {
        AssemblyVersionAttribute asmVersion = GetAsmVersion(asm);
        if (asmVersion != null)
            return asmVersion.Version;

        AssemblyFileVersionAttribute asmFileVersion = GetAsmFileVersion(asm);
        if (asmFileVersion != null)
            return asmFileVersion.Version;

        return "1.0.0.0";
    }
}