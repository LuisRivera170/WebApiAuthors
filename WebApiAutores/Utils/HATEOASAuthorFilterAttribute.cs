using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiAutores.DTOs;
using LinkGenerator = WebApiAutores.Services.LinkGenerator;

namespace WebApiAutores.Utils
{
    public class HATEOASAuthorFilterAttribute: HATEOASFilterAttribute
    {
        private readonly LinkGenerator linkGenerator;

        public HATEOASAuthorFilterAttribute(LinkGenerator linkGenerator)
        {
            this.linkGenerator = linkGenerator;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var mustIncludeHATEOAS = MustIncludeHATEOAS(context);

            if (!mustIncludeHATEOAS)
            {
                await next();
                return;
            }

            var result = context.Result as ObjectResult;

            //var model = result.Value as AuthorDTO ?? throw new ArgumentNullException("A instance of AuthorDTO was expected");
            var authorDTO = result.Value as AuthorDTOWithBooks;
            if (authorDTO == null)
            {
                var authorsDTO = result.Value as List<AuthorDTOWithBooks> ?? throw new ArgumentNullException("A instance of AuthorDTOWithBooks or List<AuthorDTOWithBooks> was expected");

                authorsDTO.ForEach(async authorDTO => await linkGenerator.BuildLinks(authorDTO));
                result.Value = authorsDTO;
            } else
            {
                await linkGenerator.BuildLinks(authorDTO);
            }

            await next();

        }

    }
}
