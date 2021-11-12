namespace Emi;

public class EmitterEventArgs : EventArgs {
    public String EventName { get; set; }
    public IDictionary<String, Object> Data { get; set; } = new Dictionary<String, Object>();

    public EmitterEventArgs(String eventName) {
        if (String.IsNullOrEmpty(eventName))
            throw new ArgumentNullException(nameof(eventName));

        EventName = eventName;
    }

    public EmitterEventArgs(String eventName, IDictionary<String, Object> data) {
        if (String.IsNullOrEmpty(eventName))
            throw new ArgumentNullException(nameof(eventName));

        EventName = eventName;
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }
}