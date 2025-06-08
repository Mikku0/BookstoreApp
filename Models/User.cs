using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApp.Models;

public abstract class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string UserLogin { get; set; } = "";
    public string Password { get; set; } = "";

    public abstract void BrowseOffers();
    public abstract void Register();
}
