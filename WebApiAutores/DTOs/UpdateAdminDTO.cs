using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs
{
    public class UpdateAdminDTO
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
