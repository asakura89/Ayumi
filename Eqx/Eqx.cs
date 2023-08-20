using System.Reflection;
using System.Text;
using Exy;

namespace Eqx;

public static class EqxLoader {
    static class EmbeddedResourceHandler {
        public static String GetTextFile(Assembly asm, String filename) {
            using Stream stream = asm.GetManifestResourceStream(filename);
            if (stream == null)
                return null;

            using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
        }
    }

    public static String Load<T>(T caller, String filename) => Load($"{caller.GetType().Name}/{filename}");

    public static String Load(String filename) {
        String rootDir = Path.GetDirectoryName(new Uri(Assembly.GetCallingAssembly().Location).LocalPath);
        String combined = null;
        if (filename.Contains('/')) {
            String[] splitted = filename.Split('/');
            combined = splitted.Aggregate("Eqx", Path.Combine);
        }

        if (String.IsNullOrEmpty(rootDir) || !Directory.Exists(rootDir))
            throw new UnintendedBehaviorException("Invalid eqx file.");

        String eqxPath = Path.Combine(rootDir, combined ?? filename) + ".eqx";
        if (!File.Exists(eqxPath))
            throw new UnintendedBehaviorException("Invalid eqx file.");

        using var stream = new FileStream(eqxPath, FileMode.Open);
            using var streamR = new StreamReader(stream, Encoding.UTF8);
                return streamR.ReadToEnd();
    }

    public static String LoadEmbedded<T>(T caller, String filename) {
        Assembly asm = caller.GetType().Assembly;
        return EmbeddedResourceHandler
            .GetTextFile(asm, $"{asm.GetName().Name}.Eqx.{caller.GetType().Name}.{filename}.eqx");
    }
}