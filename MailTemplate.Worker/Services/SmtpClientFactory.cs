using MailKit;
using MailKit.Net.Smtp;

namespace MailTemplate.Worker.Services;

public class SmtpClientFactory : ISmtpClientFactory, IDisposable
{
    private readonly ILogger<SmtpClientFactory> _logger;

    private readonly IConfiguration _configuration;

    public SmtpClientFactory(ILogger<SmtpClientFactory> logger, IConfiguration configuration)
    {
        this._logger = logger;
        this._configuration = configuration;
    }

    private SmtpClient? _client;

    public async ValueTask<SmtpClient> Create()
    {
        if (this._client is { IsConnected: true, IsAuthenticated: true })
        {
            return this._client;
        }

        this._client = new SmtpClient();

        this._client.Connected += ClientOnConnected;
        this._client.Disconnected += ClientOnDisconnected;
        this._client.MessageSent += ClientOnMessageSent;

        await this._client.ConnectAsync(
            this._configuration["SMTP-HOST"],
            this._configuration.GetValue<int>("SMTP-PORT"),
            false
        );

        await this._client.AuthenticateAsync(
            this._configuration["SMTP-USERNAME"],
            this._configuration["SMTP-PASSWORD"]
        );

        return this._client;
    }

    private void ClientOnMessageSent(object? sender, MessageSentEventArgs e)
    {
        this._logger.LogInformation("SMTP Client sent message and received {response}", e.Response);
    }

    private void ClientOnDisconnected(object? sender, DisconnectedEventArgs e)
    {
        this._logger.LogInformation("SMTP Client disconnected");
    }

    private void ClientOnConnected(object? sender, ConnectedEventArgs e)
    {
        this._logger.LogInformation("SMTP Client connected");
    }

    public void Dispose()
    {
        this._client?.Dispose();
    }
}

public interface ISmtpClientFactory
{
    ValueTask<SmtpClient> Create();
}