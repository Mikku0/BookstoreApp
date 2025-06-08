using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookstoreApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookstoreApp.Controllers
{
    [Route("OrderTracking")]
    public class OrderTrackingController : Controller
    {
        private readonly BookstoreContext _context;

        public OrderTrackingController(BookstoreContext context)
        {
            _context = context;
        }

        [HttpGet("Track/{orderId}")]
        public async Task<IActionResult> TrackOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .Include(o => o.Deliveries)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                if (order.ClientId != userId && User.FindFirst("UserType")?.Value == "Client")
                {
                    return Forbid();
                }
            }

            return View(order);
        }

        [HttpGet("Status/{orderId}")]
        public async Task<IActionResult> GetOrderStatus(int orderId)
        {
            var order = await _context.Orders
                .Select(o => new { o.Id, o.Status, o.ClientId })
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var userType = User.FindFirst("UserType")?.Value;

                if (userType == "Client" && order.ClientId != userId)
                {
                    return Forbid();
                }
            }

            return Json(new { orderId = order.Id, status = order.Status.ToString() });
        }
    }
}