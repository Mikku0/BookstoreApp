using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApp.Models;

public abstract class RegisteredUser : User
    {
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual Cart? Cart { get; set; }

        public override void BrowseOffers() { }

        public override void Register() { }

        public virtual void Login() { }
    }