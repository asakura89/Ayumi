using System.Globalization;
using System.Text.RegularExpressions;
using Arvy;
using Kacchun;

namespace KamenReader.Excel;

public static class CellValueValidator {
    public static IDictionary<String, Func<String, GridData, Boolean, ActionResponseViewModel>> Validators = new Dictionary<String, Func<String, GridData, Boolean, ActionResponseViewModel>> {
        {"String", ValidateString},
        {"string", ValidateString},
        {"Decimal", ValidateDecimal},
        {"decimal", ValidateDecimal},
        {"DateTime", ValidateDatetime},
        {"datetime", ValidateDatetime},
        {"LiteralTimeSpan", ValidateLiteralTimespan},
        {"literaltimespan", ValidateLiteralTimespan},
        {"Boolean", ValidateBoolean},
        {"boolean", ValidateBoolean},
        {"ByPass", ByPassValidation},
        {"bypass", ByPassValidation},
        {"No", NoValidator},
        {"no", NoValidator}
    };

    static ActionResponseViewModel NoValidator(String name, GridData data, Boolean allowEmpty) =>
        throw new InvalidOperationException($"There are no validator for {name}");

    static ActionResponseViewModel ByPassValidation(String name, GridData data, Boolean allowEmpty) =>
        GetSuccessMessage(name, data.Row, data.Column);

    static ActionResponseViewModel ValidateString(String name, GridData data, Boolean allowEmpty) {
        if (!allowEmpty && String.IsNullOrEmpty(data.CellValue))
            return GetInvalidFormatMessage(name, data.Row, data.Column);

        return GetSuccessMessage(name, data.Row, data.Column);
    }

    static ActionResponseViewModel ValidateDecimal(String name, GridData data, Boolean allowEmpty) {
        if (!allowEmpty && String.IsNullOrEmpty(data.CellValue))
            return GetInvalidFormatMessage(name, data.Row, data.Column);

        Decimal outValue;
        Boolean result = Decimal.TryParse(data.CellValue, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out outValue);
        if (!result)
            return GetInvalidFormatMessage(name, data.Row, data.Column);

        if (outValue < 1)
            return GetInvalidFormatMessage(name, data.Row, data.Column);

        return GetSuccessMessage(name, data.Row, data.Column);
    }

    static ActionResponseViewModel ValidateDatetime(String name, GridData data, Boolean allowEmpty) {
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
        if (iResult) {
            if (iOutValue < 1)
                return GetInvalidFormatMessage(name, data.Row, data.Column);
        }
        else {
            DateTime dOutValue;
            Boolean dResult = DateTime.TryParseExact(cellValue, DateUtils.SlashedDateDDMMYYYY, CultureInfo.InvariantCulture, DateTimeStyles.None, out dOutValue);
            if (!dResult)
                return GetInvalidFormatMessage(name, data.Row, data.Column);
        }

        return GetSuccessMessage(name, data.Row, data.Column);
    }

    static ActionResponseViewModel ValidateLiteralTimespan(String name, GridData data, Boolean allowEmpty) {
        String cellValue = data.CellValue;
        if (!allowEmpty && String.IsNullOrEmpty(data.CellValue))
            return GetInvalidFormatMessage(name, data.Row, data.Column);

        String result = HandleLiteralTimeSpan(cellValue);
        if (result == TimeSpanUtils.ZeroTimeSpan)
            return GetInvalidFormatMessage(name, data.Row, data.Column);

        TimeSpan tsOutValue;
        Boolean tsResult = TimeSpan.TryParse(result, out tsOutValue);
        if (!tsResult)
            return GetInvalidFormatMessage(name, data.Row, data.Column);

        return GetSuccessMessage(name, data.Row, data.Column);
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

    static ActionResponseViewModel ValidateBoolean(String name, GridData data, Boolean allowEmpty) {
        if (!new[] { "true", "false", "1", "0" }.Contains(data.CellValue, new InvariantCultureIgnoreCaseComparer()))
            return GetInvalidFormatMessage(name, data.Row, data.Column);

        return GetSuccessMessage(name, data.Row, data.Column);
    }

    static ActionResponseViewModel GetInvalidFormatMessage(String name, Int32 row, Int32 column) {
        return new ActionResponseViewModel(
            ResponseType: ActionResponseViewModel.Error,
            Message: $"{name} is in invalid format. Row: {row}, Col: {column}"
        );
    }

    static ActionResponseViewModel GetSuccessMessage(String name, Int32 row, Int32 column) {
        return new ActionResponseViewModel(
            ResponseType: ActionResponseViewModel.Success,
            Message: $"{name} parsed successfully. Row: {row}, Col: {column}"
        );
    }
}
