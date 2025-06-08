using Microsoft.AspNetCore.Mvc;
using BookstoreApp.Services;

namespace BookstoreApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;

        public HomeController(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllBooksAsync();
            return View(books);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}