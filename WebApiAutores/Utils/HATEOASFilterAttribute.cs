using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAutores.Utils
{
    public class HATEOASFilterAttribute: ResultFilterAttribute
    {

        protected bool MustIncludeHATEOAS(ResultExecutingContext context)
        {
            var result = context.Result as ObjectResult;

            if (!ItsSuccessfullAnswer(result)) 
            {
                return false;
            }

            var headers = context.HttpContext.Request.Headers["includeHATEOAS"];

            if (headers.Count == 0)
            {
                return false;
            }

            var mustIncludeHATEOAS = headers[0];

            if (!mustIncludeHATEOAS.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }

        private static bool ItsSuccessfullAnswer(ObjectResult result)
        {
            if (result == null || result.Value == null)
            {
                return false;
            }
            if (result.StatusCode.HasValue && !result.StatusCode.Value.ToString().StartsWith("2"))
            {
                return false;
            }
            return true;
        }

    }
}
