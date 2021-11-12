using System.Xml;
using Eksmaru;
using Reflx;

namespace Emi;

static class XmlEventDefinitionLoader {
    static XmlEventDefinition MapConfigItemToEventDefinition(XmlNode eventConfig) {
        String nameValue = eventConfig.GetAttributeValue("name");
        String onlyOnceValue = eventConfig.GetAttributeValue("onlyOnce");
        String typeValue = eventConfig.GetAttributeValue("type");
        String methodValue = eventConfig.GetAttributeValue("method");
        TypeAndAssembly eventTypeNAsm = TypeAndAssemblyParser.Instance.Parse(typeValue);

        if (String.IsNullOrEmpty(methodValue))
            throw new EmitterException($"Wrong Method configuration. '{methodValue}'.");

        return new XmlEventDefinition(nameValue, onlyOnceValue, eventTypeNAsm.Type, eventTypeNAsm.Assembly, methodValue);
    }

    internal static IEnumerable<XmlEventDefinition> Load(String configPath) {
        XmlDocument config = XmlExt.LoadFromPath(configPath);
        String eventsSelector = $"configuration/events";
        XmlNode eventsConfig = config.SelectSingleNode(eventsSelector);
        if (eventsConfig == null)
            throw new EmitterException($"{eventsSelector} wrong configuration.");

        IEnumerable<XmlEventDefinition> events = eventsConfig
            .SelectNodes("event")
            .Cast<XmlNode>()
            .Select(MapConfigItemToEventDefinition);

        return events;
    }
}