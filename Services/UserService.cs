using BookstoreApp.Data;
using BookstoreApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookstoreApp.Services
{
    public class UserService : IUserService
    {
        private readonly BookstoreContext _context;

        public UserService(BookstoreContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterUserAsync(string firstName, string lastName, string login, string password, string address)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserLogin == login);
            if (existingUser != null) return false;

            var client = new Client
            {
                FirstName = firstName,
                LastName = lastName,
                UserLogin = login,
                Password = password,
                Address = address
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RegisterEmployeeAsync(string firstName, string lastName, string login, string password)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserLogin == login);
            if (existingUser != null) return false;

            var employee = new Employee
            {
                FirstName = firstName,
                LastName = lastName,
                UserLogin = login,
                Password = password
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RegisterManagerAsync(string firstName, string lastName, string login, string password)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserLogin == login);
            if (existingUser != null) return false;

            var manager = new Manager
            {
                FirstName = firstName,
                LastName = lastName,
                UserLogin = login,
                Password = password
            };

            _context.Managers.Add(manager);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Models.User?> LoginAsync(string login, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserLogin == login && u.Password == password);
        }

        public async Task<Models.User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<Models.User?> GetUserByLoginAsync(string login)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserLogin == login);
        }
    }
}