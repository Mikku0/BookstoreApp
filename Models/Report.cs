using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApp.Models;

public class Report
{
    public int Id { get; set; }
    public string Data { get; set; } = "";
    public DateTime CreationDate { get; set; }
    public string Notes { get; set; } = "";
}