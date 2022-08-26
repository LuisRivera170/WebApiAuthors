using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validations;

namespace WebApiAutores.Entities
{
    public class Author: IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(maximumLength: 15, ErrorMessage = "The length field {0} cannot be more than {1}")]
        [FirstCapitalLetter]
        public string Name { get; set; }
        [Required]
        [Range(14, 100)]
        [NotMapped]
        public int Age { get; set; }
        public string IdCard { get; set; }
        public List<Book> Books { get; set; }

        // Attribute validations must be satisfied to execute model validations
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Age >= 18)
            {
                if (string.IsNullOrEmpty(IdCard))
                {
                    yield return new ValidationResult("The Id Card is required if the author is adult", new String[] { nameof(IdCard) });
                }
            }
        }
    }
}

