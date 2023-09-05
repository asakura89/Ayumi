using AppSea;

namespace KamenReader;

public sealed class FixedLengthReader : IFileReader {
    public FileReaderResult Read(String fullFilepath, IList<FileReaderMap> maps, Boolean firstRowAreTitles = true) {
        if (String.IsNullOrEmpty(fullFilepath))
            throw new ArgumentNullException(nameof(fullFilepath));

        if (!File.Exists(fullFilepath))
            throw new FileNotFoundException("File's not found.");

        if (maps == null)
            throw new ArgumentNullException(nameof(maps));

        if (!maps.Any())
            throw new BadConfigurationException("File Reader Map");

        var result = new FileReaderResult();
        String[] allLines = File.ReadAllLines(fullFilepath);
        for (Int32 row = 1; row <= allLines.Length; row++) {
            String currentLine = allLines[row - 1];
            Int32 lineLength = currentLine.Length;
            for (Int32 col = 1; col <= maps.Count; col++) {
                FileReaderMap currentMap = maps[col - 1];
                if (currentMap.StartAt >= lineLength)
                    throw new IndexOutOfRangeException("currentMap.StartAt");

                String data = currentLine.Substring(currentMap.StartAt, currentMap.Length);
                String cleaned = String.IsNullOrEmpty(data) ? data : data.Trim();
                if (firstRowAreTitles && row == 1)
                    result.Titles.Add(cleaned);
                else
                    result.Data.Add(new GridData(Column: currentMap.StartAt, Row: row, CellValue: cleaned, Length: 0));
            }
        }

        return result;
    }
}