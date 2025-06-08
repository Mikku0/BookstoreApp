using Microsoft.AspNetCore.Mvc;
using BookstoreApp.Services;
using BookstoreApp.Models;
using Microsoft.EntityFrameworkCore;
using BookstoreApp.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookstoreApp.Controllers
{
    [Authorize] // Cały kontroler wymaga logowania
    public class CartController : Controller
    {
        private readonly BookstoreContext _context;
        private readonly IBookService _bookService;

        public CartController(BookstoreContext context, IBookService bookService)
        {
            _context = context;
            _bookService = bookService;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var cart = await GetOrCreateUserCartAsync(userId);
            return View(cart);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int bookId, int quantity = 1)
        {
            var book = await _bookService.GetBookByIdAsync(bookId);
            if (book == null)
            {
                return NotFound("Książka nie została znaleziona.");
            }

            var userId = GetCurrentUserId();
            var cart = await GetOrCreateUserCartAsync(userId);

            // Sprawdź czy książka już jest w koszyku
            var existingItem = cart.Items.FirstOrDefault(i => i.BookId == bookId);
            
            if (existingItem != null)
            {
                // Zwiększ ilość jeśli książka już jest w koszyku
                existingItem.Quantity += quantity;
                _context.CartItems.Update(existingItem);
            }
            else
            {
                // Dodaj nową pozycję do koszyka
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    BookId = bookId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            // Zaktualizuj wartość koszyka
            await UpdateCartValueAsync(cart.Id);

            TempData["Success"] = $"Dodano '{book.Name}' do koszyka!";
            return RedirectToAction("Index", "Books");
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            if (quantity <= 0)
            {
                return await RemoveFromCart(cartItemId);
            }

            var userId = GetCurrentUserId();
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

            if (cartItem == null)
            {
                return NotFound();
            }

            cartItem.Quantity = quantity;
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();

            // Zaktualizuj wartość koszyka
            await UpdateCartValueAsync(cartItem.CartId);

            return RedirectToAction("Index");
        }

        // POST: Cart/RemoveFromCart
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = GetCurrentUserId();
            var cartItem = await _context.CartItems
                .Include(ci => ci.Book)
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

            if (cartItem == null)
            {
                return NotFound();
            }

            var cartId = cartItem.CartId;
            var bookName = cartItem.Book.Name;
            
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            // Zaktualizuj wartość koszyka
            await UpdateCartValueAsync(cartId);

            TempData["Success"] = $"Usunięto '{bookName}' z koszyka.";
            return RedirectToAction("Index");
        }

        // POST: Cart/ClearCart
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetCurrentUserId();
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null && cart.Items.Any())
            {
                _context.CartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();

                // Zaktualizuj wartość koszyka
                await UpdateCartValueAsync(cart.Id);

                TempData["Success"] = "Koszyk został wyczyszczony.";
            }

            return RedirectToAction("Index");
        }

        // GET: Cart/Checkout
        public async Task<IActionResult> Checkout()
        {
            var userId = GetCurrentUserId();
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Koszyk jest pusty.";
                return RedirectToAction("Index");
            }

            return View(cart);
        }

        // POST: Cart/ProcessCheckout
        [HttpPost]
        public async Task<IActionResult> ProcessCheckout()
        {
            var userId = GetCurrentUserId();
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Koszyk jest pusty.";
                return RedirectToAction("Index");
            }

            // Utwórz zamówienie (użytkownik jest już zalogowany)
            var order = new Order
            {
                ClientId = userId,
                Status = OrderStatus.Oczekujące,
                TotalPrice = cart.CalculateValue()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Dodaj szczegóły zamówienia
            foreach (var item in cart.Items)
            {
                var orderDetail = new OrderDetails
                {
                    OrderId = order.Id,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    Date = DateTime.Now
                };
                _context.OrderDetails.Add(orderDetail);
            }

            await _context.SaveChangesAsync();

            // Wyczyść koszyk po złożeniu zamówienia
            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();
            await UpdateCartValueAsync(cart.Id);

            TempData["Success"] = $"Zamówienie #{order.Id} zostało złożone pomyślnie!";
            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }

        // GET: Cart/OrderConfirmation
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var userId = GetCurrentUserId();
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .Include(o => o.Client)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.ClientId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Cart/GetCartItemCount - dla AJAX
        [HttpGet]
        public async Task<IActionResult> GetCartItemCount()
        {
            var userId = GetCurrentUserId();
            var itemCount = await _context.CartItems
                .Include(ci => ci.Cart)
                .Where(ci => ci.Cart.UserId == userId)
                .SumAsync(ci => ci.Quantity);

            return Json(itemCount);
        }

        #region Private Methods

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("Użytkownik nie jest zalogowany.");
            }
            return int.Parse(userIdClaim);
        }

        private async Task<Cart> GetOrCreateUserCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                // Utwórz nowy koszyk dla użytkownika
                cart = new Cart 
                { 
                    UserId = userId,
                    Value = 0 
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
                
                // Reload cart with navigation properties
                cart = await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(ci => ci.Book)
                    .FirstAsync(c => c.Id == cart.Id);
            }

            return cart;
        }

        private async Task UpdateCartValueAsync(int cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart != null)
            {
                cart.Value = cart.CalculateValue();
                _context.Carts.Update(cart);
                await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}