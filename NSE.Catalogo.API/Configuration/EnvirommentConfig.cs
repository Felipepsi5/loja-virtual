namespace NSE.Catalogo.API.Configuration
{
	public static class EnvirommentConfig
	{
		public static WebApplicationBuilder AddEnvirommentConfig(this WebApplicationBuilder builder)
		{
			builder.Configuration
				   .SetBasePath(builder.Environment.ContentRootPath)
				   .AddJsonFile("appsettings.json", true, true)
				   .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
				   .AddEnvironmentVariables();

			return builder;
		}
	}
}
