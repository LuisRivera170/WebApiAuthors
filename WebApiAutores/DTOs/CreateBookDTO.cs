using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.DTOs
{
    public class CreateBookDTO
    {

        [Required]
        [FirstCapitalLetter]
        [StringLength(maximumLength: 250)]
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
        public List<int> AuthorIds { get; set; }
    }
}
