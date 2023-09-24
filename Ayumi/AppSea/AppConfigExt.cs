using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace AppSea;

internal static class AppConfigExt {
    internal static TConfig AsT<TConfig>(this IEnumerable<IConfigurationSection> appSettings) {
        TConfig t = Activator.CreateInstance<TConfig>();
        Type tType = typeof(TConfig);
        PropertyInfo[] propList = tType.GetProperties();
        FieldInfo[] fieldList = tType.GetFields();

        IEnumerable<IConfigurationSection> filtered = appSettings.Where(item => item.Path.Split(':')[0].Equals(tType.Name, StringComparison.InvariantCultureIgnoreCase));
        foreach (PropertyInfo prop in propList) {
            IConfigurationSection setting = filtered.SingleOrDefault(item => item.Key.Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase));
            String value = null;
            if (setting != null)
                value = setting.Value;

            if (!String.IsNullOrEmpty(value))
                prop.SetValue(t, Convert.ChangeType(value, prop.PropertyType), null);
        }

        foreach (FieldInfo field in fieldList) {
            IConfigurationSection setting = filtered.SingleOrDefault(item => item.Key.Equals(field.Name, StringComparison.InvariantCultureIgnoreCase));
            String value = null;
            if (setting != null)
                value = setting.Value;

            if (!String.IsNullOrEmpty(value))
                field.SetValue(t, Convert.ChangeType(value, field.FieldType));
        }

        return t;
    }
}