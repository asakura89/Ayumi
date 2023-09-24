namespace Haru;

public interface IStorage : IPersistence {
    Int32 GetInt(String key);
    void SetInt(String key, Int32 value);
    Boolean GetBoolean(String key);
    void SetBoolean(String key, Boolean value);
    Single GetFloat(String key);
    void SetFloat(String key, Single value);
    DateTime GetDatetime(String key);
    void SetDatetime(String key, DateTime value);
    TimeSpan GetTimespan(String key);
    void SetTimespan(String key, TimeSpan value);
}