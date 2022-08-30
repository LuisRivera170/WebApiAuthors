using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
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

        [HttpGet("{bookId:int}", Name = "GetBook")]
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

        [HttpPost(Name = "CreateBook")]
        public async Task<ActionResult> PostBook(CreateBookDTO createBookDTO)
        {
            if (createBookDTO.AuthorIds == null)
            {
                return BadRequest("A book cannot be created without authors");
            }

            var authorIdsDB = await context.Authors
                .Where(authorDB => createBookDTO.AuthorIds.Contains(authorDB.Id))
                .Select(author => author.Id)
                .ToListAsync();

            if (createBookDTO.AuthorIds.Count != authorIdsDB.Count)
            {
                return BadRequest("Wrong authors");
            }

            var book = mapper.Map<Book>(createBookDTO);

            asignOrder(book);

            context.Add(book);
            await context.SaveChangesAsync();

            var bookDTO = mapper.Map<BookDTO>(book);

            return CreatedAtRoute("GetBook", new { bookId = book.Id }, bookDTO);
        }

        [HttpPut("{bookId:int}", Name = "UpdateBook")]
        public async Task<ActionResult> UpdateBook(int bookId, CreateBookDTO updateBookDTO)
        {
            var bookDB = await context.Books
                .Include(book => book.AuthorsBooks)
                .FirstOrDefaultAsync(book => book.Id == bookId);

            if (bookDB == null)
            {
                return NotFound($"Book with Id {bookId} not found");
            }

            bookDB = mapper.Map(updateBookDTO, bookDB);
            asignOrder(bookDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{bookId:int}", Name = "PartialyUpdateBook")]
        public async Task<ActionResult> PatchBook(int bookId, JsonPatchDocument<PatchBookDTO> patchBookDTO)
        {
            if (patchBookDTO == null)
            {
                return BadRequest();
            }

            var bookDB = await context.Books.FirstOrDefaultAsync(book => book.Id == bookId);
            if (bookDB == null)
            {
                return NotFound();
            }

            var bookDTO = mapper.Map<PatchBookDTO>(bookDB);
            patchBookDTO.ApplyTo(bookDTO, ModelState);

            var isValid = TryValidateModel(bookDTO);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(bookDTO, bookDB);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{bookId:int}", Name = "DeleteBook")]
        public async Task<ActionResult> DeleteBook(int bookId)
        {
            var existBook = await context.Books.AnyAsync(book => book.Id == bookId);
            if (!existBook)
            {
                return NotFound("Book not found");
            }
            context.Remove(new Book() { Id = bookId });
            await context.SaveChangesAsync();
            return NoContent();
        }

        private void asignOrder(Book book) 
        {
            if (book.AuthorsBooks != null)
            {
                for (int i = 0; i < book.AuthorsBooks.Count; i++)
                {
                    book.AuthorsBooks[i].Order = i;
                }
            }
        }
    }
}
