using System.Text.Json;

namespace Scratch;

public static class JsonSerializerExt {
    public static String AsJson<T>(this T data) =>
        JsonSerializer.Serialize(data, typeof(T));

    public static String AsIndentedJson<T>(this T data) =>
        JsonSerializer.Serialize(data, typeof(T), new JsonSerializerOptions { WriteIndented = true });
}
