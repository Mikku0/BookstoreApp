using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApp.Models;

public class Delivery
{
    public int Id { get; set; }
    public int OrderId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public void InitializePackages()
    {
        // Initialize delivery packages
    }

    public void MakeDelivery()
    {
        // Execute delivery
    }

    public void OrderDelivery()
    {
        // Order delivery service
    }
}