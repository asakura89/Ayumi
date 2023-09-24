namespace Emi;

public record XmlEventDefinition {
    public String Name { get; }
    public Boolean OnlyOnce { get; }
    public String Type { get; set; }
    public String Assembly { get; }
    public String Method { get; }

    public XmlEventDefinition(String name, String onlyOnce, String type, String assembly, String method) {
        if (String.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        if (String.IsNullOrEmpty(type))
            throw new ArgumentNullException(nameof(type));

        if (String.IsNullOrEmpty(assembly))
            throw new ArgumentNullException(nameof(assembly));

        if (String.IsNullOrEmpty(method))
            throw new ArgumentNullException(nameof(method));

        Name = name;
        OnlyOnce = String.IsNullOrEmpty(onlyOnce) ?
            false : Convert.ToBoolean(onlyOnce);

        Type = type;
        Assembly = assembly;
        Method = method;
    }
}