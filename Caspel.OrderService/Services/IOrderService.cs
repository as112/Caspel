using Caspel.Core.Models;
using Caspel.OrderService.Dto;

namespace Caspel.OrderService.Services
{
    public interface IOrderService
    {
        Task<Guid> CreateOrderAsync(OrderDto orderDto);
        Task<Order?> GetOrderByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
    }
}
