using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookstoreApp.Services;
using BookstoreApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BookstoreApp.Models;

namespace BookstoreApp.Controllers
{
    [Authorize]
    [Route("Employee")]
    public class EmployeeController : Controller
    {
        private readonly BookstoreContext _context;
        private readonly IBookService _bookService;
        private readonly IOrderService _orderService;

        public EmployeeController(BookstoreContext context, IBookService bookService, IOrderService orderService)
        {
            _context = context;
            _bookService = bookService;
            _orderService = orderService;
        }

        private bool IsEmployee()
        {
            var userType = User.FindFirst("UserType")?.Value;
            return userType == "Employee" || userType == "Manager";
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            if (!IsEmployee())
            {
                return Forbid();
            }

            var pendingOrders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Oczekujące)
                .CountAsync();

            var todayOrders = await _context.Orders
                .Where(o => o.OrderDetails.Any())
                .CountAsync();

            ViewBag.PendingOrders = pendingOrders;
            ViewBag.TodayOrders = todayOrders;

            return View();
        }

        [HttpGet("Orders")]
        public async Task<IActionResult> ManageOrders()
        {
            if (!IsEmployee())
            {
                return Forbid();
            }

            var orders = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return View(orders);
        }

        [HttpPost("UpdateOrderStatus")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus status)
        {
            if (!IsEmployee())
            {
                return Forbid();
            }

            var success = await _orderService.UpdateOrderStatusAsync(orderId, status);

            if (success)
            {
                TempData["Success"] = "Status zamówienia został zaktualizowany.";
            }
            else
            {
                TempData["Error"] = "Nie udało się zaktualizować statusu zamówienia.";
            }

            return RedirectToAction("ManageOrders");
        }

        [HttpPost("MarkAsShipped/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsShipped(int id)
        {
            if (!IsEmployee())
            {
                return Forbid();
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                TempData["Error"] = "Zamówienie nie zostało znalezione.";
                return RedirectToAction("ManageOrders");
            }

            if (order.Status != OrderStatus.Opłacone)
            {
                TempData["Error"] = "Tylko opłacone zamówienia mogą być oznaczone jako wysłane.";
                return RedirectToAction("ManageOrders");
            }

            order.Status = OrderStatus.Wysłane;

            var employeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var delivery = new Delivery
            {
                OrderId = order.Id,
                EmployeeId = employeeId
            };

            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync();

            delivery.InitializePackages();
            delivery.OrderDelivery();

            TempData["Success"] = $"Zamówienie zostało oznaczone jako wysłane i utworzono dostawę #{delivery.Id}.";
            return RedirectToAction("ManageOrders");
        }

        [HttpPost("CancelOrder/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            if (!IsEmployee())
            {
                return Forbid();
            }

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                TempData["Error"] = "Zamówienie nie zostało znalezione.";
                return RedirectToAction("ManageOrders");
            }

            if (order.Status == OrderStatus.Wysłane || order.Status == OrderStatus.Anulowane)
            {
                TempData["Error"] = "Nie można anulować zamówienia, które zostało już wysłane lub wcześniej anulowane.";
                return RedirectToAction("ManageOrders");
            }

            order.Status = OrderStatus.Anulowane;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Zamówienie zostało anulowane.";
            return RedirectToAction("ManageOrders");
        }

        [HttpGet("OrderDetails/{id}")]
        public async Task<IActionResult> OrderDetails(int id)
        {
            if (!IsEmployee())
            {
                return Forbid();
            }

            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet("Deliveries")]
        public async Task<IActionResult> ManageDeliveries()
        {
            if (!IsEmployee())
            {
                return Forbid();
            }

            var employeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var deliveries = User.FindFirst("UserType")?.Value == "Manager"
                ? await _context.Deliveries
                    .Include(d => d.Order)
                    .ThenInclude(o => o.Client)
                    .Include(d => d.Order.OrderDetails)
                    .ThenInclude(od => od.Book)
                    .Include(d => d.Employee)
                    .OrderByDescending(d => d.Id)
                    .ToListAsync()
                : await _context.Deliveries
                    .Where(d => d.EmployeeId == employeeId)
                    .Include(d => d.Order)
                    .ThenInclude(o => o.Client)
                    .Include(d => d.Order.OrderDetails)
                    .ThenInclude(od => od.Book)
                    .Include(d => d.Employee)
                    .OrderByDescending(d => d.Id)
                    .ToListAsync();

            return View(deliveries);
        }

        [HttpGet("DeliveryDetails/{id}")]
        public async Task<IActionResult> DeliveryDetails(int id)
        {
            if (!IsEmployee())
            {
                return Forbid();
            }

            var employeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var isManager = User.FindFirst("UserType")?.Value == "Manager";

            var delivery = await _context.Deliveries
                .Include(d => d.Order)
                .ThenInclude(o => o.Client)
                .Include(d => d.Order.OrderDetails)
                .ThenInclude(od => od.Book)
                .Include(d => d.Employee)
                .FirstOrDefaultAsync(d => d.Id == id && (isManager || d.EmployeeId == employeeId));

            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        [HttpPost("CompleteDelivery/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteDelivery(int id)
        {
            if (!IsEmployee())
            {
                return Forbid();
            }

            var employeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var isManager = User.FindFirst("UserType")?.Value == "Manager";

            var delivery = await _context.Deliveries
                .Include(d => d.Order)
                .FirstOrDefaultAsync(d => d.Id == id && (isManager || d.EmployeeId == employeeId));

            if (delivery == null)
            {
                TempData["Error"] = "Dostawa nie została znaleziona.";
                return RedirectToAction("ManageDeliveries");
            }

            if (delivery.Order.Status != OrderStatus.Opłacone)
            {
                TempData["Error"] = "Można realizować tylko opłacone zamówienia.";
                return RedirectToAction("ManageDeliveries");
            }

            delivery.Order.Status = OrderStatus.Wysłane;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Dostawa #{delivery.Id} została zrealizowana. Zamówienie oznaczone jako wysłane.";
            return RedirectToAction("ManageDeliveries");
        }
    }
}