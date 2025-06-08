using BookstoreApp.Models;

namespace BookstoreApp.Services
{
    public interface IOrderService
    {
        Task<Models.Order?> CreateOrderAsync(int clientId, List<(int bookId, int quantity)> items);
        Task<IEnumerable<Models.Order>> GetOrdersByClientAsync(int clientId);
        Task<bool> UpdateOrderStatusAsync(int orderId, Models.OrderStatus status);
        Task<IEnumerable<Models.Order>> GetOrdersByStatusAsync(Models.OrderStatus status);
        Task<Models.Order?> GetOrderWithDetailsAsync(int orderId);
        Task<bool> CanCancelOrderAsync(int orderId, int clientId);
        Task<IEnumerable<Models.Order>> GetAllOrdersAsync();
    }
}