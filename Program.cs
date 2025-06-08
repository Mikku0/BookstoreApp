using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using BookstoreApp.Data;
using BookstoreApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Dodaj services do kontenera
builder.Services.AddControllersWithViews();

// Konfiguracja bazy danych - możesz wybrać między SQLite i SQL Server
builder.Services.AddDbContext<BookstoreContext>(options =>
    options.UseSqlite("Data Source=bookstore.db")); // SQLite - prostsze dla developmentu
                                                    // lub
                                                    // options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // SQL Server

// Dodaj sesje
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Konfiguracja autoryzacji opartej na cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Users/Login";
        options.LogoutPath = "/Users/Logout";
        options.AccessDeniedPath = "/Users/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
    });

// Dodaj polityki autoryzacji
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ClientOnly", policy =>
        policy.RequireClaim("UserType", "Client"));

    options.AddPolicy("EmployeeOnly", policy =>
        policy.RequireClaim("UserType", "Employee", "Manager"));

    options.AddPolicy("ManagerOnly", policy =>
        policy.RequireClaim("UserType", "Manager"));

    options.AddPolicy("StaffOnly", policy =>
        policy.RequireClaim("UserType", "Employee", "Manager"));
});

// Rejestracja serwisów
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Konfiguracja HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Ważna kolejność middleware:
app.UseSession();          // Sesje przed uwierzytelnianiem
app.UseAuthentication();   // Uwierzytelnianie przed autoryzacją
app.UseAuthorization();

// Konfiguracja tras - specjalne trasy dla różnych ról
app.MapControllerRoute(
    name: "client",
    pattern: "Client/{action=Dashboard}/{id?}",
    defaults: new { controller = "Client" });

app.MapControllerRoute(
    name: "employee",
    pattern: "Employee/{action=Dashboard}/{id?}",
    defaults: new { controller = "Employee" });

app.MapControllerRoute(
    name: "manager",
    pattern: "Manager/{action=Dashboard}/{id?}",
    defaults: new { controller = "Manager" });

app.MapControllerRoute(
    name: "orderTracking",
    pattern: "track/{orderId:int}",
    defaults: new { controller = "OrderTracking", action = "TrackOrder" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();