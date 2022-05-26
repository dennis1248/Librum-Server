using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repository;

public class BookRepository : IBookRepository
{
    private readonly DataContext _context;


    public BookRepository(DataContext context)
    {
        _context = context;
    }


    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task LoadRelationShipsAsync(Book book)
    {
        await _context.Entry(book).Collection(p => p.Authors).LoadAsync();
        await _context.Entry(book).Collection(p => p.Tags).LoadAsync();
    }

    public async Task LoadRelationShipsAsync(IEnumerable<Book> books)
    {
        foreach (var book in books)
        {
            await LoadRelationShipsAsync(book);
        }
    }

    public IQueryable<Book> GetBooks(string userId)
    {
        return _context.Books.Where(book => book.UserId == userId);
    }
    
    public async Task<Book> GetBook(string userId, string bookTitle, bool trackChanges)
    {
        return trackChanges
            ? await _context.Books
                .SingleOrDefaultAsync(book => book.UserId == userId && book.Title == bookTitle)
            : await _context.Books
                .AsNoTracking()
                .SingleOrDefaultAsync(book => book.UserId == userId && book.Title == bookTitle);
    }

    public void DeleteBook(Book book)
    {
        _context.Remove(book);
    }
}