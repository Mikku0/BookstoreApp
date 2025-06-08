using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApp.Models;

public class OrderDetails
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int BookId { get; set; }
    public int Quantity { get; set; }
    public DateTime Date { get; set; }

    public virtual Order Order { get; set; } = null!;
    public virtual Book Book { get; set; } = null!;

    public decimal CalculateTotal()
    {
        return Quantity * Book.Price;
    }
}
