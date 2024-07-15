using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Security.Jwt.Core.Jwa;
using NSE.Identidade.API.Data;
using NSE.Identidade.API.Extensions;
using NSE.Web.Api.Core.Identidade;
using System.Text;

namespace NSE.Identidade.API.Configuration
{
	public static class IdentityConfig
	{
		public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, 
			IConfiguration configuration)
		{
            services.AddJwksManager(options => options.Jws = Algorithm.Create(DigitalSignaturesAlgorithm.EcdsaSha256))
                            .PersistKeysToDatabaseStore<ApplicationDbContext>()
                            .UseJwtValidation();

            services.AddDbContext<ApplicationDbContext>(options => 
					options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

			services.AddDefaultIdentity<IdentityUser>()
							.AddRoles<IdentityRole>()
							.AddErrorDescriber<IdentityMensagemPortugues>()
							.AddEntityFrameworkStores<ApplicationDbContext>()
							.AddDefaultTokenProviders();

			return services;
		}
	}
}
