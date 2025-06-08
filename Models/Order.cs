using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApp.Models;

public class Order
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalPrice { get; set; }

    public virtual Client Client { get; set; } = null!;
    public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    public string PrintOrderDetails()
    {
        return $"{Client.FirstName}, {Client.LastName}, {Status}, {TotalPrice}";
    }

    public void CancelOrder()
    {
        Status = OrderStatus.Anulowane;
    }

    public void PayForOrder()
    {
        Status = OrderStatus.Opłacone;
    }
}
