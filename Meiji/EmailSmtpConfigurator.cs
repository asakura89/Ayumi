using System.Net;
using System.Net.Mail;

namespace Meiji;

public class EmailSmtpConfigurator {
    readonly SmtpClient smtp = new SmtpClient();

    public EmailSmtpConfigurator CreateConfiguration(String host, Int32 port, Boolean useSsl = true) {
        smtp.Host = host;
        smtp.Port = port;
        smtp.EnableSsl = useSsl;

        return this;
    }

    public EmailSmtpConfigurator UseCredentials(String username, String password) {
        smtp.Credentials = new NetworkCredential(username, password);

        return this;
    }

    public EmailSmtpConfigurator SetDeliveryMethod(SmtpDeliveryMethod method) {
        smtp.DeliveryMethod = method;

        return this;
    }

    public EmailSmtpConfigurator SetTimeout(Int32 timeout) {
        smtp.Timeout = timeout;

        return this;
    } 

    public SmtpClient GetConfiguration() => smtp;
}