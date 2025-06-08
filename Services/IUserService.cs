namespace BookstoreApp.Services
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(string firstName, string lastName, string login, string password, string address);
        Task<bool> RegisterEmployeeAsync(string firstName, string lastName, string login, string password);
        Task<bool> RegisterManagerAsync(string firstName, string lastName, string login, string password);
        Task<Models.User?> LoginAsync(string login, string password);
        Task<Models.User?> GetUserByIdAsync(int id);
        Task<Models.User?> GetUserByLoginAsync(string login);
    }
}