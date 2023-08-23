namespace MailTemplate.Worker.Commands;

public class SendEmailCommand
{
    public string Type { get; set; }

    public List<RecipientRecord> Recipients { get; set; }

    public Dictionary<string, object> SubjectProperties { get; set; }

    public Dictionary<string, object> BodyProperties { get; set; }
}

public record RecipientRecord(string name, string email);