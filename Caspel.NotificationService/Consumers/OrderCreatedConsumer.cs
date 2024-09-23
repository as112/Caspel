using Caspel.Core.Interfaces;
using Caspel.Core.Services;
using MassTransit;
using System.Text.Json;

namespace Caspel.NotificationService.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreated>
    {
        private readonly ILogger<OrderCreatedConsumer> _logger;
        private readonly IEmailSender _emailSender;

        public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }
        public async Task Consume(ConsumeContext<OrderCreated> context)
        {
            var email = "cook@cook.com";
            var subject = "New order has been created";
            var html = $"<div>ProductName: {context.Message.ProductName}<div>\n";
            html += $"<div>Quantity: {context.Message.Quantity}<div>\n";
            html += $"<div>CreatedAt: {context.Message.CreatedAt}<div>\n";

            await _emailSender.SendEmailAsync(email, subject, html);

            _logger.LogInformation($"Order Created: {JsonSerializer.Serialize(context.Message)}");
        }
    }
}
