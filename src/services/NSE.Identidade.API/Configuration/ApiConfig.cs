using NSE.Web.Api.Core.Identidade;
using NSE.WebAPI.Core.Usuario;

namespace NSE.Identidade.API.Configuration
{
	public static class ApiConfig
	{
		public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
		{
			services.AddControllers();

			services.AddScoped<IAspNetUser, AspNetUser>();

			return services;
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
			app.UseAuthConfiguration();
			app.MapControllers();

            app.UseJwksDiscovery();

            return app;
		}

	}
}
