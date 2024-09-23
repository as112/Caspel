using System.Text.Json;

namespace Caspel.Core.Services;

public class NoOpEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Console.WriteLine($"""
            Email was sent:
            ---> email: {email}
            ---> subgect: {subject}

            {htmlMessage}
            """);
        return Task.CompletedTask;
    }
}
