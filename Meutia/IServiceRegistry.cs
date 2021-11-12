namespace Meutia;

public interface IServiceRegistry : IServiceProvider {
    Object GetService(String name);

    I GetService<I>();
    I GetService<I>(String name);
    void RegisterService<A, I>() where I : A;
    void RegisterService<A, I>(String name) where I : A;
    void RegisterService<I>(Func<I> instanceCreator);
    void RegisterService<I>(Func<I> instanceCreator, String name);
    void RegisterService<I>(I instance);
    void RegisterService<I>(I instance, String name);
}