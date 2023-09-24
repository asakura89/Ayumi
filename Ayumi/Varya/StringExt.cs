using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using Serena;
using ConfigurationManager = AppSea.ConfigurationManager;

namespace Varya;

public static class StringExt {
    const String MainRegex = @"\$\{(?<key>[a-zA-Z0-9\.\-\\_]*)(?:\:(?<value>[^\{\}]+))?\}";

    public static String Resolve(this String string2Resolve) => ReplaceWithDictionary(string2Resolve, new Dictionary<String, String>());

    public static String ReplaceWith<TReplace>(this String string2Replace, TReplace replacements) where TReplace : class, new() =>
        string2Replace.ReplaceWithDictionary(
            replacements
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(
                    prop => prop.Name,
                    prop => Convert.ToString(prop.GetValue(replacements, null))
                )
        );

    const Int16 MaxRecurseCount = 3;
    public static String ReplaceWithDictionary(this String string2Replace, IDictionary<String, String> replacements) {
        String result = string2Replace;
        for (Int32 count = 0; count < MaxRecurseCount; count++) {
            // NOTE: regex can't recursive and not mean to be recursive
            result = Regex.Replace(
                result,
                MainRegex,
                match => HandleRegex(match, replacements),
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);
        }

        return result;
    }

    static String HandleRegex(Match match, IDictionary<String, String> replacements) {
        String key = match.Groups["key"].Value;
        if (String.IsNullOrEmpty(key))
            return match.Value;

        String value = match.Groups["value"].Value;
        IEnumerable<String> operations = new[] { "config", "econfig", "var", "urle", "urld", "htmle", "htmld" };
        if (operations.Contains(key)) {
            switch (key) {
                case "config":
                    return HandleConfig(value, replacements);
                case "econfig":
                    return HandleEncryptedConfig(value, replacements);
                case "var":
                    return HandleVar(value);
                case "urle":
                    return HandleUrlEncode(value);
                case "urld":
                    return HandleUrlDecode(value);
                case "htmle":
                    return HandleHtmlEncode(value);
                case "htmld":
                    return HandleHtmlDecode(value);
                default:
                    return match.Value;
            }
        }

        if (String.IsNullOrEmpty(value) || value.All(Char.IsWhiteSpace)) {
            // NOTE: if key exists and value not exists then it means it's a general replaceable-var not an opeation-var
            if (replacements == null)
                return match.Value;

            if (!replacements.Any())
                return match.Value;

            if (!replacements.ContainsKey(key))
                return match.Value;

            return replacements[key];
        }

        return match.Value;
    }

    static String HandleConfig(String configKey, IDictionary<String, String> replacements) {
        String appSetting = ConfigurationManager.GetAppSetting(configKey)?.Value;
        return ReplaceWithDictionary(appSetting ?? String.Empty, replacements);
    }

    static String HandleEncryptedConfig(String configKey, IDictionary<String, String> replacements) {
        String config = HandleConfig(configKey, replacements);
        return config.Decrypt();
    }

    static String HandleVar(String var) {
        var variableRegexes = new Dictionary<String, String> {
            ["Timespan"] = @"(?<digit>\d{1,})(?<type>[Dd]|[Hh]|[Mm]|[Ss])",
            ["Datetime"] = @"[Dd]ate[Tt]ime\((?<dtformat>\w*)\)",
            ["BaseDir"] = "BaseDir"
        };

        var variableHandlers = new Dictionary<String, Func<Match, String>> {
            ["Timespan"] = HandleTimespan,
            ["Datetime"] = HandleDatetime,
            ["BaseDir"] = HandleBaseDir
        };

        foreach (String key in variableRegexes.Keys) {
            Match varMatch = Regex.Match(var, variableRegexes[key], RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);
            if (varMatch.Success)
                return variableHandlers[key].Invoke(varMatch);
        }

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

    static String HandleDatetime(Match match) {
        String format = match.Groups["dtformat"].Value;
        if (String.IsNullOrEmpty(format))
            return String.Empty;

        return DateTime.Now.ToString(format);
    }

    static String HandleBaseDir(Match match) => AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');

    static String HandleUrlEncode(String value) => HttpUtility.UrlEncode(value).Replace("+", "%20");

    static String HandleUrlDecode(String value) => HttpUtility.UrlDecode(value);

    static String HandleHtmlEncode(String value) => HttpUtility.HtmlEncode(value);

    static String HandleHtmlDecode(String value) => HttpUtility.HtmlDecode(value);
}