namespace Shiro;

public sealed class LogCallerInfo {
    public String CallerType { get; set; }
    public String CallerMember { get; set; }
}

public static class LogExt {
    // NOTE: One is not encouraged to fill `memberName` parameter as it'll be injected by the compiler service
    public static LogCallerInfo GetCallerInfo<T>(this T caller, [System.Runtime.CompilerServices.CallerMemberName] String memberName = "") =>
        new LogCallerInfo {
            CallerType = caller.GetType().FullName,
            CallerMember = memberName
        };

    public static String GetFormattedCallerInfoString<T>(this T caller, [System.Runtime.CompilerServices.CallerMemberName] String memberName = "") =>
        $"{caller.GetType().FullName}.{memberName}";

    public static String GetFormattedCallerInfoString(this Type callerType, [System.Runtime.CompilerServices.CallerMemberName] String memberName = "") =>
        $"{callerType.FullName}.{memberName}";
}