namespace Pocho;

public static class CreatorExt {
    public static T With<T>(this T t, Action<T> action) where T : class, new() {
        action(t);
        return t;
    }
}