using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entities;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/books/{bookId:int}/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public CommentsController(
            ApplicationDbContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager
        )
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet(Name = "GetComments")]
        public async Task<ActionResult<List<CommentDTO>>> GetComments(int bookId)
        {
            var comments = await context.Comments.Where(comment => comment.BookId == bookId).ToListAsync();

            return mapper.Map<List<CommentDTO>>(comments);
        }

        [HttpGet("{commentId:int}", Name = "GetCommentById")]
        public async Task<ActionResult<CommentDTO>> GetCommentById(int commentId)
        {
            var comment = await context.Comments.FirstOrDefaultAsync(comment => comment.Id == commentId);

            if (comment == null)
            {
                return NotFound($"Comment with id \"{commentId}\" not found");
            }

            return mapper.Map<CommentDTO>(comment);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost(Name = "CreateComment")]
        public async Task<ActionResult> PostComment(int bookId, CreateCommentDTO createCommentDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var user = await userManager.FindByEmailAsync(email);
            var userId = user.Id;

            var book = await context.Books.AnyAsync(book => book.Id == bookId);
            if (!book)
            {
                return BadRequest($"Book with Id {bookId} not found");
            }

            var comment = mapper.Map<Comment>(createCommentDTO);
            comment.BookId = bookId;
            comment.UserId = userId;
            context.Add(comment);
            await context.SaveChangesAsync();

            var commentDTO = mapper.Map<CommentDTO>(comment);

            return CreatedAtRoute("GetCommentById", new { commentId = comment.Id, bookId }, commentDTO);
        }

        [HttpPut("{commentId:int}", Name = "UpdateComment")]
        public async Task<ActionResult> PutComment(int bookId, int commentId, CreateCommentDTO updateCommentDTO)
        {
            var existBook = await context.Books.AnyAsync(book => book.Id == bookId);
            if (!existBook)
            {
                return BadRequest($"Book with Id {bookId} not found");
            }

            var existComment = await context.Comments.AnyAsync(comment => comment.Id == commentId);
            if (!existComment)
            {
                return BadRequest($"Comment with id {commentId} not found");
            }

            var comment = mapper.Map<Comment>(updateCommentDTO);
            comment.Id = commentId;
            context.Update(comment);
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
