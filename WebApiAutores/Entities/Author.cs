using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiAutores.Entities
{
    public class Author
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(maximumLength: 15, ErrorMessage = "The length field {0} cannot be more than {1}")]
        public string Name { get; set; }
        [Range(18, 100)]
        [NotMapped]
        public int Age { get; set; }
        public List<Book> Books { get; set; }
    }
}

