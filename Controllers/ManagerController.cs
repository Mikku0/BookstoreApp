using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookstoreApp.Services;
using BookstoreApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BookstoreApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookstoreApp.Controllers
{
    [Authorize]
    [Route("Manager")]
    public class ManagerController : Controller
    {
        private readonly BookstoreContext _context;
        private readonly IBookService _bookService;
        private readonly IOrderService _orderService;

        public ManagerController(BookstoreContext context, IBookService bookService, IOrderService orderService)
        {
            _context = context;
            _bookService = bookService;
            _orderService = orderService;
        }

        private bool IsManager()
        {
            return User.FindFirst("UserType")?.Value == "Manager";
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            if (!IsManager())
            {
                return Forbid();
            }

            var totalRevenue = await _context.Orders
                .Where(o => o.Status == OrderStatus.Wysłane)
                .SumAsync(o => o.TotalPrice);

            var monthlyRevenue = await _context.Orders
                .Where(o => o.Status == OrderStatus.Wysłane &&
                           o.OrderDetails.Any(od => od.Date.Month == DateTime.Now.Month))
                .SumAsync(o => o.TotalPrice);

            var totalOrders = await _context.Orders.CountAsync();
            var totalCustomers = await _context.Clients.CountAsync();

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalCustomers = totalCustomers;

            return View();
        }

        [HttpGet("Reports")]
        public async Task<IActionResult> Reports()
        {
            if (!IsManager())
            {
                return Forbid();
            }

            var salesReport = await _context.OrderDetails
                .Include(od => od.Book)
                .GroupBy(od => od.Book.Name)
                .Select(g => new
                {
                    BookName = g.Key,
                    TotalQuantity = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.Quantity * od.Book.Price)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .ToListAsync();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var ownReports = await _context.Reports
                .Where(r => r.ManagerId == userId)
                .OrderByDescending(r => r.CreationDate)
                .ToListAsync();

            ViewBag.SalesReport = salesReport;
            ViewBag.OwnReports = ownReports;

            return View();
        }


        [HttpGet("Users")]
        public async Task<IActionResult> ManageUsers()
        {
            if (!IsManager())
            {
                return Forbid();
            }

            var users = await _context.Users.ToListAsync();
            return View(users);
        }



        [HttpPost("Books/Delete/{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (!IsManager())
            {
                return Forbid();
            }

            var success = await _bookService.DeleteBookAsync(id);

            if (success)
            {
                TempData["Success"] = "Książka została usunięta.";
            }
            else
            {
                TempData["Error"] = "Nie udało się usunąć książki.";
            }

            return RedirectToAction("ManageBooks", "Manager");
        }


        [HttpGet("Reports/History")]
        public async Task<IActionResult> ViewReports()
        {
            if (!IsManager())
            {
                return Forbid();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var reports = await _context.Reports
                .Where(r => r.ManagerId == userId)
                .OrderByDescending(r => r.CreationDate)
                .ToListAsync();

            return View(reports);
        }
        [HttpGet("Reports/Create")]
        public IActionResult CreateReport()
        {
            if (!IsManager())
            {
                return Forbid();
            }

            return View();
        }

        [HttpPost("Reports/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReport(Report model)
        {
            if (!IsManager())
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var salesSummary = await _context.OrderDetails
                    .Include(od => od.Book)
                    .Where(od => od.Order.Status == OrderStatus.Wysłane)
                    .GroupBy(od => od.Book.Name)
                    .Select(g => new
                    {
                        BookName = g.Key,
                        TotalQuantity = g.Sum(od => od.Quantity),
                        TotalRevenue = g.Sum(od => od.Quantity * od.Book.Price)
                    })
                    .OrderByDescending(x => x.TotalQuantity)
                    .ToListAsync();

                var reportText = "Raport sprzedaży do " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "\n\n";
                reportText += "Książka;Ilość sprzedana;Przychód\n";
                foreach (var item in salesSummary)
                {
                    reportText += $"{item.BookName};{item.TotalQuantity};{item.TotalRevenue:C}\n";
                }

                model.Data = reportText;
                model.CreationDate = DateTime.Now;
                model.ManagerId = userId;

                _context.Reports.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Raport został zapisany.";
                return RedirectToAction("Reports");
            }

            return View(model);
        }
        [HttpGet("Books/Add")]
        public IActionResult AddBook()
        {
            if (!IsManager())
            {
                return Forbid();
            }


            return View();
        }

        [HttpPost("Books/Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBook(Book book, int[]? Discounts)
        {
            if (!IsManager())
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {


                var success = await _bookService.AddBookAsync(book);
                if (success)
                {
                    TempData["Success"] = "Książka została dodana.";
                    return RedirectToAction("ManageBooks", "Manager");
                }
            }

            return View(book);
        }


        [HttpGet("Books")]
        public async Task<IActionResult> ManageBooks()
        {
            if (!IsManager())
            {
                return Forbid();
            }

            var books = await _bookService.GetAllBooksAsync();
            return View(books);
        }



    }
}