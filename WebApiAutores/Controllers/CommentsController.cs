using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entities;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/books/{bookId:int}/comments")]
    public class CommentsController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CommentsController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<CommentDTO>>> Get(int bookId)
        {
            var comments = await context.Comments.Where(comment => comment.BookId == bookId).ToListAsync();
            
            return mapper.Map<List<CommentDTO>>(comments);
        }

        [HttpPost]
        public async Task<ActionResult> Post(int bookId, CreateCommentDTO commentDTO)
        {
            var book = await context.Books.AnyAsync(book => book.Id == bookId);
            if (!book)
            {
                return BadRequest($"Book with Id {bookId} not found");
            }

            var comment = mapper.Map<Comment>(commentDTO);
            comment.BookId = bookId;
            context.Add(comment);
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}
