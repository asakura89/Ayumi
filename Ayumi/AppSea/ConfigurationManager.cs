using Microsoft.Extensions.Configuration;

namespace AppSea;

public static class ConfigurationManager {
    static readonly IList<IConfigurationSection> settings;

    static ConfigurationManager() {
        IConfigurationRoot configRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        settings = GetFlattenedSections(configRoot);
    }

    static IList<IConfigurationSection> GetFlattenedSections(IConfigurationRoot root) {
        IList<IConfigurationSection> flat = new List<IConfigurationSection>();
        IList<IConfigurationSection> rootSections = root.GetChildren().ToList();
        if (rootSections != null && rootSections.Any()) {
            var remainings = new Stack<IConfigurationSection>(rootSections);
            while (remainings.Any()) {
                IConfigurationSection current = remainings.Pop();
                IEnumerable<IConfigurationSection> children = current.GetChildren();
                if (children.Any())
                    foreach (IConfigurationSection section in children)
                        remainings.Push(section);
                else
                    flat.Add(current);
            }
        }

        return flat;
    }

    public static IList<IConfigurationSection> AppSettings =>
        settings
            .Where(item => !item.Path.Split(':')[0].Equals("connectionstrings", StringComparison.InvariantCultureIgnoreCase))
            .ToList();

    public static IList<IConfigurationSection> ConnectionStrings =>
        settings
            .Where(item => item.Path.Split(':')[0].Equals("connectionstrings", StringComparison.InvariantCultureIgnoreCase))
            .ToList();

    public static IConfigurationSection GetAppSetting(String name) => AppSettings.SingleOrDefault(item => item.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase));

    public static IConfigurationSection GetConnectionString(String name) => ConnectionStrings.SingleOrDefault(item => item.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase));
}