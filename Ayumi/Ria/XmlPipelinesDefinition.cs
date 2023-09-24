namespace Ria;

public record XmlPipelinesDefinition(String Name, String ContextType, String ContextAssembly, IList<XmlPipelineActionDefinition> Actions);