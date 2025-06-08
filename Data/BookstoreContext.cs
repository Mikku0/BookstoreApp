using Microsoft.EntityFrameworkCore;
using BookstoreApp.Models;

namespace BookstoreApp.Data
{
    public class BookstoreContext : DbContext
    {
        public BookstoreContext(DbContextOptions<BookstoreContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure inheritance
            modelBuilder.Entity<User>().UseTphMappingStrategy();
            
            // Configure relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Client)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.ClientId);

            modelBuilder.Entity<OrderDetails>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetails>()
                .HasOne(od => od.Book)
                .WithMany(b => b.OrderDetails)
                .HasForeignKey(od => od.BookId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Book)
                .WithMany(b => b.CartItems)
                .HasForeignKey(ci => ci.BookId);

            // NOWA RELACJA: Cart -> RegisteredUser (1:1)
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Name = "Pan Tadeusz", Author = "Adam Mickiewicz", Price = 29.99m, Genre = "Klasyka" },
                new Book { Id = 2, Name = "Wiedźmin", Author = "Andrzej Sapkowski", Price = 34.99m, Genre = "Fantasy" },
                new Book { Id = 3, Name = "Solaris", Author = "Stanisław Lem", Price = 27.50m, Genre = "Science Fiction" }
            );
        }
    }
}