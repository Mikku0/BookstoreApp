
namespace BookstoreApp.Models;
public class Book
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Author { get; set; } = "";
    public decimal Price { get; set; }
    public string Genre { get; set; } = "";
    
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();

}