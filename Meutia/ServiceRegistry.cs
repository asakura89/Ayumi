using System.Reflection;

namespace Meutia;

public class ServiceRegistry : IServiceRegistry {
    readonly IDictionary<Type, Func<Object>> types = new Dictionary<Type, Func<Object>>();
    readonly IDictionary<String, Type> typesByName = new Dictionary<String, Type>(StringComparer.InvariantCultureIgnoreCase);

    Object CreateInstance(Type implType) {
        ConstructorInfo ctor = implType.GetConstructors()[0];
        ParameterInfo[] ctorParams = ctor.GetParameters();
        if (ctorParams.Length == 0)
            return Activator.CreateInstance(implType);

        Object[] parameters = ctorParams
            .Select(param => param.ParameterType)
            .Select(GetInstance)
            .ToArray();

        return Activator.CreateInstance(implType, parameters);
    }

    Object GetInstance(Type abstractType) {
        if (types.TryGetValue(abstractType, out Func<Object> creator))
            return creator();

        if (!abstractType.IsAbstract)
            return CreateInstance(abstractType);

        return null;
    }

    public Object TryGetService(Type serviceType) => GetInstance(serviceType);

    public Object GetService(Type serviceType) {
        Object instance = TryGetService(serviceType);
        if (instance == null)
            throw new ServiceInstanceNotFoundException(serviceType.FullName);

        return instance;
    }

    public Object GetService(String name) {
        if (!typesByName.ContainsKey(name))
            throw new ServiceInstanceNotFoundException(name);

        Type type = typesByName[name];
        return GetService(type);
    }

    public I GetService<I>() => (I) GetService(typeof(I));

    public I GetService<I>(String name) => (I) GetService(name);

    public void RegisterService<A, I>() where I : A => types[typeof(A)] = () => GetInstance(typeof(I));

    public void RegisterService<A, I>(String name) where I : A => typesByName[name] = typeof(I);

    public void RegisterService<I>(Func<I> instanceCreator) => types[typeof(I)] = () => instanceCreator();

    public void RegisterService<I>(Func<I> instanceCreator, String name) {
        typesByName[name] = typeof(I);
        types[typeof(I)] = () => instanceCreator();
    }

    public void RegisterService<I>(I instance) => types[typeof(I)] = () => instance;

    public void RegisterService<I>(I instance, String name) {
        typesByName[name] = typeof(I);
        types[typeof(I)] = () => instance;
    }
}