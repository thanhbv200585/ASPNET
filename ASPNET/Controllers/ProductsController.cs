using ASPNET.Filter;
using ASPNET.Models;
using ASPNET.Repository;
using ASPNET.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IBookRepository repo) : ControllerBase
    {
        private readonly IBookRepository _bookRepo = repo;

        [HttpGet]
        [MyLogging("AllProducts")]
        public async Task<IActionResult> GetAllBooks()
        {
            try
            {
                return Ok(await _bookRepo.GetAllBooksAsync());
            }
            catch 
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _bookRepo.GetBookAsync(id);
            return book == null ? NotFound() : Ok(book);
        }

        [HttpPost]
        [Route("", Name = "AddNewBook")]
        public async Task<IActionResult> AddNewBook(BookModel model)
        {
            try
            {
                var newBookId = await _bookRepo.AddBookAsync(model);
                return CreatedAtAction("GetBookById", "Products", new { id = newBookId }, model);
                //return CreatedAtAction(nameof(GetBookById), "Products", "AddNewBook", new { controller = "Product", id = newBookId });
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, BookModel model)
        {
            await _bookRepo.UpdateBookAsync(id, model);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await _bookRepo.DeleteBookAsync(id);
            return Ok();
        }

        [HttpPost]
        [Route("sort")]
        public async Task<IActionResult> SortingBook([FromBody] SortingRequest req)
        {
            try
            {
                var books = await _bookRepo.SortingBookAsync(req);
                return Ok(books);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
