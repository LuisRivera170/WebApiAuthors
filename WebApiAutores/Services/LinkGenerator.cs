using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using WebApiAutores.DTOs;

namespace WebApiAutores.Services
{
    public class LinkGenerator
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;

        public LinkGenerator(
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor
            )
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
        }

        private IUrlHelper BuildUrlHelper()
        {
            var factory = httpContextAccessor
                .HttpContext
                .RequestServices
                .GetRequiredService<IUrlHelperFactory>();

            return factory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        private async Task<bool> IsAdmin()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var result = await authorizationService.AuthorizeAsync(httpContext.User, "isAdmin");
            return result.Succeeded;
        }

        public async Task BuildLinks(AuthorDTO authorDTO)
        {
            var isAdmin = await IsAdmin();
            var Url = BuildUrlHelper();

            authorDTO.Links.Add(new DataHATEOAS(
                link: Url.Link("GetAuthorById", new { authorId = authorDTO.Id }),
                description: "self",
                method: "GET"
            ));

            if (isAdmin)
            {
                authorDTO.Links.Add(new DataHATEOAS(
                    link: Url.Link("UpdateAuthor", new { authorId = authorDTO.Id }),
                    description: "author-update",
                    method: "UPDATE"
                ));
                authorDTO.Links.Add(new DataHATEOAS(
                    link: Url.Link("DeleteAuthor", new { authorId = authorDTO.Id }),
                    description: "author-delete",
                    method: "DELETE"
                ));
            }
        }

    }
}
