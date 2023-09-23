namespace KamenReader.Excel;

internal class InvariantCultureIgnoreCaseComparer : IEqualityComparer<String> {
    public Boolean Equals(String x, String y) =>
        x.Equals(y, StringComparison.InvariantCultureIgnoreCase);

    public Int32 GetHashCode(String obj) =>
        obj.GetHashCode();
}