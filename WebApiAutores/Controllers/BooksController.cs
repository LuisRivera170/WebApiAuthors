using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entities;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController: ControllerBase
    {
        private readonly ApplicationDbContext context;

        public BooksController(ApplicationDbContext context)
        {
            this.context = context;
        }

        /*[HttpGet("{id:int}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await context.Books.Include(book => book.Author).FirstOrDefaultAsync(book => book.Id == id);
            if (book == null)
            {
                return NotFound($"Author with Id {id} not found");
            }

            return book;
        }*/

        [HttpPost]
        public async Task<ActionResult> PostBook(Book book)
        {
            /*var existAuthor = await context.Authors.AnyAsync(author => author.Id == book.AuthorId);
            if (!existAuthor)
            {
                return BadRequest($"Author with Id {book.AuthorId} not exist");
            }*/
            context.Add(book);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
