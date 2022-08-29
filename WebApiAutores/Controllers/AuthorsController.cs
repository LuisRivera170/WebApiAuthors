using Microsoft.AspNetCore.Mvc;
using WebApiAutores.Entities;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebApiAutores.Controllers
{

    [ApiController]
    [Route("api/authors")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthorsController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<AuthorsController> logger;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public AuthorsController(
            ApplicationDbContext context,
            ILogger<AuthorsController> logger,
            IMapper mapper,
            IConfiguration configuration)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [HttpGet("configs")]
        public ActionResult<string> getConfigs() 
        {
            // configuration["connectionStrings:defaultConnection"];
            return configuration["SUBJECT_EMAIL"];
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<AuthorDTOWithBooks>>> GetAuthors() {
            var authors = await context.Authors
                .Include(author => author.AuthorsBooks)
                .ThenInclude(authorBook => authorBook.Book)
                .ToListAsync();

            return mapper.Map<List<AuthorDTOWithBooks>>(authors);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetAuthor([FromRoute] string name) 
        {
            var author = await context.Authors.FirstOrDefaultAsync(author => author.Name.Contains(name));   

            if (author == null)
            {
                return NotFound($"Author with name \"{name}\" not found");
            }


            return Ok(mapper.Map<AuthorDTO>(author));
        }

        [HttpGet("{name}/list")]
        public async Task<ActionResult<List<AuthorDTO>>> GetAuthors([FromRoute] string name)
        {
            var authors = await context.Authors.Where(author => author.Name.Contains(name)).ToListAsync();

            return mapper.Map<List<AuthorDTO>>(authors);
        }

        [HttpGet("{authorId:int}", Name="GetAuthorById")]
        public async Task<ActionResult<AuthorDTOWithBooks>> GetAuthorById(int authorId)
        {
            var author = await context.Authors
                .Include(author => author.AuthorsBooks)
                .ThenInclude(authorBook => authorBook.Book)
                .FirstOrDefaultAsync(author => author.Id == authorId);

            if (author == null)
            {
                return NotFound($"Author with Id {authorId} not found");
            }

            return mapper.Map<AuthorDTOWithBooks>(author);
        }

        [HttpPost]
        public async Task<ActionResult> PostAuthor([FromBody] CreateAuthorDTO createAuthorDTO)
        {
            var existAuthor = await context.Authors.AnyAsync(author => author.Name == createAuthorDTO.Name);

            if (existAuthor)
            {
                return BadRequest($"Author with name {createAuthorDTO.Name} al ready exist");
            }

            var author = mapper.Map<Author>(createAuthorDTO);

            context.Add(author);
            await context.SaveChangesAsync();

            var authorDTO = mapper.Map<AuthorDTO>(author);

            return CreatedAtRoute("GetAuthorById", new { authorId = author.Id }, authorDTO);
        }

        [HttpPut("{authorId:int}")]
        public async Task<ActionResult> PutAuthor(CreateAuthorDTO updateAuthorDTO, int authorId)
        {
            var existAuthor = await context.Authors.AnyAsync(author => author.Id == authorId);
            if (!existAuthor)
            {
                return NotFound("Author not found");
            }
            var author = mapper.Map<Author>(updateAuthorDTO);
            author.Id = authorId;
            context.Update(author);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        [HttpDelete("{authorId:int}")]
        public async Task<ActionResult> DeleteAuthor(int authorId)
        {
            var existAuthor = await context.Authors.AnyAsync(author => author.Id == authorId);
            if (!existAuthor)
            {
                return NotFound("Author not found");
            }
            context.Remove(new Author() { Id = authorId });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}

