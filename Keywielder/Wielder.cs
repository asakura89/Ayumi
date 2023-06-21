using System.Globalization;
using System.Text;
using RyaNG;

namespace Keywielder;

public enum AlphaType {
    UpperLowerNumericSymbol,
    UpperLowerNumeric,
    UpperLowerSymbol,
    UpperLower,
    UpperNumeric,
    UpperSymbol,
    UpperHex,
    Upper,

    LowerNumeric,
    LowerSymbol,
    LowerHex,
    Lower,

    NumericSymbol,
    Numeric,
    Symbol
}

public class Wielder {
    const String UppercaseAlphabet = "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z";
    const String LowercaseAlphabet = "a b c d e f g h i j k l m n o p q r s t u v w x y z";
    const String UppercaseHexAlphabet = "A B C D E F";
    const String LowercaseHexAlphabet = "a b c d e f";
    const String Numeric = "1 2 3 4 5 6 7 8 9 0";
    const String Symbol = "~ ! @ # $ % ^ & * _ - + = ` | \\ ( ) { } [ ] : ; < > . ? /";

    static readonly IDictionary<AlphaType, String> alphaTypeDict = new Dictionary<AlphaType, String> {
        [AlphaType.UpperLowerNumericSymbol] = UppercaseAlphabet + " " + LowercaseAlphabet + " " + Numeric + " " + Symbol,
        [AlphaType.UpperLowerNumeric] = UppercaseAlphabet + " " + LowercaseAlphabet + " " + Numeric,
        [AlphaType.UpperLowerSymbol] = UppercaseAlphabet + " " + LowercaseAlphabet + " " + Symbol,
        [AlphaType.UpperLower] = UppercaseAlphabet + " " + LowercaseAlphabet,
        [AlphaType.UpperNumeric] = UppercaseAlphabet + " " + Numeric,
        [AlphaType.UpperSymbol] = UppercaseAlphabet + " " + Symbol,
        [AlphaType.UpperHex] = UppercaseHexAlphabet + " " + Numeric,
        [AlphaType.Upper] = UppercaseAlphabet,

        [AlphaType.LowerNumeric] = LowercaseAlphabet + " " + Numeric,
        [AlphaType.LowerSymbol] = LowercaseAlphabet + " " + Symbol,
        [AlphaType.LowerHex] = LowercaseHexAlphabet + " " + Numeric,
        [AlphaType.Lower] = LowercaseAlphabet,

        [AlphaType.NumericSymbol] = Numeric + " " + Symbol,
        [AlphaType.Numeric] = Numeric,

        [AlphaType.Symbol] = Symbol
    };

    readonly StringBuilder keyBuilder = new StringBuilder();

    Wielder() { }

    public static Wielder New() => new Wielder();

    public Wielder AddRandomString(Int32 valueLength) => AddRandomString(valueLength, AlphaType.Upper);

    public Wielder AddRandomString(Int32 valueLength, AlphaType type) => AddRandom(valueLength, alphaTypeDict[type].Split(' '));

    public Wielder AddRandomNumber(Int32 valueLength) => AddRandomString(valueLength, AlphaType.Numeric);

    public Wielder AddRandomAlphaNumeric(Int32 valueLength) => AddRandomAlphaNumeric(valueLength, true);

    public Wielder AddRandomAlphaNumeric(Int32 valueLength, Boolean uppercase) =>
        uppercase ? AddRandomString(valueLength, AlphaType.Upper) : AddRandomString(valueLength, AlphaType.Lower);

    Wielder AddRandom(Int32 valueLength, String[] charCombination) {
        var randomString = new StringBuilder();
        for (Int32 i = 0; i < valueLength; i++) {
            Int32 randomIdx = (charCombination.Length - 1).Ryandomize();
            randomString.Append(charCombination[randomIdx]);
        }

        keyBuilder.Append(randomString);
        return this;
    }

    public Wielder AddRandomHex(Int32 valueLength) => AddRandomHex(valueLength, true);

    public Wielder AddRandomHex(Int32 valueLength, Boolean uppercase) =>
        uppercase ? AddRandomString(valueLength, AlphaType.UpperHex) : AddRandomString(valueLength, AlphaType.LowerHex);

    public Wielder AddGuidString() {
        keyBuilder.Append(Guid.NewGuid().ToString("N"));
        return this;
    }

    public Wielder AddString(String value) => AddString(value, value.Length);

    public Wielder AddString(String value, Int32 valueLength) {
        String strWithLength = value[..valueLength].ToUpper();
        keyBuilder.Append(strWithLength);
        return this;
    }

    const Char Space = ' ';
    public Wielder AddRightPadded(Func<Wielder, Wielder> toBeRightPadded, Int32 valueLength) =>
        AddRightPadded(toBeRightPadded(New()).BuildKey(), valueLength, Space);

    public Wielder AddRightPadded(Func<Wielder, Wielder> toBeRightPadded, Int32 valueLength, Char paddedBy) =>
        AddRightPadded(toBeRightPadded(New()).BuildKey(), valueLength, paddedBy);

