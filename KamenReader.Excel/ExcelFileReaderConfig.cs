namespace KamenReader.Excel;

public sealed record ExcelImportConfig(String Code, IList<ExcelFileReaderConfigItem> Config);
