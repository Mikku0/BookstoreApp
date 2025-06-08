using BookstoreApp.Data;
using BookstoreApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BookstoreApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly BookstoreContext _context;

        public OrderService(BookstoreContext context)
        {
            _context = context;
        }

        public async Task<Order?> CreateOrderAsync(int clientId, List<(int bookId, int quantity)> items)
        {
            var client = await _context.Clients.FindAsync(clientId);
            if (client == null) return null;

            var order = new Order
            {
                ClientId = clientId,
                Status = OrderStatus.Oczekujące
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            decimal totalPrice = 0;
            foreach (var (bookId, quantity) in items)
            {
                var book = await _context.Books.FindAsync(bookId);
                if (book == null) continue;

                var orderDetail = new OrderDetails
                {
                    OrderId = order.Id,
                    BookId = bookId,
                    Quantity = quantity,
                    Date = DateTime.Now
                };

                _context.OrderDetails.Add(orderDetail);
                totalPrice += book.Price * quantity;
            }

            order.TotalPrice = totalPrice;
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersByClientAsync(int clientId)
        {
            return await _context.Orders
                .Where(o => o.ClientId == clientId)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}