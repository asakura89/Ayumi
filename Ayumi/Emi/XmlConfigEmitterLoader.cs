using System.Reflection;
using Reflx;

namespace Emi;

public class XmlConfigEmitterLoader : IEmitterLoader {
    readonly String configPath;
    readonly IEmitter globalEmitter;
    readonly ITypeAndAssemblyParser typeNAsmParser;

    public XmlConfigEmitterLoader(ITypeAndAssemblyParser typeNAsmParser) : this(typeNAsmParser, $"{AppDomain.CurrentDomain.BaseDirectory}\\emitter.config.xml") { }

    public XmlConfigEmitterLoader(ITypeAndAssemblyParser typeNAsmParser, String configPath) {
        this.typeNAsmParser = typeNAsmParser ?? throw new ArgumentNullException(nameof(typeNAsmParser));
        this.configPath = configPath ?? throw new ArgumentNullException(nameof(configPath));
        globalEmitter = new Emitter();
    }

    public IEmitter Load() {
        IEnumerable<XmlEventDefinition> events = XmlEventDefinitionLoader.Load(configPath);
        foreach (XmlEventDefinition definition in events) {
            Type type = typeNAsmParser.GetType(new TypeAndAssembly(definition.Type, definition.Assembly));
            Object instance = Activator.CreateInstance(type);
            MethodInfo methodInfo = instance
                .GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(method => method.Name == definition.Method);

            if (methodInfo == null)
                throw new EmitterException($"Method '{definition.Method}' was not found. Type '{definition.Type}', Assembly '{definition.Assembly}'.");

            var eventDelegate = (Action<EmitterEventArgs>) Delegate.CreateDelegate(typeof(Action<EmitterEventArgs>), instance, methodInfo);
            if (definition.OnlyOnce)
                globalEmitter.Once(definition.Name, eventDelegate);
            else
                globalEmitter.On(definition.Name, eventDelegate);
        }

        return globalEmitter;
    }
}