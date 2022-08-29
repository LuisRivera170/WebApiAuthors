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

        [HttpGet("{bookId:int}")]
        public async Task<ActionResult<BookDTOWithAuthors>> GetBook(int bookId)
        {
            var book = await context.Books
                .Include(libroDB => libroDB.AuthorsBooks)
                .ThenInclude(authorBook => authorBook.Author)
                .Include(libroDB => libroDB.Comments)
                .FirstOrDefaultAsync(book => book.Id == bookId);

            if (book == null)
            {
                return NotFound($"Book with Id {bookId} not found");
            }

            book.AuthorsBooks = book.AuthorsBooks.OrderBy(authorBook => authorBook.Order).ToList();

            return mapper.Map<BookDTOWithAuthors>(book);
        }

        [HttpPost]
        public async Task<ActionResult> PostBook(CreateBookDTO bookDTO)
        {
            if (bookDTO.AuthorIds == null)
            {
                return BadRequest("A book cannot be created without authors");
            }

            var authorIdsDB = await context.Authors
                .Where(authorDB => bookDTO.AuthorIds.Contains(authorDB.Id))
                .Select(author => author.Id)
                .ToListAsync();

            if (bookDTO.AuthorIds.Count != authorIdsDB.Count)
            {
                return BadRequest("Wrong authors");
            }

            var book = mapper.Map<Book>(bookDTO);

            if (book.AuthorsBooks != null)
            {
                for (int i = 0; i < book.AuthorsBooks.Count; i++)
                {
                    book.AuthorsBooks[i].Order = i;
                }
            }

            context.Add(book);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
