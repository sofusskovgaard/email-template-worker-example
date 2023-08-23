using MailTemplate.Worker.Commands;
using MailTemplate.Worker.Services;
using MailTemplate.Worker.Templates;
using MailTemplate.Worker.Templates.Subjects;
using MimeKit;
using MimeKit.Text;

namespace MailTemplate.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ISmtpClientFactory _smtpClientFactory;

        private readonly IEmailGenerator _emailGenerator;

        public Worker(ISmtpClientFactory smtpClientFactory, IEmailGenerator emailGenerator)
        {
            this._smtpClientFactory = smtpClientFactory;
            this._emailGenerator = emailGenerator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var command = new SendEmailCommand()
            {
                Type = "HelloWorld",
                BodyProperties = new Dictionary<string, object>()
                {
                    { "Name", "Sofus" }
                },
                SubjectProperties = new Dictionary<string, object>()
                {
                    { "Name", "Sofus" }
                },
                Recipients = new List<RecipientRecord>()
                {
                    new("Sofus Skovgaard", "hello@sofusskovgaard.com")
                }
            };

            var messages = new List<MimeMessage>();

            foreach (var recipient in command.Recipients)
            {
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress("no-reply", "no-reply@sofusskovgaard.com"));

                message.To.Add(new MailboxAddress(recipient.name, recipient.email));
                message.Subject = await this._emailGenerator.GenerateSubject<HelloWorldSubjectModel>(command);

                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = await this._emailGenerator.GenerateBody<HelloWorldModel>(command)
                };

                messages.Add(message);
            }

            var client = await this._smtpClientFactory.Create();

            foreach (var message in messages)
            {
                await client.SendAsync(message, stoppingToken);
            }
        }
    }
}