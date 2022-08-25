using Microsoft.AspNetCore.Mvc;
using WebApiAutores.Entities;

namespace WebApiAutores.Controllers
{

    [ApiController]
    [Route("api/authors")]
    public class AuthorsController: ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Author>> GetAuthors() {
            return new List<Author>() {
                new Author()
                {
                    Id = 1,
                    Name = "Felipe"
                },
                new Author()
                {
                    Id = 2,
                    Name = "Claudia"
                }
            };
        }
    }
}

