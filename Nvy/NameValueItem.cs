namespace Nvy;

public record NameValueItem(String Name, String Value) {
    public const String NameProperty = "Name";
    public const String ValueProperty = "Value";
    public const Char ListDelimiter = '·';
    public const Char ItemDelimiter = '•';

    public static NameValueItem Empty => new(String.Empty, String.Empty);
}