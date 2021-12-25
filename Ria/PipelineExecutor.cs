using System.Reflection;
using Reflx;

namespace Ria;

public class PipelineExecutor : IPipelineExecutor {
    readonly IList<XmlPipelinesDefinition> definitions;

    public PipelineExecutor(IList<XmlPipelinesDefinition> definitions) {
        if (definitions == null || !definitions.Any())
            throw new ArgumentNullException(nameof(definitions));

        this.definitions = definitions;
    }

    public PipelineContext Execute(String pipelineName) => Execute(pipelineName, null);

    public PipelineContext Execute(String pipelineName, IDictionary<String, Object> data) {
        XmlPipelinesDefinition pipeline = definitions.SingleOrDefault(definition => definition.Name.Equals(pipelineName, StringComparison.OrdinalIgnoreCase));
        Type ctxType = TypeAndAssemblyParser.Instance.GetType(new TypeAndAssembly(pipeline.ContextType, pipeline.ContextAssembly));
        if (!typeof(PipelineContext).IsAssignableFrom(ctxType))
            throw new PipelineException($"Context Type '{pipeline.ContextType}', Assembly '{pipeline.ContextAssembly}' must be inherited from PipelineContext type.");

        Object context = Activator.CreateInstance(ctxType);
        if (data != null && data.Any()) {
            foreach (KeyValuePair<String, Object> dataItem in data)
                ((PipelineContext) context).Data.Add(dataItem);
        }

        foreach (XmlPipelineActionDefinition action in pipeline.Actions) {
            Type type = TypeAndAssemblyParser.Instance.GetType(new TypeAndAssembly(action.Type, action.Assembly));
            Object instance = Activator.CreateInstance(type);
            MethodInfo methodInfo = instance
                .GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(method => method.Name == action.Method);

            if (methodInfo == null)
                throw new PipelineException($"Method '{action.Method}' was not found. Type '{action.Type}', Assembly '{action.Assembly}'.");

            methodInfo.Invoke(instance, new[] { context });
            if (((PipelineContext) context).Cancelled)
                break;
        }

        return (PipelineContext) context;
    }
}