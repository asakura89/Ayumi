using KamenReader;
using SpreadsheetLight;

namespace KamenReader.Excel;

public sealed class ExcelReader { // : IFileReader {
    public FileReaderResult Read(String fullFilepath, String worksheetName, Boolean firstRowAreTitles = true) {
        if (String.IsNullOrEmpty(fullFilepath))
            throw new ArgumentException("fullFilepath");
        if (!File.Exists(fullFilepath))
            throw new InvalidOperationException("File's not found.");

        var result = new FileReaderResult();
        using (var doc = new SLDocument(fullFilepath)) {
            if (String.IsNullOrEmpty(worksheetName))
                doc.SelectWorksheet(doc.GetCurrentWorksheetName());
            else
                doc.SelectWorksheet(worksheetName);

            var stats = doc.GetWorksheetStatistics();
            for (int row = 1; row <= stats.EndRowIndex; row++) {
                for (int col = 1; col <= stats.EndColumnIndex; col++) {
                    String data = doc.GetCellValueAsString(row, col);
                    String cleaned = String.IsNullOrEmpty(data) ? data : data.Trim();
                    if (firstRowAreTitles && row == 1)
                        result.Titles.Add(cleaned);
                    else
                        result.Data.Add(new GridData(Column: col, Row: row, CellValue: cleaned, Length: cleaned.Length));
                }
            }
        }

        return result;
    }
}