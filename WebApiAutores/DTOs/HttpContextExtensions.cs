using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WebApiAutores.DTOs
{
    public static class HttpContextExtensions
    {

        public async static Task InserPaginationParametersHeaders<T>(
            this HttpContext httpContext,
            IQueryable<T> queryable
        )
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double totalRecords = await queryable.CountAsync();
            httpContext.Response.Headers.Add("Total-Records", totalRecords.ToString());
        }

    }
}
