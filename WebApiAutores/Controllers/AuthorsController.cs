﻿using Microsoft.AspNetCore.Mvc;
using WebApiAutores.Entities;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using AutoMapper;

namespace WebApiAutores.Controllers
{

    [ApiController]
    [Route("api/authors")]
    public class AuthorsController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<AuthorsController> logger;
        private readonly IMapper mapper;

        public AuthorsController(ApplicationDbContext context, ILogger<AuthorsController> logger, IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Author>>> GetAuthors() {
            return await context.Authors
                //.Include(author => author.Books)
                .ToListAsync();
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetAuthor([FromRoute] string name) 
        {
            var author = await context.Authors.FirstOrDefaultAsync(author => author.Name.Contains(name));
            if (author == null)
            {
                return NotFound($"Author with name \"{name}\" not found");
            }
            return Ok(author);
        }

        [HttpPost]
        public async Task<ActionResult> PostAuthor([FromBody] CreateAuthorDTO authorDTO)
        {
            var existAuthor = await context.Authors.AnyAsync(author => author.Name == authorDTO.Name);

            if (existAuthor)
            {
                return BadRequest($"Author with name {authorDTO.Name} al ready exist");
            }

            var author = mapper.Map<Author>(authorDTO);

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

