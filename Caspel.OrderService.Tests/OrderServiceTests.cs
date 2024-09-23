using Caspel.Core.Interfaces;
using Caspel.Core.Models;
using Caspel.OrderService.Data;
using Caspel.OrderService.Dto;
using Caspel.OrderService.Services;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;


namespace Caspel.OrderService.Tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        [Test]
        public async Task CreateOrder_ShouldSaveOrder()
        {
            // Arrange
            var webHost = new WebApplicationFactory<Caspel.OrderService.Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddMassTransitTestHarness();
                    services.RemoveAll<DbContextOptions<AppDbContext>>();
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("test1_db");
                    });

                    var publishEndpoint = services.SingleOrDefault(d => d.ServiceType == typeof(IPublishEndpoint));
                    services.Remove(publishEndpoint);
                    var mockPublishEndpoint = new Mock<IPublishEndpoint>();
                    mockPublishEndpoint.Setup(_ => _.Publish(It.IsAny<OrderCreated>(), CancellationToken.None)).Returns(Task.CompletedTask);

                    services.AddTransient(_ => mockPublishEndpoint.Object);
                });
            });

            var orderService = webHost.Services.CreateScope().ServiceProvider.GetRequiredService<IOrderService>();
            var dbContext = webHost.Services.CreateScope().ServiceProvider.GetService<AppDbContext>();

            var orderDto = new OrderDto
            {
                ProductName = "Test Product",
                Quantity = 2,
            };

            // Act
            var orderId = await orderService.CreateOrderAsync(orderDto);
           
            var savedOrder = dbContext.Orders.First(x => x.Id == orderId);

            // Assert
            Assert.That(savedOrder, Is.Not.Null);
            Assert.That(savedOrder.Id, Is.EqualTo(orderId));
        }

        [Test]
        public async Task GetOrderById_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var webHost = new WebApplicationFactory<Caspel.OrderService.Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddMassTransitTestHarness();
                    services.RemoveAll<DbContextOptions<AppDbContext>>();
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("test2_db");
                    });
                });
            });

            var orderService = webHost.Services.CreateScope().ServiceProvider.GetRequiredService<IOrderService>();
            var dbContext = webHost.Services.CreateScope().ServiceProvider.GetService<AppDbContext>();

            var order = new Order
            {
                Id = Guid.NewGuid(),
                ProductName = "Test Product",
                Quantity = 2,
                CreatedAt = DateTime.UtcNow
            };

            await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();

            // Act
            var retrievedOrder = await orderService.GetOrderByIdAsync(order.Id);

            // Assert
            Assert.That(retrievedOrder, Is.Not.Null);
            Assert.That(retrievedOrder, Is.EqualTo(order));

            dbContext = null;
            orderService = null;
        }

        [Test]
        public async Task GetOrderById_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var webHost = new WebApplicationFactory<Caspel.OrderService.Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddMassTransitTestHarness();
                    services.RemoveAll<DbContextOptions<AppDbContext>>();
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("test3_db");
                    });
                });
            });

            var orderService = webHost.Services.CreateScope().ServiceProvider.GetRequiredService<IOrderService>();
            var dbContext = webHost.Services.CreateScope().ServiceProvider.GetService<AppDbContext>();

            // Act
            var retrievedOrder = await orderService.GetOrderByIdAsync(Guid.NewGuid());

            // Assert
            Assert.That(retrievedOrder, Is.Null);
        }

        [Test]
        public async Task GetAllOrders_ShouldReturnAllOrders_WhenOrdersExist()
        {
            // Arrange
            var webHost = new WebApplicationFactory<Caspel.OrderService.Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddMassTransitTestHarness();
                    services.RemoveAll<DbContextOptions<AppDbContext>>();
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("test4_db");
                    });
                });
            });

            var orderService = webHost.Services.CreateScope().ServiceProvider.GetRequiredService<IOrderService>();
            var dbContext = webHost.Services.CreateScope().ServiceProvider.GetService<AppDbContext>();

            var orders = new[]
            {
                new Order
                {
                    Id = Guid.NewGuid(),
                    ProductName = "Product 1",
                    Quantity = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    ProductName = "Product 2",
                    Quantity = 3,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await dbContext.Orders.AddRangeAsync(orders);
            await dbContext.SaveChangesAsync();

            // Act
            var retrievedOrders = await orderService.GetAllOrdersAsync();

            // Assert
            Assert.That(retrievedOrders.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllOrders_ShouldReturnEmpty_WhenNoOrdersExist()
        {
            // Arrange
            var webHost = new WebApplicationFactory<Caspel.OrderService.Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddMassTransitTestHarness();
                    services.RemoveAll<DbContextOptions<AppDbContext>>();
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("test5_db");
                    });
                });
            });

            var orderService = webHost.Services.CreateScope().ServiceProvider.GetRequiredService<IOrderService>();

            // Act
            var retrievedOrders = await orderService.GetAllOrdersAsync();

            // Assert
            Assert.That(retrievedOrders.Count() == 0, Is.True);
        }
    }
}
