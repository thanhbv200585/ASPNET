using ASPNET.Data;
using ASPNET.Models;
using ASPNET.Requests;

namespace ASPNET.Repository
{
    public interface IBookRepository
    {
        public Task<List<BookModel>> GetAllBooksAsync();
        public Task<BookModel> GetBookAsync(int id);
        public Task<int> AddBookAsync(BookModel model);
        public Task<int> UpdateBookAsync(int id, BookModel model);
        public Task<int> DeleteBookAsync(int id);
        public Task<List<BookModel>> SortingBookAsync(SortingRequest req);
    }
}
