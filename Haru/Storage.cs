using System.Xml;
using Eksmaru;
using Exy;
using Itsu;

namespace Haru;

public class XmlStorage {
    readonly String path;
    readonly String appName;
    XmlDocument docRoot;
    Boolean loaded;

    public XmlStorage(String appName) : this(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "storage.xml"), appName) { }

    public XmlStorage(String path, String appName) {
        if (String.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        if (String.IsNullOrEmpty(appName))
            throw new ArgumentNullException(nameof(appName));

        this.path = path;
        this.appName = appName;
    }

    void InitStorage(String path, String appName) {
        var xmlDoc = new XmlDocument();
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", String.Empty));
        XmlNode storage = xmlDoc.CreateElement("storage", null);
        XmlNode app = xmlDoc.CreateElement(appName, null);
        storage.AppendChild(app);
        xmlDoc.AppendChild(storage);

        xmlDoc.Save(path);
    }

    void Load(String path, String appName) {
        if (!loaded) {
            if (!File.Exists(path))
                InitStorage(path, appName);

            docRoot = XmlExt.LoadFromPath(path);
            XmlNode storage = docRoot.SelectSingleNode("storage");
            if (storage == null)
                throw new UnintendedBehaviorException("xml file contains invalid storage tag.");

            if (storage.SelectSingleNode(appName) == null) {
                XmlNode app = docRoot.CreateElement(appName, null);
                storage.AppendChild(app);
                docRoot.Save(path);
            }

            loaded = true;
        }
    }

    IEnumerable<XmlNode> GetAllItemNodes() =>
        docRoot
            .SelectNodes($"storage/{appName}/item")
            .Cast<XmlNode>();

    public IEnumerable<String> Keys {
        get {
            Load(path, appName);

            return GetAllItemNodes()
                .Select(node => node.GetAttributeValue("key"));
        }
    }

    XmlNode GetItemNode(String key) => docRoot.SelectSingleNode($"storage/{appName}/item[@key='{key}']");

    public Boolean Exists(String key) {
        Load(path, appName);

        XmlNode item = GetItemNode(key);
        if (item == null)
            return false;

        return true;
    }

    public String Get(String key) {
        Load(path, appName);

        XmlNode item = GetItemNode(key);
        if (item == null)
            return String.Empty;

        return item.GetAttributeValue(key);
    }

    public void Set(String key, String value) {
        Load(path, appName);

        XmlNode node = GetItemNode(key);
        if (node == null)
        {
            node = docRoot.CreateElement("item", null);
            docRoot.AssignAttributeTo(node, "key", key);
        }

        docRoot.AssignAttributeTo(node, "value", value ?? "null");
        docRoot.Save(path);
    }

    public Int32 GetInt(String key) {
        String item = Get(key);
        if (String.IsNullOrEmpty(item))
            return 0;

        return Convert.ToInt32(item);
    }

    public void SetInt(String key, Int32 value) => Set(key, value.ToString());

    public Boolean GetBoolean(String key) {
        String item = Get(key);
        if (String.IsNullOrEmpty(item))
            return false;

        return Boolean.Parse(item);
    }

    public void SetBoolean(String key, Boolean value) => Set(key, value.ToString());

    public Single GetFloat(String key) {
        String item = Get(key);
        if (String.IsNullOrEmpty(item))
            return 0;

        return Convert.ToSingle(item);
    }

    public void SetFloat(String key, Single value) => Set(key, value.ToString());

    public DateTime GetDatetime(String key) {
        String item = Get(key);
        if (String.IsNullOrEmpty(item))
            return DateTime.MinValue;

        return item.FromIsoDateTime();
    }

    public void SetDatetime(String key, DateTime value) => Set(key, value.ToIsoDateTime());

    public TimeSpan GetTimespan(String key) {
        String item = Get(key);
        if (String.IsNullOrEmpty(item))
            return TimeSpan.MinValue;

        return TimeSpan.Parse(item);
    }

    public void SetTimespan(String key, TimeSpan value) => Set(key, value.ToString());

    void RemoveItemNode(XmlNode node) => docRoot.SelectSingleNode($"storage/{appName}").RemoveChild(node);

    public void Remove(String key) {
        Load(path, appName);

        XmlNode node = GetItemNode(key);
        if (node != null) {
            RemoveItemNode(node);
            docRoot.Save(path);
        }
    }

    public void Clear() {
        Load(path, appName);

        if (GetAllItemNodes().Any()) {
            XmlNode storageNode = docRoot.SelectSingleNode("storage");
            storageNode.RemoveChild(storageNode.SelectSingleNode(appName));
            storageNode.AppendChild(docRoot.CreateElement(appName, null));
        }
    }
}