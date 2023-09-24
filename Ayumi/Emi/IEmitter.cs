namespace Emi;

public interface IEmitter {
    Int32 Count { get; }

    IEmitter Emit(String name, EmitterEventArgs arg);

    IEmitter Off(String name);

    IEmitter Off(String name, Action<EmitterEventArgs> callback);

    IEmitter On(String name, Action<EmitterEventArgs> callback);

    IEmitter Once(String name, Action<EmitterEventArgs> callback);
}
