using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace ASPNET.Data
{
    public class BookStoreContext : IdentityDbContext<ApplicationUser>
    {
        public BookStoreContext(DbContextOptions<BookStoreContext> options) : base(options) { }

        #region DbSet
        public DbSet<Book> Books { get; set;}
        #endregion
    }
}
