namespace BookstoreApp.Services
{
    public interface IBookService
    {
        Task<IEnumerable<Models.Book>> GetAllBooksAsync();
        Task<Models.Book?> GetBookByIdAsync(int id);
        Task<bool> AddBookAsync(Models.Book book);
        Task<bool> UpdateBookAsync(Models.Book book);
        Task<bool> DeleteBookAsync(int id);
    }
}