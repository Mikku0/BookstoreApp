using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BookstoreApp.Models;

public class Client : RegisteredUser
{
    public string Address { get; set; } = "";
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual Cart? Cart { get; set; }

    public void AddBookToCart(Book book, int quantity = 1)
    {
        if (Cart == null) return;

        var existingItem = Cart.Items.FirstOrDefault(i => i.BookId == book.Id);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            Cart.Items.Add(new CartItem
            {
                BookId = book.Id,
                Book = book,
                Quantity = quantity,
                CartId = Cart.Id
            });
        }
        Cart.Value = Cart.CalculateValue();
    }

    public void RemoveBookFromCart(int bookId, int quantity = 1)
    {
        if (Cart == null) return;

        var item = Cart.Items.FirstOrDefault(i => i.BookId == bookId);
        if (item != null)
        {
            item.Quantity -= quantity;
            if (item.Quantity <= 0)
            {
                Cart.Items.Remove(item);
            }
        }
        Cart.Value = Cart.CalculateValue();
    }

    public Order PlaceOrder()
    {
        if (Cart == null || !Cart.Items.Any()) 
            throw new InvalidOperationException("Koszyk jest pusty");

        var order = new Order
        {
            ClientId = this.Id,
            Client = this,
            Status = OrderStatus.Oczekujące,
            OrderDetails = Cart.Items.Select(item => new OrderDetails
            {
                BookId = item.BookId,
                Book = item.Book,
                Quantity = item.Quantity,
            }).ToList()
        };

        Cart.Items.Clear();
        Cart.Value = 0;

        return order;
    }

    public void PrintAllOrders()
    {
        Console.WriteLine($"Zamówienia klienta: {FirstName}");
        Console.WriteLine("=" + new string('=', 40));
        
        foreach (var order in Orders.OrderByDescending(o => o.Id))
        {
            Console.WriteLine($"Zamówienie #{order.Id} - Status: {order.Status}");
            Console.WriteLine($"Data: {order.Id} (ID jako data przykładowa)");
            
            foreach (var detail in order.OrderDetails)
            {
                Console.WriteLine($"  - {detail.Book.Name} x{detail.Quantity}");
            }
        }
    }
}