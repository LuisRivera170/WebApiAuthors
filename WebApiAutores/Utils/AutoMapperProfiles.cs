using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entities;

namespace WebApiAutores.Utils
{
    public class AutoMapperProfiles: Profile
    {

        public AutoMapperProfiles()
        {
            // Author
            CreateMap<CreateAuthorDTO, Author>();
            CreateMap<Author, AuthorDTO>();

            // Book
            CreateMap<CreateBookDTO, Book>();
            CreateMap<Book, BookDTO>();

            // Comment
            CreateMap<CreateCommentDTO, Comment>();
            CreateMap<Comment, CommentDTO>();
        }

    }
}
