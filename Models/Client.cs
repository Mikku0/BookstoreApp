using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApp.Models;

public class Client : RegisteredUser
{
    public string Address { get; set; } = "";

    public void AddBookToCart()
    {
        // Add book to cart
    }

    public void RemoveBookFromCart()
    {
        // Remove book from cart
    }

    public void PlaceOrder()
    {
        // Place an order
    }

    public void PrintAllOrders()
    {
        // Print order history
    }
}
