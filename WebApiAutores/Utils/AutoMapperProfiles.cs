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
            CreateMap<Author, AuthorDTOWithBooks>()
                .ForMember(
                    authorDTO => authorDTO.Books,
                    options => options.MapFrom(MapAuthorDTOLibros)
                );

            // Book
            CreateMap<CreateBookDTO, Book>()
                .ForMember(
                    book => book.AuthorsBooks, 
                    options => options.MapFrom(MapAuthorsBooks)
                );
            CreateMap<Book, BookDTO>();
            CreateMap<Book, BookDTOWithAuthors>()
                .ForMember(
                    bookDTO => bookDTO.Authors,
                    options => options.MapFrom(MapBookDTOAuthors)
                );

            // Comment
            CreateMap<CreateCommentDTO, Comment>();
            CreateMap<Comment, CommentDTO>();
        }

        private List<BookDTO> MapAuthorDTOLibros(Author author, AuthorDTO authorDTO)
        {
            var books = new List<BookDTO>();
            if (author.AuthorsBooks == null)
            {
                return books;
            }

            foreach (var authorBook in author.AuthorsBooks)
            {
                books.Add(new BookDTO()
                {
                    Id = authorBook.Book.Id,
                    Title = authorBook.Book.Title
                });
            }

            return books;
        }

        private List<AuthorBook> MapAuthorsBooks(CreateBookDTO bookDTO, Book book)
        {
            var authorBooks = new List<AuthorBook>();
            if (bookDTO.AuthorIds == null)
            {
                return authorBooks;
            }
            foreach (var authorId in bookDTO.AuthorIds) 
            {
                authorBooks.Add(new AuthorBook() { AuthorId = authorId });
            }
            return authorBooks;
        }

        private List<AuthorDTO> MapBookDTOAuthors(Book book, BookDTO bookDTO)
        {
            var authors = new List<AuthorDTO>();

            if (book.AuthorsBooks == null)
            {
                return authors;
            }

            foreach (var authorBook in book.AuthorsBooks)
            {
                authors.Add(new AuthorDTO() 
                { 
                    Id = authorBook.AuthorId,
                    Name = authorBook.Author.Name
                });
            }

            return authors;
        }

    }
}
