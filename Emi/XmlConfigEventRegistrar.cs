using System.Reflection;
using Reflx;

namespace Emi;

public class XmlConfigEventRegistrar : IEventRegistrar {
    readonly String configPath;
    readonly IDictionary<String, EventInfo> handlers;
    readonly ITypeAndAssemblyParser typeNAsmParser;

    public XmlConfigEventRegistrar(ITypeAndAssemblyParser typeNAsmParser) : this(typeNAsmParser, $"{AppDomain.CurrentDomain.BaseDirectory}\\emitter.config.xml") { }

    public XmlConfigEventRegistrar(ITypeAndAssemblyParser typeNAsmParser, String configPath) {
        this.typeNAsmParser = typeNAsmParser ?? throw new ArgumentNullException(nameof(typeNAsmParser));
        this.configPath = configPath ?? throw new ArgumentNullException(nameof(configPath));
        handlers = new Dictionary<String, EventInfo>();
    }

    public void Register(Object classWithHandlers) {
        Type classWithHandlersType = classWithHandlers.GetType();
        EventInfo[] eventHandlers = classWithHandlersType.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (EventInfo handler in eventHandlers) {
            String handlerName = classWithHandlersType.Name + "." + handler.Name;
            if (handlers.ContainsKey(handlerName))
                handlers[handlerName] = handler;
            else
                handlers.Add(handlerName, handler);
        }

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

            EventInfo handler = handlers[definition.Name];
            var eventDelegate = Delegate.CreateDelegate(handler.EventHandlerType, instance, methodInfo);

            MethodInfo addHandler = handler.GetAddMethod(true);
            if (definition.OnlyOnce) {
                EventHandler<EmitterEventArgs> wrapper = null;
                Delegate wrapperDelegate = null;
                wrapper = (source, args) => {
                    Delegate localWrapperDelegate = wrapperDelegate;
                    Object localEventSource = classWithHandlers;
                    EventInfo localHandler = handler;
                    localHandler.GetRemoveMethod(true).Invoke(localEventSource, new[] { localWrapperDelegate });
                    Delegate localDelegate = eventDelegate;
                    localDelegate.DynamicInvoke(source, args);
                };
                wrapperDelegate = Delegate.CreateDelegate(handler.EventHandlerType, wrapper.Target, wrapper.Method);
                addHandler.Invoke(classWithHandlers, new Object[] { wrapperDelegate });
            }
            else
                addHandler.Invoke(classWithHandlers, new Object[] { eventDelegate });
        }
    }
}