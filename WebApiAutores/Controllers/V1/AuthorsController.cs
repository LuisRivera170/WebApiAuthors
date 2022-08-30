using Microsoft.AspNetCore.Mvc;
using WebApiAutores.Entities;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApiAutores.Utils;

namespace WebApiAutores.Controllers.V1
{

    [ApiController]
    [Route("api/v1/authors")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<AuthorsController> logger;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IAuthorizationService authorizationService;

        public AuthorsController(
            ApplicationDbContext context,
            ILogger<AuthorsController> logger,
            IMapper mapper,
            IConfiguration configuration,
            IAuthorizationService authorizationService)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
            this.configuration = configuration;
            this.authorizationService = authorizationService;
        }

        [HttpGet("configsV1")]
        public ActionResult<string> getConfigs()
        {
            // configuration["connectionStrings:defaultConnection"];
            return configuration["SUBJECT_EMAIL"];
        }

        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        [HttpGet(Name = "GetAuthorsV1")]
        public async Task<ActionResult<List<AuthorDTOWithBooks>>> GetAuthors([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Authors.AsQueryable();
            await HttpContext.InserPaginationParametersHeaders(queryable);
            var authors = await queryable
                .Include(author => author.AuthorsBooks)
                .ThenInclude(authorBook => authorBook.Book)
                .OrderBy(author => author.Name)
                .Paginate(paginationDTO)
                .ToListAsync();

            /*var authors = await context.Authors
                .Include(author => author.AuthorsBooks)
                .ThenInclude(authorBook => authorBook.Book)
                .ToListAsync();*/

            return mapper.Map<List<AuthorDTOWithBooks>>(authors);
        }

        [HttpGet("{name}", Name = "GetAuthorByNameV1")]
        public async Task<IActionResult> GetAuthor([FromRoute] string name)
        {
            var author = await context.Authors.FirstOrDefaultAsync(author => author.Name.Contains(name));

            if (author == null)
            {
                return NotFound($"Author with name \"{name}\" not found");
            }


            return Ok(mapper.Map<AuthorDTO>(author));
        }

        [HttpGet("{name}/list", Name = "GetAuthorsByNameV1")]
        public async Task<ActionResult<List<AuthorDTO>>> GetAuthorsByName([FromRoute] string name)
        {
            var authors = await context.Authors.Where(author => author.Name.Contains(name)).ToListAsync();
            return mapper.Map<List<AuthorDTO>>(authors);
        }

        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        [HttpGet("{authorId:int}", Name = "GetAuthorByIdV1")]
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

        [HttpPost(Name = "CreateAuthorV1")]
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

            return CreatedAtRoute("GetAuthorByIdV1", new { authorId = author.Id }, authorDTO);
        }

        [HttpPut("{authorId:int}", Name = "UpdateAuthorV1")]
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

        /// <summary>
        /// Deletes an author
        /// </summary>
        /// <param name="authorId">Author identifier</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        [HttpDelete("{authorId:int}", Name = "DeleteAuthorV1")]
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

