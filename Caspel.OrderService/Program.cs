using Caspel.OrderService.Data;
using Caspel.OrderService.Dto;
using Caspel.OrderService.Middleware;
using Caspel.OrderService.Services;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Caspel.OrderService;

public class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("OrderDb"));

        builder.Services.AddOptions<RabbitMqTransportOptions>()
            .BindConfiguration("RabbitMqOptions");

        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<RabbitMqTransportOptions>>().Value;
                cfg.Host(options.Host, options.Port, options.VHost, h =>
                {
                    h.Username(options.User);
                    h.Password(options.Pass);
                });
                cfg.ConfigureEndpoints(context);
            });
        });

        builder.Services.AddScoped<IOrderService, OrderServiceImpl>();

        var app = builder.Build();

        app.UseMiddleware<ApiKeyMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapPost("/api/create-order", async (IOrderService orderService, [FromBody] OrderDto orderDto) =>
        {
            var orderId = await orderService.CreateOrderAsync(orderDto);
            return orderId == Guid.Empty ? Results.StatusCode(StatusCodes.Status500InternalServerError) : Results.Ok(orderId);
        });

        app.MapGet("/api/get-order/{id}", async (IOrderService orderService, Guid id) =>
        {
            var order = await orderService.GetOrderByIdAsync(id);
            return order is null ? Results.NotFound() : Results.Ok(order);
        });

        app.MapGet("/api/get-orders", async (IOrderService orderService) =>
        {
            var orders = await orderService.GetAllOrdersAsync();
            return orders.Any() ? Results.Ok(orders) : Results.NotFound();
        });

        app.Run();

    }
}

