namespace Haru;

public interface IPersistence {
    IEnumerable<String> Keys { get; }
    Boolean Exists(String key);
    String Get(String key);
    void Set(String key, String value);
    void Remove(String key);
    void Clear();
}