namespace Nvy;

public static class NameValueItemExt {
    public static IEnumerable<NameValueItem> AsNameValueList(this String delimitedString) {
        String[] splittedList = delimitedString.Split(NameValueItem.ListDelimiter);
        return splittedList
            .Select(item => item.Split(NameValueItem.ItemDelimiter))
            .Select(splittedItem => new NameValueItem(splittedItem[0], splittedItem[1]));
    }

    public static IEnumerable<NameValueItem> AsNameValueList<T>(this IEnumerable<T> dataList, Func<T, String> nameSelector, Func<T, String> valueSelector) where T : class =>
        dataList.Select(data => new NameValueItem(nameSelector(data), valueSelector(data)));

    public static String AsDelimitedString(this IEnumerable<NameValueItem> nameValueList) {
        String[] delimitedStringList = nameValueList.Select(item => item.Name + NameValueItem.ItemDelimiter + item.Value).ToArray();
        String delimitedString = String.Join(NameValueItem.ListDelimiter.ToString(), delimitedStringList);

        return delimitedString;
    }

    public static String AsDelimitedString<T>(this IEnumerable<T> dataList, Func<T, String> nameSelector, Func<T, String> valueSelector) where T : class =>
        dataList
            .AsNameValueList(nameSelector, valueSelector)
            .AsDelimitedString();
}