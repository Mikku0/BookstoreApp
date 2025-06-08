using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApp.Models;

public class CartItem
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public int BookId { get; set; }
    public int Quantity { get; set; }

    public virtual Cart Cart { get; set; } = null!;
    public virtual Book Book { get; set; } = null!;
}