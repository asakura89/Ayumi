using System.Text.Json;

namespace Scratch;

public abstract class RunnableBase {
    protected readonly String DataDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");

    protected String GetDataPath(String filename) {
        if (!Directory.Exists(DataDirPath))
            Directory.CreateDirectory(DataDirPath);

        return Path.Combine(DataDirPath, filename);
    }

    protected readonly String OutputDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Temp");

    protected String GetOutputPath(String filename) {
        if (!Directory.Exists(OutputDirPath))
            Directory.CreateDirectory(OutputDirPath);

        return Path.Combine(OutputDirPath, filename);
    }

    protected void DisplayTitle(String title) {
        if (!String.IsNullOrEmpty(title)) {
            Console.WriteLine(title);
            Console.WriteLine(
                String.Join(
                    String.Empty,
                    Enumerable.Repeat("-", title.Length)
                ));
        }
    }

    protected void Dbg(Object obj) => Dbg(String.Empty, obj);

    protected void Dbg(String title, Object obj) {
        DisplayTitle(title);

        if (!(obj is String && String.IsNullOrEmpty(obj.ToString()))) {
            Console.WriteLine(
                JsonSerializer.Serialize(
                    obj,
                    typeof(Object),
                    new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}