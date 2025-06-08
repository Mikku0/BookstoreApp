using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApp.Models;

public class Cart
{
    public int Id { get; set; }
    public decimal Value { get; set; }

    public int UserId { get; set; }
    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();

    public void AddBook(Book book, int quantity = 1)
    {
        var existingItem = Items.FirstOrDefault(i => i.BookId == book.Id);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            Items.Add(new CartItem
            {
                BookId = book.Id,
                Book = book,
                Quantity = quantity,
                CartId = this.Id
            });
        }
        Value = CalculateValue();
    }

    public void RemoveBook(int bookId, int quantity = 1)
    {
        var item = Items.FirstOrDefault(i => i.BookId == bookId);
        if (item != null)
        {
            item.Quantity -= quantity;
            if (item.Quantity <= 0)
            {
                Items.Remove(item);
            }
        }
        Value = CalculateValue();
    }

    public decimal CalculateValue()
    {
        return Items.Sum(item => item.Book.Price * item.Quantity);
    }
}