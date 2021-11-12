namespace Ria;

public class PipelineException : Exception {
    public PipelineException(String message) : base(message) { }

    public PipelineException(String message, Exception innerException) : base(message, innerException) { }

    public PipelineException() : base() { }
}