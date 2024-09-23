using Caspel.Core.Interfaces;
using Caspel.Core.Models;
using Caspel.OrderService.Data;
using Caspel.OrderService.Dto;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Caspel.OrderService.Services
{
    public class OrderServiceImpl : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderServiceImpl> _logger;

        public OrderServiceImpl(AppDbContext context, IPublishEndpoint publishEndpoint, ILogger<OrderServiceImpl> logger)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<Guid> CreateOrderAsync(OrderDto orderDto)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                ProductName = orderDto.ProductName,
                Quantity = orderDto.Quantity,
                CreatedAt = DateTimeOffset.Now
            };

            try
            {
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                await _publishEndpoint.Publish<OrderCreated>(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Guid.Empty;
            }

            return order.Id;
        }

        public async Task<Order?> GetOrderByIdAsync(Guid id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            try
            {
                return await _context.Orders.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Enumerable.Empty<Order>();
            }
        }
    }
}
