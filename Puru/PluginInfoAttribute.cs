namespace Puru;

[AttributeUsage(AttributeTargets.Class)]
public sealed class PluginInfoAttribute : Attribute {
    public String Code { get; }
    public String Name { get; }
    public String Desc { get; }

    public PluginInfoAttribute(String code, String name, String desc = "") {
        Code = code;
        Name = name;
        Desc = desc;
    }
}