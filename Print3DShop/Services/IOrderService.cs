using Print3DShop.Models;

namespace Print3DShop.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Order order, string cartId);
        Task<Order> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task UpdateOrderStatusAsync(int id, string status);
        Task UpdateTrackingNumberAsync(int id, string trackingNumber);
    }
}