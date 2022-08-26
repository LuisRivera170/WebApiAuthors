using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.DTOs
{
    public class CreateBookDTO
    {

        [Required]
        [FirstCapitalLetter]
        public string Title { get; set; }

    }
}
