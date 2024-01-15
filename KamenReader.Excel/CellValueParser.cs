using System.Globalization;
using Arvy;
using Kacchun;
using System.Text.RegularExpressions;
using Tipe;

namespace KamenReader.Excel;

public static class CellValueParser {
    public static IDictionary<String, Func<String, GridData, DataType>> Parsers = new Dictionary<String, Func<String, GridData, DataType>> {
        {"String", ParseStringValue},
        {"string", ParseStringValue},
        {"Decimal", ParseDecimalValue},
        {"decimal", ParseDecimalValue},
        {"DateTime", ParseDatetimeValue},
        {"datetime", ParseDatetimeValue},
        {"LiteralTimeSpan", ParseLiteralTimespanValue},
        {"literaltimespan", ParseLiteralTimespanValue},
        {"Boolean", ParseBooleanValue},
        {"boolean", ParseBooleanValue},
        {"No", NoParser},
        {"no", NoParser}
    };

    static DataType NoParser(String name, GridData data) =>
        throw new InvalidOperationException($"There are no parser for {name}");

    static DataType ParseStringValue(String name, GridData data) =>
        new DataType(name, data.CellValue, typeof(String));

    static DataType ParseDecimalValue(String name, GridData data) {
        Decimal outValue;
        Boolean result = Decimal.TryParse(data.CellValue, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out outValue);

        return new DataType(name, outValue, typeof(Decimal));
    }

    static DataType ParseDatetimeValue(String name, GridData data) {
        String cellValue = data.CellValue;
        if (String.IsNullOrEmpty(cellValue))
            cellValue = DateUtils.MinSlashedDDMMYYYYDateString;

        /*
            NOTE: According to these links, Excel Datetime could be stored as Number and has bugs
            https://support.microsoft.com/en-nz/help/214019/method-to-determine-whether-a-year-is-a-leap-year
            https://support.microsoft.com/en-nz/help/214326/excel-incorrectly-assumes-that-the-year-1900-is-a-leap-year
            http://www.kirix.com/stratablog/excel-date-conversion-days-from-1900
            https://stackoverflow.com/questions/22612203/how-to-get-format-type-of-cell-using-c-sharp-in-spreadsheetlight
        */
        Int32 iOutValue;
        Boolean iResult = Int32.TryParse(cellValue, out iOutValue);
        DateTime dOutValue;

        if (iResult)
            dOutValue = new DateTime(1900, 1, 1).AddDays(iOutValue - 2);
        else
            DateTime.TryParseExact(cellValue, DateUtils.SlashedDateDDMMYYYY, CultureInfo.InvariantCulture, DateTimeStyles.None, out dOutValue);

        return new DataType(name, dOutValue, typeof(DateTime));
    }

    static DataType ParseLiteralTimespanValue(String name, GridData data) {
        String cellValue = data.CellValue;
        String result = HandleLiteralTimeSpan(cellValue);
        TimeSpan tsOutValue;
        Boolean tsResult = TimeSpan.TryParse(result, out tsOutValue);

        return new DataType(name, tsOutValue, typeof(TimeSpan));
    }

    static String HandleLiteralTimeSpan(String literal) {
        String regex = @"(?<digit>\d{1,})(?<type>[Dd]|[Hh]|[Mm]|[Ss])";

        Match varMatch = Regex.Match(literal, regex, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);
        if (varMatch.Success)
            return HandleTimespan(varMatch);

        return String.Empty;
    }

    static String HandleTimespan(Match match) {
        String digit = match.Groups["digit"].Value;
        String type = match.Groups["type"].Value;
        if (String.IsNullOrEmpty(digit))
            return String.Empty;

        if (String.IsNullOrEmpty(type))
            return String.Empty;

        String duration = digit.PadLeft(2, '0');
        switch (type) {
            case "d":
                return $"{duration}:00:00:00";
            case "h":
                return $"00:{duration}:00:00";
            case "m":
                return $"00:00:{duration}:00";
            case "s":
                return $"00:00:00:{duration}";
            default:
                return "00:00:00:00";
        }
    }

    static DataType ParseBooleanValue(String name, GridData data) {
        if (new[] { "true", "1" }.Contains(data.CellValue, new InvariantCultureIgnoreCaseComparer()))
            return new DataType(name, true, typeof(Boolean));

        if (new[] { "false", "0" }.Contains(data.CellValue, new InvariantCultureIgnoreCaseComparer()))
            return new DataType(name, false, typeof(Boolean));

        return new DataType(name, false, typeof(Boolean));
    }
}
