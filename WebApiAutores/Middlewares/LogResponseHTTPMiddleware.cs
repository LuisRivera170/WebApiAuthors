namespace WebApiAutores.Middlewares
{

    public static class LogResponseHTTPMiddlewareExtensions
    {

        public static IApplicationBuilder UseLogResponseHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LogResponseHTTPMiddleware>();
        }

    }

    public class LogResponseHTTPMiddleware
    {

        private readonly RequestDelegate next;
        private readonly ILogger<LogResponseHTTPMiddleware> logger;

        public LogResponseHTTPMiddleware(RequestDelegate next, ILogger<LogResponseHTTPMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (var ms = new MemoryStream())
            {
                var originalBodyResponse = context.Response.Body;
                context.Response.Body = ms;

                await next(context);

                ms.Seek(0, SeekOrigin.Begin);
                string response = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(originalBodyResponse);
                context.Response.Body = originalBodyResponse;

                logger.LogInformation(response);
            }
        }
    }
}
