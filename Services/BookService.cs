using BookstoreApp.Data;
using BookstoreApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BookstoreApp.Services
{
    public class BookService : IBookService
    {
        private readonly BookstoreContext _context;

        public BookService(BookstoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<bool> AddBookAsync(Book book)
        {
            _context.Books.Add(book);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            _context.Books.Update(book);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            _context.Books.Remove(book);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}