﻿using Microsoft.OpenApi.Models;

namespace NSE.Identidade.API.Configuration
{
	public static class SwaggerConfig
	{
		public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
		{
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Title = "NerdStore Enterprise Identity API",
					Description = "Esta API faz parte do curso ASP.NET Core Enterprise Application.",
					Contact = new OpenApiContact() { Name = "Felipe Pimenta", Email = "felipepimentasilva5@gmail.com" },
					License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
				});
			});

			return services;
		}
	}
}