    public Wielder AddRightPadded(String toBeRightPadded, Int32 valueLength) =>
        AddRightPadded(toBeRightPadded, valueLength, Space);

    public Wielder AddRightPadded(String toBeRightPadded, Int32 valueLength, Char paddedBy) {
        String resultString = toBeRightPadded
            .PadRight(valueLength, paddedBy)[..valueLength];
        keyBuilder.Append(resultString);

        return this;
    }

    public Wielder AddLeftPadded(Func<Wielder, Wielder> tobeLeftPadded, Int32 valueLength) =>
        AddLeftPadded(tobeLeftPadded(New()).BuildKey(), valueLength, Space);

    public Wielder AddLeftPadded(Func<Wielder, Wielder> tobeLeftPadded, Int32 valueLength, Char paddedBy) =>
        AddLeftPadded(tobeLeftPadded(New()).BuildKey(), valueLength, paddedBy);

    public Wielder AddLeftPadded(String tobeLeftPadded, Int32 valueLength) =>
        AddLeftPadded(tobeLeftPadded, valueLength, Space);

    public Wielder AddLeftPadded(String tobeLeftPadded, Int32 valueLength, Char paddedBy) {
        String resultString = tobeLeftPadded
            .PadLeft(valueLength, paddedBy)[..valueLength];
        keyBuilder.Append(resultString);

        return this;
    }

    public Wielder AddShortYear() => AddYear(2);

    public Wielder AddLongYear() => AddYear(4);

    Wielder AddYear(Int32 valueLength) {
        Int32 currentYear = DateTime.Now.Year;
        String yearWithLength = valueLength == 4 ?
            currentYear.ToString(CultureInfo.InvariantCulture) :
            currentYear.ToString(CultureInfo.InvariantCulture).Substring(2, 2);
        keyBuilder.Append(yearWithLength);

        return this;
    }

    public Wielder AddShortMonth() => AddMonth(3);

    public Wielder AddShortMonth(IList<String> customMonthList) => AddMonth(3, customMonthList);

    public Wielder AddLongMonth() => AddMonth(4);

    public Wielder AddLongMonth(IList<String> customMonthList) => AddMonth(4, customMonthList);

    public Wielder AddNumericMonth() => AddMonth(2);

    const Char Zero = '0';
    Wielder AddMonth(Int32 valueLength) {
        String[] defaultMonthList = { "", "JANUARY", "FEBRUARY", "MARCH", "APRIL", "MAY", "JUNE", "JULY", "AUGUST", "SEPTEMBER", "OCTOBER", "NOVEMBER", "DECEMBER" };
        return AddMonth(valueLength, defaultMonthList);
    }

    Wielder AddMonth(Int32 valueLength, IList<String> monthList) {
        String month = String.Empty;
        Int32 currentMonth = DateTime.Now.Month;
        switch (valueLength) {
            case 4:
                month = monthList[currentMonth];
                break;
            case 3:
                month = monthList[currentMonth][..3];
                break;
            case 2:
                month = currentMonth.ToString(CultureInfo.InvariantCulture).PadLeft(2, Zero);
                break;
        }

        keyBuilder.Append(month);
        return this;
    }

    public Wielder AddDate() => AddDate(0);

    public Wielder AddDate(Int32 valueLength) {
        keyBuilder.Append(DateTime.Now.Day.ToString().PadLeft(valueLength, Zero));
        return this;
    }

    public Wielder AddShortDay() => AddDay(3);

    public Wielder AddShortDay(IList<String> customDayList) => AddDay(3, customDayList);

    public Wielder AddLongDay() => AddDay(4);

    public Wielder AddLongDay(IList<String> customDayList) => AddDay(4, customDayList);

    public Wielder AddNumericDay() => AddDay(2);

    Wielder AddDay(Int32 valueLength) {
        String[] defaultDayList = { "", "SUNDAY", "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY" };
        return AddDay(valueLength, defaultDayList);
    }

    Wielder AddDay(Int32 valueLength, IList<String> dayList) {
        String day = String.Empty;
        Int32 currentDayOfWeek = Convert.ToInt32(DateTime.Now.DayOfWeek) + 1;
        switch (valueLength) {
            case 4:
                day = dayList[currentDayOfWeek];
                break;
            case 3:
                day = dayList[currentDayOfWeek][..3];
                break;
            case 2:
                day = currentDayOfWeek.ToString(CultureInfo.InvariantCulture).PadLeft(2, Zero);
                break;
        }

        keyBuilder.Append(day);
        return this;
    }

    public Wielder AddCounter(Int32 currentCounter) => AddCounter(currentCounter, 1);

    public Wielder AddCounter(Int32 currentCounter, Int32 increment) {
        String counter = (currentCounter + increment).ToString(CultureInfo.InvariantCulture);
        keyBuilder.Append(counter);
        return this;
    }

    public String BuildKey() => keyBuilder.ToString();
}