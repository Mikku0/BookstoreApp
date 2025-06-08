using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApp.Models;

public class RegisteredUser : User
    {
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual Cart? Cart { get; set; } // Dodajemy koszyk do użytkownika

        public override void BrowseOffers()
        {
            // Implementation for registered user browsing
        }

        public override void Register()
        {
            // Implementation for user registration
        }

        public virtual void Login()
        {
            // Login implementation
        }
    }