using MailTemplate.Worker;
using MailTemplate.Worker.Services;
using RazorLight;
using RazorLight.Extensions;

var builder = new HostApplicationBuilder();

builder.Services.AddRazorLight(() => new RazorLightEngineBuilder()
    .UseFileSystemProject(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates"))
    .UseMemoryCachingProvider()
    .Build());

builder.Services.AddSingleton<ISmtpClientFactory, SmtpClientFactory>();
builder.Services.AddSingleton<IEmailGenerator, EmailGenerator>();

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

await app.RunAsync();
