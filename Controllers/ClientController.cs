using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookstoreApp.Services;
using BookstoreApp.Data;
using BookstoreApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookstoreApp.Controllers
{
    [Authorize]
    [Route("Client")]
    public class ClientController : Controller
    {
        private readonly BookstoreContext _context;
        private readonly IBookService _bookService;
        private readonly IOrderService _orderService;

        public ClientController(BookstoreContext context, IBookService bookService, IOrderService orderService)
        {
            _context = context;
            _bookService = bookService;
            _orderService = orderService;
        }

        private bool IsClient()
        {
            return User.FindFirst("UserType")?.Value == "Client";
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            if (!IsClient())
            {
                return Forbid();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var orders = await _orderService.GetOrdersByClientAsync(userId);

            ViewBag.RecentOrders = orders.Take(5).ToList();
            ViewBag.TotalOrders = orders.Count();

            return View();
        }

        [HttpGet("Orders")]
        public async Task<IActionResult> MyOrders()
        {
            if (!IsClient())
            {
                return Forbid();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var orders = await _orderService.GetOrdersByClientAsync(userId);

            return View(orders);
        }

        [HttpGet("OrderDetails/{id}")]
        public async Task<IActionResult> OrderDetails(int id)
        {
            if (!IsClient())
            {
                return Forbid();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .FirstOrDefaultAsync(o => o.Id == id && o.ClientId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> Profile()
        {
            if (!IsClient())
            {
                return Forbid();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var client = await _context.Clients.FindAsync(userId);

            return View(client);
        }
        [HttpPost("CancelOrder/{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            if (!IsClient())
            {
                return Forbid();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.ClientId == userId);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatus.Oczekujące)
            {
                TempData["Error"] = "Nie można anulować tego zamówienia.";
                return RedirectToAction("MyOrders");
            }

            order.Status = OrderStatus.Anulowane;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Zamówienie zostało anulowane.";
            return RedirectToAction("MyOrders");
        }

        [HttpPost("PayForOrder/{id}")]
        public async Task<IActionResult> PayForOrder(int id)
        {
            if (!IsClient())
            {
                return Forbid();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.ClientId == userId);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatus.Oczekujące)
            {
                TempData["Error"] = "Nie można opłacić tego zamówienia.";
                return RedirectToAction("MyOrders");
            }

            order.Status = OrderStatus.Opłacone;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Zamówienie zostało opłacone.";
            return RedirectToAction("MyOrders");
        }

        [HttpGet("OrderHistory")]
        public async Task<IActionResult> OrderHistory()
        {
            if (!IsClient())
            {
                return Forbid();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var orders = await _context.Orders
                .Where(o => o.ClientId == userId)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return View(orders);
        }
    }
}