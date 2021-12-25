namespace Emi;

public class Emitter : IEmitter {
    readonly Object eLock = new Object();
    readonly Dictionary<String, IList<Action<EmitterEventArgs>>> e = new Dictionary<String, IList<Action<EmitterEventArgs>>>();

    public Int32 Count {
        get {
            lock (eLock)
                return e.Count;
        }
    }

    public IEmitter On(String name, Action<EmitterEventArgs> callback) {
        if (String.IsNullOrEmpty(name))
            throw new EmitterException("Name must be specified.");

        if (callback == null)
            throw new EmitterException("Invalid callback.");

        lock (eLock) {
            if (!e.ContainsKey(name))
                e.Add(name, new List<Action<EmitterEventArgs>>());

            e[name].Add(callback);
        }

        return this;
    }

    public IEmitter Off(String name) => Off(name, null);

    public IEmitter Off(String name, Action<EmitterEventArgs> callback) {
        if (String.IsNullOrEmpty(name))
            throw new EmitterException("Name must be specified.");

        lock (eLock) {
            if (!e.ContainsKey(name))
                return this;

            if (callback == null) {
                e.Remove(name);
                return this;
            }

            IList<Action<EmitterEventArgs>> callbacks = e[name];
            IList<Action<EmitterEventArgs>> liveCallbacks = callbacks
                .Where(callb => !callb.Equals(callback))
                .ToList();

            if (liveCallbacks.Any())
                e[name] = liveCallbacks;
            else
                e.Remove(name);
        }

        return this;
    }

    public IEmitter Once(String name, Action<EmitterEventArgs> callback) {
        if (String.IsNullOrEmpty(name))
            throw new EmitterException("Name must be specified.");

        if (callback == null)
            throw new EmitterException("Invalid callback.");

        IEmitter self = this;
        Action<EmitterEventArgs> wrapper = null;
        wrapper = arg => {
            self.Off(name, wrapper);
            callback.Invoke(arg);
        };

        return self.On(name, wrapper);
    }

    public IEmitter Emit(String name, EmitterEventArgs arg) {
        if (String.IsNullOrEmpty(name))
            throw new EmitterException("Name must be specified.");

        if (!e.ContainsKey(name))
            return this;

        IList<Action<EmitterEventArgs>> callbacks = e[name];
        foreach (Action<EmitterEventArgs> callback in callbacks)
            callback.Invoke(arg);

        return this;
    }
}