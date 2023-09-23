namespace KamenReader.Excel;

public record XmlWorksheetDefinition(Int32 Index, String Name, IList<XmlColumnDefinition> Columns);
