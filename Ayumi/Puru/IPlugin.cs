namespace Puru;

public interface IPlugin {
    String ComponentName { get; }
    String ComponentDesc { get; }
    Object Process(Object processArgs);
}