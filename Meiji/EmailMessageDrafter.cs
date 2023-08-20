using System.Net.Mail;

namespace Meiji;

public class EmailMessageDrafter {
    readonly MailMessage message = new MailMessage();

    public EmailMessageDrafter CreateDraft(String from, String to, String subject, String body, Boolean useHtml = true) {
        message.From = new MailAddress(from);
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = useHtml;
        message.To.Add(new MailAddress(to));

        return this;
    }

    public EmailMessageDrafter AddTo(String to) {
        message.To.Add(new MailAddress(to));

        return this;
    }

    public EmailMessageDrafter AddTos(IEnumerable<String> tos) {
        foreach (String to in tos)
            message.To.Add(new MailAddress(to));

        return this;
    }

    public EmailMessageDrafter AddCc(String cc) {
        message.CC.Add(new MailAddress(cc));

        return this;
    }

    public EmailMessageDrafter AddCcs(IEnumerable<String> ccs) {
        foreach (String cc in ccs)
            message.CC.Add(new MailAddress(cc));

        return this;
    }

    public EmailMessageDrafter AddBcc(String bcc) {
        message.Bcc.Add(new MailAddress(bcc));

        return this;
    }

    public EmailMessageDrafter AddBccs(IEnumerable<String> bccs) {
        foreach (String bcc in bccs)
            message.Bcc.Add(new MailAddress(bcc));

        return this;
    }

    public EmailMessageDrafter AddAttachment(String filePath) {
        var fileInfo = new FileInfo(filePath);
        message.Attachments.Add(new Attachment(fileInfo.OpenRead(), fileInfo.Name));

        return this;
    }

    public EmailMessageDrafter AddAttachments(IEnumerable<String> filePaths) {
        foreach (String path in filePaths) {
            var fileInfo = new FileInfo(path);
            message.Attachments.Add(new Attachment(fileInfo.OpenRead(), fileInfo.Name));
        }

        return this;
    }

    public MailMessage GetDraft() => message;
}