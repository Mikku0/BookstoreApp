using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApp.Models;

public class Manager : RegisteredUser
{
    public decimal Salary { get; set; }
    public DateTime DateOfEmployment { get; set; }

    public void CreateReport()
    {
        // Create reports
    }

    public void ViewReports()
    {
        // View existing reports
    }

    public void AddDiscount()
    {
        // Add discount functionality
    }
}
