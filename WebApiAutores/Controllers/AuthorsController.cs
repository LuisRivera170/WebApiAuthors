using Microsoft.AspNetCore.Mvc;
using WebApiAutores.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebApiAutores.Controllers
{

    [ApiController]
    [Route("api/authors")]
    public class AuthorsController: ControllerBase
    {
        private readonly ApplicationDbContext context;

        public AuthorsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Author>>> GetAuthors() {
            return await context.Authors.Include(author => author.Books).ToListAsync();
        }

        [HttpGet("{name=Pedro}")]
        public async Task<IActionResult> GetAuthor([FromRoute] string name, [FromQuery] int id) 
        {
            var author = await context.Authors.FirstOrDefaultAsync(author => author.Name.Contains(name));
            if (author == null)
            {
                return NotFound($"Author with name \"{name}\" not found");
            }
            return Ok(author);
        }

        [HttpPost]
        public async Task<ActionResult> PostAuthor([FromBody] Author author)
        {
            context.Add(author);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutAuthor(Author author, int id)
        {
            if (author.Id != id)
            {
                return BadRequest("Author id don't match");
            }
            var existAuthor = await context.Authors.AnyAsync(author => author.Id == id);
            if (!existAuthor)
            {
                return NotFound("Author not found");
            }
            context.Update(author);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAuthor(int id)
        {
            var existAuthor = await context.Authors.AnyAsync(author => author.Id == id);
            if (!existAuthor)
            {
                return NotFound("Author not found");
            }
            context.Remove(new Author() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}

