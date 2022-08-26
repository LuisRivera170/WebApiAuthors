using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entities;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public BooksController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookDTO>> GetBook(int id)
        {
            var book = await context.Books.FirstOrDefaultAsync(book => book.Id == id);
            if (book == null)
            {
                return NotFound($"Book with Id {id} not found");
            }

            return mapper.Map<BookDTO>(book);
        }

        [HttpPost]
        public async Task<ActionResult> PostBook(CreateBookDTO bookDTO)
        {
            /*var existAuthor = await context.Authors.AnyAsync(author => author.Id == book.AuthorId);
            if (!existAuthor)
            {
                return BadRequest($"Author with Id {book.AuthorId} not exist");
            }*/
            var book = mapper.Map<Book>(bookDTO);

            context.Add(book);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
