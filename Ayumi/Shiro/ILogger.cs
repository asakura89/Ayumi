namespace Shiro;

public interface ILogger {
    void Debug<T>(String caller, T obj);
    void Debug(String caller);
    void Debug(String caller, String message);
    void Info<T>(String caller, T obj);
    void Info(String caller);
    void Info(String caller, String message);
    void Error<T>(String caller, T obj);
    void Error(String caller, Exception ex);
    void Error(String caller);
    void Error(String caller, String message);
}