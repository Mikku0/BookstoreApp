using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApp.Models;

public class Employee : RegisteredUser
{
    public decimal Salary { get; set; }
    public DateTime DateOfEmployment { get; set; }

    public void UpdateStock()
    {
        // Update book stock
    }

    public void TakeDelivery()
    {
        // Handle deliveries
    }

    public void SendOrder()
    {
        // Send orders
    }
}
