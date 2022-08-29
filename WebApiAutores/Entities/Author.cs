using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.Entities
{
    public class Author
    {

        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 50)]
        [FirstCapitalLetter]
        public string Name { get; set; }
        public List<AuthorBook> AuthorsBooks { get; set; }
    }
}

