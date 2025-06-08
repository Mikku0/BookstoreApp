using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApp.Models;

public class Discount
{
    public int Id { get; set; }
    public decimal Percentage { get; set; }
    public string Description { get; set; } = "";

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public void ApplyDiscount()
    {
        // Apply discount logic
    }
}
