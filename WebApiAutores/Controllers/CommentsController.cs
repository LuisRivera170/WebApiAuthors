﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
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

        [HttpPost]
        public async Task<ActionResult> Post(int bookId, CreateCommentDTO createCommentDTO)
        {
            var book = await context.Books.AnyAsync(book => book.Id == bookId);
            if (!book)
            {
                return BadRequest($"Book with Id {bookId} not found");
            }

            var comment = mapper.Map<Comment>(createCommentDTO);
            comment.BookId = bookId;
            context.Add(comment);
            await context.SaveChangesAsync();

            var commentDTO = mapper.Map<CommentDTO>(comment);

            return CreatedAtRoute("GetCommentById", new { commentId = comment.Id, bookId = bookId }, commentDTO);
        }

    }
}
