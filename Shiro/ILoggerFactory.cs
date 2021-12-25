namespace Shiro;

public interface ILoggerFactory {
    ILogger CreateByType(Type caller);
    ILogger CreateByName(String caller);
}