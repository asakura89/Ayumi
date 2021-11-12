namespace KamenReader;

public record FileReaderResult {
    public IList<String> Titles { get; } = new List<String>();
    public IList<GridData> Data { get; } = new List<GridData>();
};