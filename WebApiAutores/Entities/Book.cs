﻿using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.Entities
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [FirstCapitalLetter]
        [StringLength(maximumLength: 250)]
        public string Title { get; set; }
        public List<Comment> Comments { get; set; }
        public List<AuthorBook> AuthorsBooks { get; set; }
    }
}
