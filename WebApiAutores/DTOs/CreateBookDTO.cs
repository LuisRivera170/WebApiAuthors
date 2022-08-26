using WebApiAutores.Validations;

namespace WebApiAutores.DTOs
{
    public class CreateBookDTO
    {

        [FirstCapitalLetter]
        public string Title { get; set; }

    }
}
