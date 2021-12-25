using System.Text.RegularExpressions;

namespace Itsu;

public static class DateTimeExt {
    static readonly String[] months = new[] {
            String.Empty,
            "Jan",
            "Feb",
            "March",
            "Apr",
            "May",
            "June",
            "July",
            "August",
            "Sept",
            "Oct",
            "Nov",
            "Dec"
        };

    static readonly String[] fullmonths = new[] {
            String.Empty,
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        };

    static readonly String[] days = new[] {
            String.Empty,
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday"
        };

    public static String CurrentReadableDay => DateTime.Now.AsReadableDay();
    public static String CurrentReadableDate => DateTime.Now.AsReadableDate();
    public static String CurrentSimpleReadableDate => DateTime.Now.AsSimpleReadableDate();
    public static String AsReadableDay(this DateTime date) => days[Convert.ToInt32(date.DayOfWeek) + 1];
    public static String AsReadableDate(this DateTime date) => $"{date.Day} {fullmonths[date.Month]} {date.Year}";
    public static String AsSimpleReadableDate(this DateTime date) => $"{date.Day} {months[date.Month]}";

    public static DateTime ToUtc(this DateTime datetime, TimeZoneInfo timezone = null) {
        if (datetime.Kind == DateTimeKind.Utc)
            return datetime;

        if (datetime == DateTime.MinValue || datetime == DateTime.MaxValue)
            return DateTime.SpecifyKind(datetime, DateTimeKind.Utc);

        if (datetime.Kind == DateTimeKind.Local)
            return TimeZoneInfo.ConvertTimeToUtc(datetime);

        if (timezone == null)
            return TimeZoneInfo.ConvertTimeToUtc(datetime, TimeZoneInfo.Local);

        return TimeZoneInfo.ConvertTimeToUtc(datetime, timezone);
    }

    public static String ToIsoDateTime(this DateTime datetime) {
        datetime = datetime.ToUtc();
        return $"{datetime:yyyyMMddTHHmmssZ}";
    }

    static Int32 ToInt32(this String value) => value.ToInt32(default(Int32));

    static Int32 ToInt32(this String value, Int32 defaultValue) {
        if (String.IsNullOrEmpty(value))
            return defaultValue;

        Boolean success = Int32.TryParse(value, out Int32 converted);
        if (success)
            return converted;

        return defaultValue;
    }

    public static DateTime FromUtc(this DateTime datetime, TimeZoneInfo timezone = null) {
        if (datetime.Kind == DateTimeKind.Local)
            return datetime;

        if (datetime == DateTime.MinValue || datetime == DateTime.MaxValue)
            return DateTime.SpecifyKind(datetime, DateTimeKind.Local);

        if (timezone == null)
            return TimeZoneInfo.ConvertTimeFromUtc(datetime, TimeZoneInfo.Local);

        return TimeZoneInfo.ConvertTimeToUtc(datetime, timezone);
    }

    public static DateTime FromIsoDateTime(this String isoString) {
        if (String.IsNullOrEmpty(isoString))
            return DateTime.MinValue;

        /*

        20201220T025957Z
        20201220T025957
        20201220T
        20201220
        T201220Z
        0917T20

         */

        Match m = Regex.Match(isoString, "^(?<year>[0-9]{4})?(?<mo>[0-9]{2})?(?<date>[0-9]{2})?T?(?<hour>[0-9]{2})?(?<min>[0-9]{2})?(?<sec>[0-9]{2})?(?:Z$)?", RegexOptions.Compiled | RegexOptions.Singleline);
        if (!m.Groups["year"].Success &&
            !m.Groups["mo"].Success &&
            !m.Groups["date"].Success &&
            !m.Groups["hour"].Success &&
            !m.Groups["min"].Success &&
            !m.Groups["sec"].Success)
            throw new FormatException("iso datetime format is invalid.");

        Int32 year = m.Groups["year"].Value.ToInt32();
        Int32 month = m.Groups["mo"].Value.ToInt32();
        Int32 date = m.Groups["date"].Value.ToInt32();
        Int32 hour = m.Groups["hour"].Value.ToInt32();
        Int32 min = m.Groups["min"].Value.ToInt32();
        Int32 sec = m.Groups["sec"].Value.ToInt32();

        var fromString = new DateTime(year, month, date, hour, min, sec, DateTimeKind.Utc);
        return fromString.FromUtc();
    }
}