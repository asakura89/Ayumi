namespace Meutia;

public class ServiceInstanceNotFoundException : Exception {
    public ServiceInstanceNotFoundException(String message) : base(message) { }

    public ServiceInstanceNotFoundException(String message, Exception innerException) : base(message, innerException) { }

    public ServiceInstanceNotFoundException() : base() { }
}