namespace Haru;

public interface IAppSession : IPersistence {
    new Object Get(String key);
    void Set(String key, Object value);
}