namespace BookstoreApp.Services
{
    public interface IOrderService
    {
        Task<Models.Order?> CreateOrderAsync(int clientId, List<(int bookId, int quantity)> items);
        Task<IEnumerable<Models.Order>> GetOrdersByClientAsync(int clientId);
        Task<bool> UpdateOrderStatusAsync(int orderId, Models.OrderStatus status);
    }
}