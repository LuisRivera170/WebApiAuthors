using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace WebApiAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(option => 
                    option.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
                );

            services
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"))
                );

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.Map("/middleware", app =>
            {
                app.Run(async context => {
                    await context.Response.WriteAsync("Middleware interception");
                });
            });

            app.Use(async (context, next) => 
            {
                using (var ms = new MemoryStream()) 
                {
                    var originalBodyResponse = context.Response.Body;
                    context.Response.Body = ms;

                    await next.Invoke();

                    ms.Seek(0, SeekOrigin.Begin);
                    string response = new StreamReader(ms).ReadToEnd();
                    ms.Seek(0, SeekOrigin.Begin);

                    ms.CopyToAsync(originalBodyResponse);
                    context.Response.Body = originalBodyResponse;

                    logger.LogInformation(response);
                }
            });

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

