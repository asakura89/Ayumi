using System.Xml;
using AppSea;
using Eksmaru;
using Varya;

namespace KamenReader.Excel;

public class XmlConfigExcelReader {
    readonly String configPath;

    public IList<XmlSpreadsheetDefinition> SpreadsheetDefinitions = new List<XmlSpreadsheetDefinition>();

    public XmlConfigExcelReader() :
        this($"{AppDomain.CurrentDomain.BaseDirectory}\\excelreader.config.xml") { }

    public XmlConfigExcelReader(String configPath) {
        this.configPath = configPath ?? throw new ArgumentNullException(nameof(configPath));
    }

    XmlColumnDefinition MapConfigToColumnDefinition(XmlNode columnConfig) {
        String indexValue = columnConfig.GetAttributeValue("index");
        String nameValue = columnConfig.GetAttributeValue("name");
        String typeValue = columnConfig.GetAttributeValue("type");
        String processValue = columnConfig.GetAttributeValue("process");
        String allowEmptyValue = columnConfig.GetAttributeValue("allowEmpty");

        if (String.IsNullOrEmpty(indexValue) &&
            String.IsNullOrEmpty(nameValue))
            throw new BadConfigurationException("");

        Int32 index = String.IsNullOrEmpty(indexValue) ? -1 : Convert.ToInt32(indexValue);

        if (String.IsNullOrEmpty(typeValue))
            throw new BadConfigurationException("");

        if (String.IsNullOrEmpty(processValue) &&
            String.IsNullOrEmpty(allowEmptyValue))
            throw new BadConfigurationException("");

        Boolean process = String.IsNullOrEmpty(processValue) ? default : Convert.ToBoolean(processValue);
        Boolean allowEmpty = String.IsNullOrEmpty(allowEmptyValue) ? default : Convert.ToBoolean(allowEmptyValue);

        return new XmlColumnDefinition(index, nameValue, typeValue, process, allowEmpty);
    }

    XmlWorksheetDefinition MapConfigToWorksheetDefinition(XmlNode worksheetConfig) {
        String indexValue = worksheetConfig.GetAttributeValue("index");
        String nameValue = worksheetConfig.GetAttributeValue("name");

        if (String.IsNullOrEmpty(indexValue) &&
            String.IsNullOrEmpty(nameValue))
            throw new BadConfigurationException("");

        Int32 index = String.IsNullOrEmpty(indexValue) ? -1 : Convert.ToInt32(indexValue);

        IList<XmlColumnDefinition> columns =
            worksheetConfig
                .SelectNodes("column")
                .Cast<XmlNode>()
                .Select(MapConfigToColumnDefinition)
                .ToList();

        return new XmlWorksheetDefinition(index, nameValue, columns);
    }

    XmlSpreadsheetDefinition MapConfigToSpreadsheetDefinition(XmlNode spreadsheetConfig) {
        String nameValue = spreadsheetConfig.GetAttributeValue("name");
        String pathValue = spreadsheetConfig.GetAttributeValue("path");

        if (String.IsNullOrEmpty(nameValue))
            throw new BadConfigurationException("");

        if (String.IsNullOrEmpty(pathValue))
            throw new BadConfigurationException("");

        IList<XmlWorksheetDefinition> worksheets =
            spreadsheetConfig
                .SelectNodes("worksheet")
                .Cast<XmlNode>()
                .Select(MapConfigToWorksheetDefinition)
                .ToList();

        return new XmlSpreadsheetDefinition(nameValue, pathValue, worksheets);
    }

    public IList<FileReaderResult> Read(String spreadsheetName) {
        XmlSpreadsheetDefinition spreadsheet = null;
        if (!SpreadsheetDefinitions.Any()) {
            XmlDocument config = XmlExt.LoadFromPath(configPath);
            String spreadsheetSelector = $"configuration/spreadsheet";
            IEnumerable<XmlNode> spreadsheetConfig =
                config
                    .SelectNodes(spreadsheetSelector)
                    .Cast<XmlNode>();

            if (spreadsheetConfig == null || !spreadsheetConfig.Any())
                throw new BadConfigurationException($"{spreadsheetSelector}");

            SpreadsheetDefinitions = spreadsheetConfig
                .Select(MapConfigToSpreadsheetDefinition)
                .ToList();
        }

        spreadsheet =
            SpreadsheetDefinitions
                .FirstOrDefault(ssheet => ssheet.Name == spreadsheetName);

        IList<FileReaderResult> results = new List<FileReaderResult>();
        foreach (XmlWorksheetDefinition wsheet in spreadsheet.Worksheets) {
            String path = spreadsheet.Path.Resolve();
            results.Add(new ExcelReader().Read(path, wsheet.Name));
        }

        return results;
    }
}
