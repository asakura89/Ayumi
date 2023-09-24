namespace Exy;

// Note: This Exception is to be used in any exception thrown by developer intentionally or by the program to denote any unintended behavior
// Why we are not using defaul InvalidOperationException?
// I have dilemma actually because of the description is not align with the intention to throw exception in any unintended behavior
[Serializable]
public class UnintendedBehaviorException : Exception {
    public UnintendedBehaviorException(String message) : base(message) { }

    public UnintendedBehaviorException(String message, Exception innerException) : base(message, innerException) { }

    public UnintendedBehaviorException() : base() { }
}