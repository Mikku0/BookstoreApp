using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApp.Models;

public class Warehouse
{
    public int Id { get; set; }
    public List<Book> Books { get; set; } = new List<Book>();

    public void UpdateStock()
    {
        // Update warehouse stock
    }

    public void AddBook()
    {
        // Add book to warehouse
    }

    public void RemoveBook()
    {
        // Remove book from warehouse
    }

    public void GetBooks()
    {
        // Retrieve books from warehouse
    }

    public void GetBookById(int id)
    {
        // Get specific book by ID
    }

    public void DisplayBooks()
    {
        // Display all books
    }
}