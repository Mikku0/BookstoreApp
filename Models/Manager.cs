using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApp.Models;

public class Manager : RegisteredUser
{
    public decimal Salary { get; set; }
    public DateTime DateOfEmployment { get; set; }
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();


    public void CreateReport() { }

    public void ViewReports() { }

    public void AddDiscount() { }
}
