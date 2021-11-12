namespace AppSea;

[Serializable]
public class BadConfigurationException : Exception {
    const String ExcMessage = "Configuration \"{0}\" is not configured correctly.";

    public BadConfigurationException(String configurationName) : base(String.Format(ExcMessage, configurationName)) { }

    public BadConfigurationException(String configurationName, Exception innerException) : base(String.Format(ExcMessage, configurationName), innerException) { }

    public BadConfigurationException() : base() { }
}