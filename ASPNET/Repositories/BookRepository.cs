using ASPNET.Data;
using ASPNET.Models;
using ASPNET.Repository;
using ASPNET.Requests;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using System.Text;

namespace ASPNET.Repositories
{
    public class BookRepository : IBookRepository
    {
        public readonly static int UPDATE_NOT_FOUND = -1;
        public readonly static int DELETE_NOT_FOUND = -2;
        private readonly BookStoreContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        public BookRepository(BookStoreContext context, IMapper mapper, IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            _context = context;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }
        public async Task<int> AddBookAsync(BookModel model)
        {
            var newBook = _mapper.Map<Book>(model);
            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();
            return newBook.Id;
        }

        public async Task<int> DeleteBookAsync(int id)
        {
            var deleteBook = _context.Books.SingleOrDefault(b => b.Id == id);
            if (deleteBook != null)
            {
                _context.Books.Remove(deleteBook);
                await _context.SaveChangesAsync();
                return deleteBook.Id;
            }
            return DELETE_NOT_FOUND;
        }

        public async Task<List<BookModel>> GetAllBooksAsync()
        {
            string cacheKey = "bookList";
            if (!_memoryCache.TryGetValue(cacheKey, out List<BookModel> bookList))
            {
                bookList = _mapper.Map<List<BookModel>>(await _context.Books.ToListAsync());
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };
                _memoryCache.Set(cacheKey, bookList, cacheExpiryOptions);
            }
            return bookList;
        }

        public async Task<BookModel> GetBookAsync(int id)
        {
            string cacheKey = "book" + id;
            var redisBook = await _distributedCache.GetAsync(cacheKey);
            string serializedBook;
            var book = new Book();
            if (redisBook != null)
            {
                serializedBook = Encoding.UTF8.GetString(redisBook);
                book = JsonConvert.DeserializeObject<Book>(serializedBook);
            }
            else
            {
                book = await _context.Books!.FindAsync(id);
                serializedBook = JsonConvert.SerializeObject(book);
                redisBook = Encoding.UTF8.GetBytes(serializedBook);
                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2))
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(5));
                await _distributedCache.SetAsync(cacheKey, redisBook, options);
            }
            return _mapper.Map<BookModel>(book);
        }

        public async Task<int> UpdateBookAsync(int id, BookModel model)
        {
            if (id == model.Id)
            {
                var updateBook = _mapper.Map<Book>(model);
                _context.Books!.Update(updateBook);
                await _context.SaveChangesAsync();
                return updateBook.Id;
            }
            return UPDATE_NOT_FOUND;
        }

        public async Task<List<BookModel>> SortingBookAsync(SortingRequest req)
        {
            IQueryable<Book> query = this._context.Books.AsQueryable();
            List<Book> response;
            if (req.OrderType == "desc")
            {
                response = await query.OrderBy(req.OrderBy + " desc")
                    .Skip((req.Page - 1) * req.Take)
                    .Take(req.Take).ToListAsync();
            }
            else if (req.OrderType == "asc" || req.OrderType == null || req.OrderType == "") 
            {
                response = await query.OrderBy(req.OrderBy + " asc")
                    .Skip((req.Page - 1) * req.Take)
                    .Take(req.Take).ToListAsync();
            }
            else
            {
                throw new Exception("Order Type must be 'asc' or 'desc'");
            }
            return _mapper.Map<List<BookModel>>(response);
        }
    }
}
