using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApp.Models;

public class Employee : RegisteredUser
{
    public decimal Salary { get; set; }
    public DateTime DateOfEmployment { get; set; }
    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();


    public void UpdateStock() { }

    public void TakeDelivery(Delivery delivery)
    {
        if (!Deliveries.Contains(delivery))
        {
            Deliveries.Add(delivery);
        }
    }

    public void SendOrder() { }
}
