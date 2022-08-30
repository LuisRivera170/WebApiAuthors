using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers
{

    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController: ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "GetRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DataHATEOAS>>> Get()
        {
            var dataHATEOAS = new List<DataHATEOAS>();

            var isAdmin = await authorizationService.AuthorizeAsync(User, "isAdmin");


            dataHATEOAS.Add(new DataHATEOAS(link: Url.Link("GetRoot", new {}), description: "self", method: "GET"));
            dataHATEOAS.Add(new DataHATEOAS(link: Url.Link("GetAuthors", new { }), description: "authors", method: "GET"));
            dataHATEOAS.Add(new DataHATEOAS(link: Url.Link("CreateAuthor", new { }), description: "author-create", method: "POST"));
            dataHATEOAS.Add(new DataHATEOAS(link: Url.Link("CreateBook", new { }), description: "book-create", method: "POST"));
            if (isAdmin.Succeeded)
            {
                dataHATEOAS.Add(new DataHATEOAS(link: Url.Link("DeleteAuthor", new { }), description: "author-delete", method: "DELETE"));
            }

            return dataHATEOAS;
        }

    }
}
