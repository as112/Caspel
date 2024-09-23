using Caspel.Core.Services;
using Caspel.NotificationService.Consumers;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Caspel.NotificationService;

public class Program
{
    static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddScoped<IEmailSender, NoOpEmailSender>();

        builder.Services.AddOptions<RabbitMqTransportOptions>()
            .BindConfiguration("RabbitMqOptions");

        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderCreatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<RabbitMqTransportOptions>>().Value;
                cfg.Host(options.Host, options.Port, options.VHost, h =>
                {
                    h.Username(options.User);
                    h.Password(options.Pass);
                });
                cfg.ConfigureEndpoints(context);
                Console.WriteLine();
            });
        });

        var host = builder.Build();
        host.Run();
    }
}


