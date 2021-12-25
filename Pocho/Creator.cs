namespace Pocho;

public class Creator<T> where T : class, new() {
    readonly T t;

    public Creator() {
        t = new T();
    }

    public Creator<T> With(Action<T> action) {
        action(t);
        return this;
    }

    public T Create() => t;
}