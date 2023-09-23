namespace KamenReader.Excel;

public record XmlSpreadsheetDefinition(String Name, String Path, IList<XmlWorksheetDefinition> Worksheets);
