using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.DTOs
{
    public class CreateAuthorDTO
    {

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(maximumLength: 15, ErrorMessage = "The length field {0} cannot be more than {1}")]
        [FirstCapitalLetter]
        public string Name { get; set; }

    }
}
