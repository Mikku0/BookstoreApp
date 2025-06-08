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

    public int EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = null!;

    public void InitializePackages()
    {
    }

    public void MakeDelivery()
    {

    }

    public void OrderDelivery()
    {
    }
}