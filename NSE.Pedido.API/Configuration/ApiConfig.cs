using Microsoft.EntityFrameworkCore;
using NSE.Pedidos.Infra.Data;

namespace NSE.Pedidos.API.Configuration
{
    public static class ApiConfig
    {
        public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PedidosContext>(options => 
              options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("Total", builder =>
                builder
                       .AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader());
            });
        }

        public static WebApplication UseApiConfiguration(this WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });
            }

            var builder = new ConfigurationBuilder()
                              .SetBasePath(env.ContentRootPath)
                              .AddUserSecrets<Program>();

            app.UseHttpsRedirection();
            //app.UseAuthConfiguration();
            app.MapControllers();

            return app;
        }
    }
}
