using System.Text;

namespace AppSea;

[Serializable]
public class BadConfigurationException : Exception {
    const String ExcMessage = "Configuration \"{0}\" is not configured correctly.";

    public BadConfigurationException(String configurationName) : base(String.Format(ExcMessage, configurationName)) { }

    public BadConfigurationException(String configurationName, Exception innerException) : base(String.Format(ExcMessage, configurationName), innerException) { }

    public BadConfigurationException(String configurationName, String customMessage) : base(
        new StringBuilder()
            .AppendFormat(ExcMessage, configurationName)
            .Append(' ')
            .Append(customMessage)
            .ToString()) { }

    public BadConfigurationException(String configurationName, String customMessage, Exception innerException) : base(
        new StringBuilder()
            .AppendFormat(ExcMessage, configurationName)
            .Append(' ')
            .Append(customMessage)
            .ToString(), innerException) { }

    public BadConfigurationException() : base() { }
}