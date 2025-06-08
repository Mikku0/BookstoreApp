using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApp.Models;

public class Cart
{
    public int Id { get; set; }
    public int UserId { get; set; } // Dodajemy powiązanie z użytkownikiem
    public decimal Value { get; set; }

    public virtual RegisteredUser User { get; set; } = null!; // Navigation property
    public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();

    public void AddBook()
    {
        // Add book to cart
    }

    public void RemoveBook()
    {
        // Remove book from cart
    }

    public decimal CalculateValue()
    {
        return Items.Sum(item => item.Book.Price * item.Quantity);
    }
}
