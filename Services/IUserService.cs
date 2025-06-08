namespace BookstoreApp.Services
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(string firstName, string lastName, string login, string password, string address);
        Task<Models.User?> LoginAsync(string login, string password);
        Task<Models.User?> GetUserByIdAsync(int id);
        Task<Models.User?> GetUserByLoginAsync(string login);
    }
}